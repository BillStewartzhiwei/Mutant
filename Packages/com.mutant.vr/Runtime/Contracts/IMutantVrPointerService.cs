using Mutant.VR.Controls;

namespace Mutant.VR.Contracts
{
    public interface IMutantVrPointerService
    {
        bool TryBuildPointer(MutantVrControllerHand hand, out UnityEngine.Ray pointerRay);
    }
}