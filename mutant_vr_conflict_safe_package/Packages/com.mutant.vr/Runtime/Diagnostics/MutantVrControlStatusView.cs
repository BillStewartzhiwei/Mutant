using Mutant.VR.Bootstrap;
using Mutant.VR.Contracts;
using Mutant.VR.Controls;
using UnityEngine;

namespace Mutant.VR.Diagnostics
{
    [DisallowMultipleComponent]
    public sealed class MutantVrControlStatusView : MonoBehaviour
    {
        [SerializeField] private MutantVrInstaller _installer;
        [SerializeField] private bool _showOnGui = true;

        private IMutantVrControlService _controlService;

        private void Awake()
        {
            if (_installer == null)
            {
                _installer = GetComponentInParent<MutantVrInstaller>(true);
            }
        }

        private void OnGUI()
        {
            if (!_showOnGui || !Application.isPlaying || !TryResolveControlService())
            {
                return;
            }

            MutantVrControlSnapshot snapshot = _controlService.CurrentSnapshot;

            GUILayout.BeginArea(new Rect(16f, 176f, 320f, 180f), GUI.skin.box);
            GUILayout.Label("Mutant VR Controls");
            GUILayout.Label($"Frame: {snapshot.FrameIndex}");
            GUILayout.Label($"Realtime: {snapshot.RealtimeSinceStartup:F3}");
            GUILayout.Label($"Left Trigger: {snapshot.LeftTriggerPressed}");
            GUILayout.Label($"Right Trigger: {snapshot.RightTriggerPressed}");
            GUILayout.Label($"Left Grip: {snapshot.LeftGripPressed}");
            GUILayout.Label($"Right Grip: {snapshot.RightGripPressed}");
            GUILayout.Label($"Left Primary: {snapshot.LeftPrimaryPressed}");
            GUILayout.Label($"Right Primary: {snapshot.RightPrimaryPressed}");
            GUILayout.Label($"Left Stick: {snapshot.LeftStickAxis}");
            GUILayout.Label($"Right Stick: {snapshot.RightStickAxis}");
            GUILayout.EndArea();
        }

        private bool TryResolveControlService()
        {
            if (_controlService != null)
            {
                return true;
            }

            if (_installer == null || _installer.RuntimeContext == null)
            {
                return false;
            }

            return _installer.RuntimeContext.TryGetService(out _controlService);
        }
    }
}
