using Mutant.VR.Controls;
using UnityEngine;

namespace Mutant.VR.Contracts
{
    public interface IMutantVrRigService
    {
        void RefreshRig();

        bool TryGetHeadTransform(out Transform headTransform);
        bool TryGetHandTransform(MutantVrControllerHand hand, out Transform handTransform);
        bool TryGetPointerOriginTransform(MutantVrControllerHand hand, out Transform pointerOriginTransform);
    }
}