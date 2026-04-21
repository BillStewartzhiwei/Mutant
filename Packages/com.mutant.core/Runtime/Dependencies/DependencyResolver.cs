using System;
using System.Collections.Generic;
using System.Text;

namespace Mutant.Core
{
    /// <summary>
    /// 负责依赖校验、阶段逆向依赖校验、循环依赖检测和稳定拓扑排序。
    /// </summary>
    public sealed class DependencyResolver
    {
        public DependencyResolutionResult Resolve(IReadOnlyList<ModuleDescriptor> descriptors)
        {
            if (descriptors == null)
            {
                throw new ArgumentNullException(nameof(descriptors));
            }

            var descriptorById = BuildDescriptorMap(descriptors);
            ValidateDependencies(descriptors, descriptorById);

            var orderedModules = TopologicalSort(descriptors, descriptorById);
            return new DependencyResolutionResult(orderedModules.AsReadOnly());
        }

        private static Dictionary<string, ModuleDescriptor> BuildDescriptorMap(IReadOnlyList<ModuleDescriptor> descriptors)
        {
            var descriptorById = new Dictionary<string, ModuleDescriptor>(StringComparer.Ordinal);

            for (int i = 0; i < descriptors.Count; i++)
            {
                var descriptor = descriptors[i];
                if (descriptor == null)
                {
                    throw new InvalidOperationException("Descriptor cannot be null.");
                }

                if (descriptorById.ContainsKey(descriptor.ModuleId))
                {
                    throw new InvalidOperationException(
                        string.Format("Duplicate descriptor id detected: '{0}'.", descriptor.ModuleId));
                }

                descriptorById.Add(descriptor.ModuleId, descriptor);
            }

            return descriptorById;
        }

        private static void ValidateDependencies(
            IReadOnlyList<ModuleDescriptor> descriptors,
            Dictionary<string, ModuleDescriptor> descriptorById)
        {
            for (int i = 0; i < descriptors.Count; i++)
            {
                var descriptor = descriptors[i];
                var dependencies = descriptor.Dependencies;

                if (dependencies == null)
                {
                    continue;
                }

                for (int d = 0; d < dependencies.Count; d++)
                {
                    var dependencyId = dependencies[d];

                    if (string.IsNullOrWhiteSpace(dependencyId))
                    {
                        throw new InvalidOperationException(
                            string.Format("Module '{0}' contains an empty dependency id.", descriptor.ModuleId));
                    }

                    ModuleDescriptor dependencyDescriptor;
                    if (!descriptorById.TryGetValue(dependencyId, out dependencyDescriptor))
                    {
                        throw new InvalidOperationException(
                            string.Format(
                                "Module '{0}' depends on '{1}', but that module is not registered.",
                                descriptor.ModuleId,
                                dependencyId));
                    }

                    if (descriptor.BootPhase < dependencyDescriptor.BootPhase)
                    {
                        throw new InvalidOperationException(
                            string.Format(
                                "Module '{0}' in phase '{1}' cannot depend on later phase module '{2}' in phase '{3}'.",
                                descriptor.ModuleId,
                                descriptor.BootPhase,
                                dependencyDescriptor.ModuleId,
                                dependencyDescriptor.BootPhase));
                    }
                }
            }
        }

        private static List<ModuleDescriptor> TopologicalSort(
            IReadOnlyList<ModuleDescriptor> descriptors,
            Dictionary<string, ModuleDescriptor> descriptorById)
        {
            var indegreeById = new Dictionary<string, int>(StringComparer.Ordinal);
            var adjacencyById = new Dictionary<string, List<ModuleDescriptor>>(StringComparer.Ordinal);

            for (int i = 0; i < descriptors.Count; i++)
            {
                indegreeById[descriptors[i].ModuleId] = 0;
                adjacencyById[descriptors[i].ModuleId] = new List<ModuleDescriptor>();
            }

            for (int i = 0; i < descriptors.Count; i++)
            {
                var descriptor = descriptors[i];
                var dependencies = descriptor.Dependencies;

                if (dependencies == null)
                {
                    continue;
                }

                for (int d = 0; d < dependencies.Count; d++)
                {
                    var dependencyId = dependencies[d];
                    indegreeById[descriptor.ModuleId] += 1;
                    adjacencyById[dependencyId].Add(descriptor);
                }
            }

            var available = new List<ModuleDescriptor>();
            for (int i = 0; i < descriptors.Count; i++)
            {
                if (indegreeById[descriptors[i].ModuleId] == 0)
                {
                    available.Add(descriptors[i]);
                }
            }

            available.Sort(CompareDescriptor);

            var ordered = new List<ModuleDescriptor>(descriptors.Count);

            while (available.Count > 0)
            {
                var current = available[0];
                available.RemoveAt(0);

                ordered.Add(current);

                var dependents = adjacencyById[current.ModuleId];
                for (int i = 0; i < dependents.Count; i++)
                {
                    var dependent = dependents[i];
                    indegreeById[dependent.ModuleId] -= 1;

                    if (indegreeById[dependent.ModuleId] == 0)
                    {
                        available.Add(dependent);
                    }
                }

                available.Sort(CompareDescriptor);
            }

            if (ordered.Count != descriptors.Count)
            {
                var message = BuildCycleMessage(descriptors, indegreeById, descriptorById);
                throw new InvalidOperationException(message);
            }

            return ordered;
        }

        private static int CompareDescriptor(ModuleDescriptor left, ModuleDescriptor right)
        {
            var phaseCompare = left.BootPhase.CompareTo(right.BootPhase);
            if (phaseCompare != 0)
            {
                return phaseCompare;
            }

            var orderCompare = left.Order.CompareTo(right.Order);
            if (orderCompare != 0)
            {
                return orderCompare;
            }

            var registrationCompare = left.RegistrationIndex.CompareTo(right.RegistrationIndex);
            if (registrationCompare != 0)
            {
                return registrationCompare;
            }

            return string.CompareOrdinal(left.ModuleId, right.ModuleId);
        }

        private static string BuildCycleMessage(
            IReadOnlyList<ModuleDescriptor> descriptors,
            Dictionary<string, int> indegreeById,
            Dictionary<string, ModuleDescriptor> descriptorById)
        {
            var builder = new StringBuilder();
            builder.AppendLine("Circular dependency detected between modules.");

            for (int i = 0; i < descriptors.Count; i++)
            {
                var descriptor = descriptors[i];
                int indegree;
                if (!indegreeById.TryGetValue(descriptor.ModuleId, out indegree))
                {
                    continue;
                }

                if (indegree <= 0)
                {
                    continue;
                }

                builder.Append("- ");
                builder.Append(descriptor.ModuleId);
                builder.Append(" depends on: ");

                var dependencies = descriptor.Dependencies;
                if (dependencies == null || dependencies.Count == 0)
                {
                    builder.AppendLine("(none)");
                    continue;
                }

                for (int d = 0; d < dependencies.Count; d++)
                {
                    builder.Append(dependencies[d]);
                    if (d < dependencies.Count - 1)
                    {
                        builder.Append(", ");
                    }
                }

                builder.AppendLine();
            }

            return builder.ToString();
        }
    }
}