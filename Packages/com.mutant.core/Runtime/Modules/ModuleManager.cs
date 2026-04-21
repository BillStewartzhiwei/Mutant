using System;
using System.Collections.Generic;

namespace Mutant.Core
{
    /// <summary>
    /// 模块生命周期执行器。
    /// 负责 Register / Init / Start / Stop / Dispose。
    /// </summary>
    public sealed class ModuleManager
    {
        private readonly ModuleRegistry _registry;
        private readonly StartupPlan _startupPlan;
        private readonly Dictionary<string, MutantModuleState> _states =
            new Dictionary<string, MutantModuleState>(StringComparer.Ordinal);
        private readonly List<ModuleStartupError> _errors =
            new List<ModuleStartupError>();

        public ModuleManager(ModuleRegistry registry, StartupPlan startupPlan)
        {
            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            if (startupPlan == null)
            {
                throw new ArgumentNullException(nameof(startupPlan));
            }

            _registry = registry;
            _startupPlan = startupPlan;

            var allDescriptors = _registry.GetAllDescriptors();
            for (int i = 0; i < allDescriptors.Count; i++)
            {
                _states[allDescriptors[i].ModuleId] = MutantModuleState.None;
            }

            for (int i = 0; i < _startupPlan.OrderedModules.Count; i++)
            {
                _states[_startupPlan.OrderedModules[i].ModuleId] = MutantModuleState.Resolved;
            }
        }

        public StartupPlan StartupPlan
        {
            get { return _startupPlan; }
        }

        public IReadOnlyList<ModuleStartupError> Errors
        {
            get { return _errors.AsReadOnly(); }
        }

        public MutantModuleState GetState(string moduleId)
        {
            MutantModuleState state;
            if (_states.TryGetValue(moduleId, out state))
            {
                return state;
            }

            return MutantModuleState.None;
        }

        public IMutantModule GetModule(string moduleId)
        {
            IMutantModule module;
            return _registry.TryGetModule(moduleId, out module) ? module : null;
        }

        public T GetModule<T>() where T : class, IMutantModule
        {
            return _registry.GetModule<T>();
        }

        public IReadOnlyList<ModuleDescriptor> GetStartedDescriptorsInStartupOrder()
        {
            var results = new List<ModuleDescriptor>();

            for (int i = 0; i < _startupPlan.OrderedModules.Count; i++)
            {
                var descriptor = _startupPlan.OrderedModules[i];
                if (GetState(descriptor.ModuleId) == MutantModuleState.Started)
                {
                    results.Add(descriptor);
                }
            }

            return results.AsReadOnly();
        }

        public void ExecuteRegister()
        {
            for (int i = 0; i < _startupPlan.OrderedModules.Count; i++)
            {
                var descriptor = _startupPlan.OrderedModules[i];

                if (GetState(descriptor.ModuleId) == MutantModuleState.Failed)
                {
                    continue;
                }

                TryRunLifecycle(descriptor, "Register", MutantModuleState.Registered, descriptor.Instance.OnRegister);
            }
        }

        public void ExecuteInitialize()
        {
            for (int b = 0; b < _startupPlan.Batches.Count; b++)
            {
                var batch = _startupPlan.Batches[b];

                for (int i = 0; i < batch.Modules.Count; i++)
                {
                    var descriptor = batch.Modules[i];

                    if (GetState(descriptor.ModuleId) == MutantModuleState.Failed)
                    {
                        continue;
                    }

                    EnsureDependenciesInitialized(descriptor);

                    SetState(descriptor.ModuleId, MutantModuleState.Initializing);
                    TryRunLifecycle(descriptor, "Init", MutantModuleState.Initialized, descriptor.Instance.OnInit);
                }
            }
        }

