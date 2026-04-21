using Mutant.VR.Bootstrap;
using Mutant.VR.Contracts;
using Mutant.VR.Core;
using UnityEngine;

namespace Mutant.VR.Diagnostics
{
    [DisallowMultipleComponent]
    public sealed class MutantVrRuntimeStatusView : MonoBehaviour
    {
        [SerializeField] private MutantVrInstaller _installer;
        [SerializeField] private bool _showOnGui = true;

        private void Awake()
        {
            if (_installer == null)
            {
                _installer = GetComponentInParent<MutantVrInstaller>(true);
            }
        }

        private void OnGUI()
        {
            if (!_showOnGui || !Application.isPlaying)
            {
                return;
            }

            MutantVrContext context = _installer != null ? _installer.RuntimeContext : null;

            GUILayout.BeginArea(new Rect(16f, 16f, 340f, 150f), GUI.skin.box);
            GUILayout.Label("Mutant VR Runtime");

            if (context == null)
            {
                GUILayout.Label("Context: null");
                GUILayout.EndArea();
                return;
            }

            GUILayout.Label($"State: {context.State}");

            IMutantVrPlatformAdapter adapter = context.PlatformAdapter;
            GUILayout.Label($"Adapter: {(adapter != null ? adapter.AdapterKey : "null")}");
            GUILayout.Label($"Adapter Installed: {(adapter != null && adapter.IsInstalled)}");
            GUILayout.Label($"Rig Service: {context.ServiceRegistry.Contains<IMutantVrRigService>()}");
            GUILayout.Label($"Control Service: {context.ServiceRegistry.Contains<IMutantVrControlService>()}");
            GUILayout.Label($"Pointer Service: {context.ServiceRegistry.Contains<IMutantVrPointerService>()}");

            GUILayout.EndArea();
        }
    }
}
