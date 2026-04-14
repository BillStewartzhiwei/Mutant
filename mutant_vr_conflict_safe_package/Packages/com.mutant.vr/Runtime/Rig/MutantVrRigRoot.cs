using UnityEngine;

namespace Mutant.VR.Rig
{
    [DisallowMultipleComponent]
    public sealed class MutantVrRigRoot : MonoBehaviour
    {
        [SerializeField] private Transform _headAnchor;
        [SerializeField] private Transform _leftHandAnchor;
        [SerializeField] private Transform _rightHandAnchor;
        [SerializeField] private Transform _leftPointerOrigin;
        [SerializeField] private Transform _rightPointerOrigin;

        public MutantVrRigReferences BuildRigReferences()
        {
            return new MutantVrRigReferences
            {
                HeadAnchorTransform = _headAnchor,
                LeftHandAnchorTransform = _leftHandAnchor,
                RightHandAnchorTransform = _rightHandAnchor,
                LeftPointerOriginTransform = _leftPointerOrigin != null ? _leftPointerOrigin : _leftHandAnchor,
                RightPointerOriginTransform = _rightPointerOrigin != null ? _rightPointerOrigin : _rightHandAnchor
            };
        }

        private void Reset()
        {
            if (_headAnchor == null && Camera.main != null)
            {
                _headAnchor = Camera.main.transform;
            }
        }
    }
}
