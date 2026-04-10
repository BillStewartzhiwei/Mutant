using Mutant.Core.Modules;

namespace Mutant.LSL
{
    public sealed class MutantLslModule : IModule
    {
        public const string ModuleDisplayName = "Mutant.LSL";

        public string Name => ModuleDisplayName;

        public int Priority => 0;

        public bool IsInitialized { get; private set; }

        public IMutantLslService Service { get; private set; }

        public void Init()
        {
            if (IsInitialized)
            {
                return;
            }

            Service = new MutantLslService();
            IsInitialized = true;
        }

        public void Update()
        {
        }

        public void LateUpdate()
        {
        }

        public void FixedUpdate()
        {
        }

        public void Dispose()
        {
            Service = null;
            IsInitialized = false;
        }
    }
}