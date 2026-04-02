using System.Collections.Generic;
using System.Linq;

namespace Mutant.Core.Modules
{
    public sealed class ModuleManager
    {
        private static ModuleManager _instance;
        public static ModuleManager Instance => _instance ??= new ModuleManager();

        private readonly List<IModule> _modules = new();
        private bool _initialized;

        private ModuleManager() { }

        public void Register(IModule module)
        {
            if (module == null)
                return;

            if (_modules.Contains(module))
                return;

            _modules.Add(module);
            _modules.Sort((a, b) => a.Priority.CompareTo(b.Priority));

            if (_initialized)
                module.Init();
        }

        public void Unregister(IModule module)
        {
            if (module == null)
                return;

            if (_modules.Remove(module) && _initialized)
                module.Dispose();
        }

        public T GetModule<T>() where T : class, IModule
        {
            return _modules.OfType<T>().FirstOrDefault();
        }

        public IReadOnlyList<IModule> GetAllModules()
        {
            return _modules;
        }

        public void InitAll()
        {
            if (_initialized)
                return;

            _initialized = true;

            foreach (var module in _modules)
                module.Init();
        }

        public void UpdateAll()
        {
            if (!_initialized) return;

            foreach (var module in _modules)
                module.Update();
        }

        public void LateUpdateAll()
        {
            if (!_initialized) return;

            foreach (var module in _modules)
                module.LateUpdate();
        }

        public void FixedUpdateAll()
        {
            if (!_initialized) return;

            foreach (var module in _modules)
                module.FixedUpdate();
        }

        public void DisposeAll()
        {
            if (!_initialized)
            {
                _modules.Clear();
                return;
            }

            for (int i = _modules.Count - 1; i >= 0; i--)
                _modules[i].Dispose();

            _modules.Clear();
            _initialized = false;
        }
    }
}