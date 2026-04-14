using Mutant.VR.Bootstrap;
using Mutant.VR.Contracts;
using Mutant.VR.Controls;
using UnityEngine;

namespace Mutant.VR.Diagnostics
{
    [DisallowMultipleComponent]
    public sealed class MutantVrRigStatusView : MonoBehaviour
    {
        [SerializeField] private MutantVrInstaller _installer;
        [SerializeField] private float _axisLength = 0.15f;
        [SerializeField] private float _forwardLength = 0.35f;

        private IMutantVrRigService _rigService;

        private void Awake()
        {
            if (_installer == null)
            {
                _installer = GetComponentInParent<MutantVrInstaller>(true);
            }
        }

        private void Update()
        {
            if (!TryResolveRigService())
            {
                return;
            }

            if (_rigService.TryGetHeadTransform(out Transform headTransform))
            {
                DrawBasis(headTransform, Color.white);
            }

            if (_rigService.TryGetHandTransform(MutantVrControllerHand.Left, out Transform leftHandTransform))
            {
                DrawBasis(leftHandTransform, Color.green);
            }

            if (_rigService.TryGetHandTransform(MutantVrControllerHand.Right, out Transform rightHandTransform))
            {
                DrawBasis(rightHandTransform, Color.cyan);
            }
        }

        private bool TryResolveRigService()
        {
            if (_rigService != null)
            {
                return true;
            }

            if (_installer == null || _installer.RuntimeContext == null)
            {
                return false;
            }

            return _installer.RuntimeContext.TryGetService(out _rigService);
        }

        private void DrawBasis(Transform targetTransform, Color forwardColor)
        {
            Vector3 position = targetTransform.position;
            Debug.DrawRay(position, targetTransform.forward * _forwardLength, forwardColor);
            Debug.DrawRay(position, targetTransform.up * _axisLength, Color.yellow);
            Debug.DrawRay(position, targetTransform.right * _axisLength, Color.red);
        }
    }
}