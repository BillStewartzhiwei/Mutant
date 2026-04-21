using Mutant.VR.Bootstrap;
using UnityEngine;

namespace Mutant.VR.CoreBridge
{
    [DisallowMultipleComponent]
    public sealed class MutantVrCoreInstallExample : MonoBehaviour
    {
        [SerializeField] private MutantVrInstaller _installer;

        public void InstallFromCore()
        {
            _installer?.InstallRuntime();
        }

        public void TickFromCore(float deltaTime)
        {
            if (_installer != null && _installer.RuntimeModule != null)
            {
                _installer.RuntimeModule.Tick(deltaTime);
            }
        }

        public void ShutdownFromCore()
        {
            _installer?.ShutdownRuntime();
        }

        private void Reset()
        {
            if (_installer == null)
            {
                _installer = GetComponent<MutantVrInstaller>();
            }
        }
    }
}