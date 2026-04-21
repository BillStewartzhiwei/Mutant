using System;
using System.Collections.Generic;

namespace Mutant.Core
{
    /// <summary>
    /// 模块注册中心。
    /// 只负责收集模块及其描述，不执行生命周期。
    /// </summary>
    public sealed class ModuleRegistry
    {
        private readonly Dictionary<string, ModuleDescriptor> _descriptorsById =
            new Dictionary<string, ModuleDescriptor>(StringComparer.Ordinal);

        private readonly List<ModuleDescriptor> _descriptorsInRegistrationOrder =
            new List<ModuleDescriptor>();

        private int _registrationCounter;

        public int Count
        {
            get { return _descriptorsInRegistrationOrder.Count; }
        }

        public ModuleDescriptor Register(
            IMutantModule module,
            bool autoStart = true,
            bool required = true,
            bool allowFailure = false,
            string displayName = null)
        {
            if (module == null)
            {
                throw new ArgumentNullException(nameof(module));
            }

            if (string.IsNullOrWhiteSpace(module.ModuleId))
            {
                throw new ArgumentException("ModuleId cannot be null or empty.", nameof(module));
            }

            if (_descriptorsById.ContainsKey(module.ModuleId))
            {
                throw new InvalidOperationException(
                    string.Format("Module id '{0}' is already registered.", module.ModuleId));
            }

            var descriptor = new ModuleDescriptor(
                module,
                _registrationCounter++,
                autoStart,
                required,
                allowFailure,
                displayName);

            _descriptorsById.Add(descriptor.ModuleId, descriptor);
            _descriptorsInRegistrationOrder.Add(descriptor);

            return descriptor;
        }

        public void RegisterRange(IEnumerable<IMutantModule> modules)
        {
            if (modules == null)
            {
                throw new ArgumentNullException(nameof(modules));
            }

            foreach (var module in modules)
            {
                Register(module);
            }
        }

        public bool Contains(string moduleId)
        {
            return _descriptorsById.ContainsKey(moduleId);
        }

        public bool TryGetDescriptor(string moduleId, out ModuleDescriptor descriptor)
        {
            return _descriptorsById.TryGetValue(moduleId, out descriptor);
        }

        public bool TryGetModule(string moduleId, out IMutantModule module)
        {
            ModuleDescriptor descriptor;
            if (_descriptorsById.TryGetValue(moduleId, out descriptor))
            {
                module = descriptor.Instance;
                return true;
            }

            module = null;
            return false;
        }

        public T GetModule<T>() where T : class, IMutantModule
        {
            for (int i = 0; i < _descriptorsInRegistrationOrder.Count; i++)
            {
                var instance = _descriptorsInRegistrationOrder[i].Instance as T;
                if (instance != null)
                {
                    return instance;
                }
            }

            return null;
        }

        public IReadOnlyList<ModuleDescriptor> GetAllDescriptors()
        {
            return _descriptorsInRegistrationOrder.AsReadOnly();
        }

        public IReadOnlyList<ModuleDescriptor> GetAutoStartDescriptors()
        {
            var results = new List<ModuleDescriptor>();

            for (int i = 0; i < _descriptorsInRegistrationOrder.Count; i++)
            {
                if (_descriptorsInRegistrationOrder[i].AutoStart)
                {
                    results.Add(_descriptorsInRegistrationOrder[i]);
                }
            }

            return results.AsReadOnly();
        }
    }
}