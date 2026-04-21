using UnityEngine;
using Mutant.Core.Diagnostics;
using Mutant.Core.Modules;

namespace Mutant.Core.Bootstrap
{
    public sealed class CoreBootstrap : MonoBehaviour
    {
        [SerializeField] private bool dontDestroyOnLoad = true;

        private static bool _initialized;
        private bool _isOwner;

        private void Awake()
        {
            if (_initialized)
            {
                CoreRecorder.Record("CoreBootstrap", "Duplicate bootstrap detected, destroying instance.");
                Destroy(gameObject);
                return;
            }

            _initialized = true;
            _isOwner = true;
            CoreRecorder.Record("CoreBootstrap", "Owner bootstrap initialized.");

            if (dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);

            ModuleManager.Instance.InitAll();
        }

        private void Update()
        {
            ModuleManager.Instance.UpdateAll();
        }

        private void LateUpdate()
        {
            ModuleManager.Instance.LateUpdateAll();
        }

        private void FixedUpdate()
        {
            ModuleManager.Instance.FixedUpdateAll();
        }

        private void OnDestroy()
        {
            if (_isOwner && _initialized)
            {
                CoreRecorder.Record("CoreBootstrap", "Owner bootstrap destroying, disposing modules.");
                ModuleManager.Instance.DisposeAll();
                _initialized = false;
            }
        }
    }
}
