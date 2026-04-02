using UnityEngine;
using Mutant.Core.Modules;

namespace Mutant.Core.Bootstrap
{
    public sealed class CoreBootstrap : MonoBehaviour
    {
        [SerializeField] private bool dontDestroyOnLoad = true;

        private static bool _initialized;

        private void Awake()
        {
            if (_initialized)
            {
                Destroy(gameObject);
                return;
            }

            _initialized = true;

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
            if (_initialized)
            {
                ModuleManager.Instance.DisposeAll();
                _initialized = false;
            }
        }
    }
}