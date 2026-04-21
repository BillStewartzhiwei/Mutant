using System;
using UnityEngine;

namespace Mutant.VR.Rig
{
    [Serializable]
    public sealed class MutantVrRigReferences
    {
        public Transform HeadAnchorTransform;
        public Transform LeftHandAnchorTransform;
        public Transform RightHandAnchorTransform;
        public Transform LeftPointerOriginTransform;
        public Transform RightPointerOriginTransform;

        public MutantVrRigReferences CreateCopy()
        {
            return new MutantVrRigReferences
            {
                HeadAnchorTransform = HeadAnchorTransform,
                LeftHandAnchorTransform = LeftHandAnchorTransform,
                RightHandAnchorTransform = RightHandAnchorTransform,
                LeftPointerOriginTransform = LeftPointerOriginTransform,
                RightPointerOriginTransform = RightPointerOriginTransform
            };
        }
    }
}
