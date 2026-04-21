using Mutant.VR.Controls;

namespace Mutant.VR.Contracts
{
    public interface IMutantVrControlService
    {
        MutantVrControlSnapshot CurrentSnapshot { get; }

        void UpdateControls(float deltaTime);
    }
}
