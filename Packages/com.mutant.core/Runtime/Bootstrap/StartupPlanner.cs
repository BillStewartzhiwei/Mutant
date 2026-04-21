using System;
using System.Collections.Generic;

namespace Mutant.Core
{
    /// <summary>
    /// 根据依赖解析结果构建启动批次。
    /// 先按 Phase，再在 Phase 内按依赖层级切分批次。
    /// </summary>
    public sealed class StartupPlanner
    {
        public StartupPlan Build(DependencyResolutionResult resolutionResult)
        {
            if (resolutionResult == null)
            {
                throw new ArgumentNullException(nameof(resolutionResult));
            }

            var orderedModules = resolutionResult.OrderedModules;
            var batches = new List<StartupBatch>();

            var phaseOrder = new[]
            {
                MutantBootPhase.Infrastructure,
                MutantBootPhase.Platform,
                MutantBootPhase.Domain,
                MutantBootPhase.Application
            };

            for (int p = 0; p < phaseOrder.Length; p++)
            {
                var phase = phaseOrder[p];
                var phaseModules = CollectPhaseModules(orderedModules, phase);

                if (phaseModules.Count == 0)
                {
                    continue;
                }

                BuildPhaseBatches(phaseModules, phase, batches);
            }

            return new StartupPlan(batches.AsReadOnly());
        }

        private static List<ModuleDescriptor> CollectPhaseModules(
            IReadOnlyList<ModuleDescriptor> orderedModules,
            MutantBootPhase phase)
        {
            var results = new List<ModuleDescriptor>();

            for (int i = 0; i < orderedModules.Count; i++)
            {
                if (orderedModules[i].BootPhase == phase)
                {
                    results.Add(orderedModules[i]);
                }
            }

            return results;
        }

        private static void BuildPhaseBatches(
            List<ModuleDescriptor> phaseModules,
            MutantBootPhase phase,
            List<StartupBatch> outputBatches)
        {
            var localIndexById = new Dictionary<string, int>(StringComparer.Ordinal);
            var indegreeById = new Dictionary<string, int>(StringComparer.Ordinal);
            var adjacencyById = new Dictionary<string, List<ModuleDescriptor>>(StringComparer.Ordinal);

            for (int i = 0; i < phaseModules.Count; i++)
            {
                var descriptor = phaseModules[i];
                localIndexById[descriptor.ModuleId] = i;
                indegreeById[descriptor.ModuleId] = 0;
                adjacencyById[descriptor.ModuleId] = new List<ModuleDescriptor>();
            }

            for (int i = 0; i < phaseModules.Count; i++)
            {
                var descriptor = phaseModules[i];
                var dependencies = descriptor.Dependencies;

                if (dependencies == null)
                {
                    continue;
                }

                for (int d = 0; d < dependencies.Count; d++)
                {
                    var dependencyId = dependencies[d];
                    int dependencyLocalIndex;
                    if (!localIndexById.TryGetValue(dependencyId, out dependencyLocalIndex))
                    {
                        continue;
                    }

                    indegreeById[descriptor.ModuleId] += 1;
                    adjacencyById[dependencyId].Add(descriptor);
                }
            }

            var ready = new List<ModuleDescriptor>();
            for (int i = 0; i < phaseModules.Count; i++)
            {
                if (indegreeById[phaseModules[i].ModuleId] == 0)
                {
                    ready.Add(phaseModules[i]);
                }
            }

            ready.Sort((left, right) =>
            {
                var leftIndex = localIndexById[left.ModuleId];
                var rightIndex = localIndexById[right.ModuleId];
                return leftIndex.CompareTo(rightIndex);
            });

            var processedCount = 0;
            var batchIndex = 0;

            while (ready.Count > 0)
            {
                var currentBatchModules = new List<ModuleDescriptor>(ready);
                ready.Clear();

                outputBatches.Add(new StartupBatch(
                    phase,
                    batchIndex++,
                    currentBatchModules.AsReadOnly()));

                for (int i = 0; i < currentBatchModules.Count; i++)
                {
                    var current = currentBatchModules[i];
                    processedCount++;

                    var dependents = adjacencyById[current.ModuleId];
                    for (int j = 0; j < dependents.Count; j++)
                    {
                        var dependent = dependents[j];
                        indegreeById[dependent.ModuleId] -= 1;

                        if (indegreeById[dependent.ModuleId] == 0)
                        {
                            ready.Add(dependent);
                        }
                    }
                }

                ready.Sort((left, right) =>
                {
                    var leftIndex = localIndexById[left.ModuleId];
                    var rightIndex = localIndexById[right.ModuleId];
                    return leftIndex.CompareTo(rightIndex);
                });
            }

            if (processedCount != phaseModules.Count)
            {
                throw new InvalidOperationException(
                    string.Format("Failed to build startup batches for phase '{0}'.", phase));
            }
        }
    }
}