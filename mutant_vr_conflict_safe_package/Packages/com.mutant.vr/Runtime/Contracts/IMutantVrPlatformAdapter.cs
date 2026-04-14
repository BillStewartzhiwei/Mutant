using Mutant.VR.Core;

namespace Mutant.VR.Contracts
{
    public interface IMutantVrPlatformAdapter
    {
        string AdapterKey { get; }
        bool CanInstall { get; }
        bool IsInstalled { get; }

        void Install(MutantVrContext context);
        void Tick(float deltaTime);
        void Shutdown();
    }
}
