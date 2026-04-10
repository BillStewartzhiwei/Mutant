using UnityEngine;
using Mutant.Core.Modules;
using Mutant.Log.Config;
using Mutant.Log.Modules;

namespace Mutant.Log.Installers
{
    [DisallowMultipleComponent]
    public sealed class MutantLogModuleInstaller : MonoBehaviour
    {
        [SerializeField] private MutantLogRuntimeSettings _logSettingsAsset;

        private void Awake()
        {
            if (_logSettingsAsset == null)
            {
                Debug.LogWarning(
                    "[MutantLogModuleInstaller] Log settings asset is missing. " +
                    "A runtime fallback settings asset will be used.");
            }

            MutantLogModule.Configure(_logSettingsAsset);
            ModuleManager.Instance.Register<MutantLogModule>();
        }
    }
}