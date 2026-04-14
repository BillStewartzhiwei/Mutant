using Mutant.VR.Config;
using Mutant.VR.Contracts;
using Mutant.VR.Core;
using Mutant.VR.Rig;
using UnityEngine;

namespace Mutant.VR.Bootstrap
{
    [DisallowMultipleComponent]
    public sealed class MutantVrInstaller : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private MutantVrSettings _settings;

        [Header("Scene References")]
        [SerializeField] private MutantVrRigRoot _rigRoot;
        [SerializeField] private MonoBehaviour _platformAdapterBehaviour;

        private MutantVrContext _runtimeContext;
        private MutantVrModule _runtimeModule;

        public MutantVrContext RuntimeContext => _runtimeContext;
        public MutantVrModule RuntimeModule => _runtimeModule;

        private void Awake()
        {
            BuildRuntime();

            bool shouldAutoInstall = _settings == null || _settings.AutoInstallOnAwake;
            if (shouldAutoInstall)
            {
                InstallRuntime();
            }
        }

        private void Update()
        {
            bool shouldAutoTick = _settings == null || _settings.AutoTickInUpdate;
            if (!shouldAutoTick)
            {
                return;
            }

            _runtimeModule?.Tick(Time.deltaTime);
        }

        private void OnDestroy()
        {
            bool shouldAutoShutdown = _settings == null || _settings.AutoShutdownOnDestroy;
            if (shouldAutoShutdown)
            {
                _runtimeModule?.Shutdown();
            }
        }

        [ContextMenu("Mutant/VR/Install Runtime")]
        public void InstallRuntime()
        {
            if (_runtimeModule == null)
            {
                BuildRuntime();
            }

            _runtimeModule?.Install();
        }

        [ContextMenu("Mutant/VR/Tick Runtime Once")]
        public void TickRuntimeOnce()
        {
            _runtimeModule?.Tick(Time.deltaTime);
        }

        [ContextMenu("Mutant/VR/Shutdown Runtime")]
        public void ShutdownRuntime()
        {
            _runtimeModule?.Shutdown();
        }

        private void BuildRuntime()
        {
            IMutantVrPlatformAdapter adapter = ResolvePlatformAdapter();
            MutantVrRigReferences rigReferences = _rigRoot != null
                ? _rigRoot.BuildRigReferences()
                : new MutantVrRigReferences();

            _runtimeContext = new MutantVrContext();
            _runtimeContext.Configure(_settings, rigReferences, adapter);

            _runtimeModule = new MutantVrModule(_runtimeContext);
        }

        private IMutantVrPlatformAdapter ResolvePlatformAdapter()
        {
            if (_platformAdapterBehaviour is IMutantVrPlatformAdapter serializedAdapter)
            {
                return serializedAdapter;
            }

            MonoBehaviour[] localBehaviours = GetComponents<MonoBehaviour>();
            for (int i = 0; i < localBehaviours.Length; i++)
            {
                if (localBehaviours[i] is IMutantVrPlatformAdapter foundAdapter)
                {
                    _platformAdapterBehaviour = localBehaviours[i];
                    return foundAdapter;
                }
            }

            MonoBehaviour[] childBehaviours = GetComponentsInChildren<MonoBehaviour>(true);
            for (int i = 0; i < childBehaviours.Length; i++)
            {
                if (childBehaviours[i] is IMutantVrPlatformAdapter foundAdapter)
                {
                    _platformAdapterBehaviour = childBehaviours[i];
                    return foundAdapter;
                }
            }

            Debug.LogWarning("[MutantVrInstaller] No IMutantVrPlatformAdapter found.");
            return null;
        }

        private void Reset()
        {
            if (_rigRoot == null)
            {
                _rigRoot = GetComponentInChildren<MutantVrRigRoot>(true);
            }
        }
    }
}