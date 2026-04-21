using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mutant.Core
{
    /// <summary>
    /// Mutant 启动总入口。
    /// 当前版本已接入启动编排 + Loop 绑定 + PlayerLoop 安装。
    /// </summary>
    public static class MutantRuntimeBootstrap
    {
        private static bool _isBooted;
        private static ModuleRegistry _registry;
        private static ModuleManager _moduleManager;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            _isBooted = false;
            _registry = null;
            _moduleManager = null;
        }

        public static bool IsBooted
        {
            get { return _isBooted; }
        }

        public static ModuleRegistry Registry
        {
            get { return _registry; }
        }

        public static ModuleManager ModuleManager
        {
            get { return _moduleManager; }
        }

        public static ModuleManager Boot(params IMutantModule[] modules)
        {
            return Boot((IEnumerable<IMutantModule>)modules);
        }

        public static ModuleManager Boot(IEnumerable<IMutantModule> modules)
        {
            if (_isBooted)
            {
                return _moduleManager;
            }

            if (modules == null)
            {
                throw new ArgumentNullException(nameof(modules));
            }

            _registry = new ModuleRegistry();

            foreach (var module in modules)
            {
                _registry.Register(module);
            }

            var resolver = new DependencyResolver();
            var resolutionResult = resolver.Resolve(_registry.GetAutoStartDescriptors());

            var planner = new StartupPlanner();
            var startupPlan = planner.Build(resolutionResult);

            _moduleManager = new ModuleManager(_registry, startupPlan);

            Debug.Log("[Mutant.Core] Boot begin.");

            _moduleManager.ExecuteRegister();
            _moduleManager.ExecuteInitialize();
            _moduleManager.ExecuteStart();

            var bindingBuilder = new LoopBindingBuilder();
            bindingBuilder.BindStartedModules(_moduleManager);

            PlayerLoopInstaller.Install();

            _isBooted = true;

            Debug.Log(string.Format(
                "[Mutant.Core] Boot success. Update={0}, FixedUpdate={1}, LateUpdate={2}",
                MutantLoop.UpdateCount,
                MutantLoop.FixedUpdateCount,
                MutantLoop.LateUpdateCount));

            return _moduleManager;
        }

        public static void Shutdown()
        {
            if (!_isBooted || _moduleManager == null)
            {
                return;
            }

            Debug.Log("[Mutant.Core] Shutdown begin.");

            PlayerLoopInstaller.Uninstall();
            MutantLoop.Clear();

            _moduleManager.ExecuteStop();
            _moduleManager.ExecuteDispose();

            _moduleManager = null;
            _registry = null;
            _isBooted = false;

            Debug.Log("[Mutant.Core] Shutdown complete.");
        }
    }
}