        public void ExecuteStart()
        {
            for (int b = 0; b < _startupPlan.Batches.Count; b++)
            {
                var batch = _startupPlan.Batches[b];

                for (int i = 0; i < batch.Modules.Count; i++)
                {
                    var descriptor = batch.Modules[i];

                    if (GetState(descriptor.ModuleId) == MutantModuleState.Failed)
                    {
                        continue;
                    }

                    EnsureDependenciesStarted(descriptor);

                    SetState(descriptor.ModuleId, MutantModuleState.Starting);
                    TryRunLifecycle(descriptor, "Start", MutantModuleState.Started, descriptor.Instance.OnStart);
                }
            }
        }

        public void ExecuteStop()
        {
            for (int i = _startupPlan.OrderedModules.Count - 1; i >= 0; i--)
            {
                var descriptor = _startupPlan.OrderedModules[i];
                var state = GetState(descriptor.ModuleId);

                if (state != MutantModuleState.Started)
                {
                    continue;
                }

                SetState(descriptor.ModuleId, MutantModuleState.Stopping);
                TryRunLifecycle(descriptor, "Stop", MutantModuleState.Stopped, descriptor.Instance.OnStop);
            }
        }

        public void ExecuteDispose()
        {
            for (int i = _startupPlan.OrderedModules.Count - 1; i >= 0; i--)
            {
                var descriptor = _startupPlan.OrderedModules[i];
                var state = GetState(descriptor.ModuleId);

                if (state == MutantModuleState.Disposed || state == MutantModuleState.None)
                {
                    continue;
                }

                SetState(descriptor.ModuleId, MutantModuleState.Disposing);
                TryRunLifecycle(descriptor, "Dispose", MutantModuleState.Disposed, descriptor.Instance.OnDispose);
            }
        }

        private void EnsureDependenciesInitialized(ModuleDescriptor descriptor)
        {
            var dependencies = descriptor.Dependencies;
            if (dependencies == null || dependencies.Count == 0)
            {
                return;
            }

            for (int i = 0; i < dependencies.Count; i++)
            {
                var dependencyId = dependencies[i];
                var dependencyState = GetState(dependencyId);

                if (dependencyState == MutantModuleState.Initialized ||
                    dependencyState == MutantModuleState.Started ||
                    dependencyState == MutantModuleState.Stopped ||
                    dependencyState == MutantModuleState.Disposed)
                {
                    continue;
                }

                throw new InvalidOperationException(
                    string.Format(
                        "Module '{0}' cannot initialize because dependency '{1}' is in state '{2}'.",
                        descriptor.ModuleId,
                        dependencyId,
                        dependencyState));
            }
        }

        private void EnsureDependenciesStarted(ModuleDescriptor descriptor)
        {
            var dependencies = descriptor.Dependencies;
            if (dependencies == null || dependencies.Count == 0)
            {
                return;
            }

            for (int i = 0; i < dependencies.Count; i++)
            {
                var dependencyId = dependencies[i];
                var dependencyState = GetState(dependencyId);

                if (dependencyState == MutantModuleState.Started)
                {
                    continue;
                }

                throw new InvalidOperationException(
                    string.Format(
                        "Module '{0}' cannot start because dependency '{1}' is in state '{2}'.",
                        descriptor.ModuleId,
                        dependencyId,
                        dependencyState));
            }
        }

        private void TryRunLifecycle(
            ModuleDescriptor descriptor,
            string stage,
            MutantModuleState successState,
            Action action)
        {
            try
            {
                action();
                SetState(descriptor.ModuleId, successState);
            }
            catch (Exception exception)
            {
                SetState(descriptor.ModuleId, MutantModuleState.Failed);
                _errors.Add(new ModuleStartupError(descriptor.ModuleId, stage, exception));

                if (descriptor.IsCritical)
                {
                    throw new InvalidOperationException(
                        string.Format("Critical module '{0}' failed during '{1}'.", descriptor.ModuleId, stage),
                        exception);
                }
            }
        }

        private void SetState(string moduleId, MutantModuleState state)
        {
            _states[moduleId] = state;
        }
    }
}