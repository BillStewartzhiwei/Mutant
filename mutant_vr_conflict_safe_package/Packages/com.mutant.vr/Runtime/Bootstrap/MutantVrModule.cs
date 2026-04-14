using System;
using Mutant.VR.Contracts;
using Mutant.VR.Core;

namespace Mutant.VR.Bootstrap
{
    public sealed class MutantVrModule
    {
        public MutantVrContext Context { get; }

        public MutantVrModule(MutantVrContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public bool Install()
        {
            IMutantVrPlatformAdapter platformAdapter = Context.PlatformAdapter;
            if (platformAdapter == null)
            {
                Context.SetState(MutantVrLifecycleState.Failed);
                Context.LogError("Install failed: PlatformAdapter is null.");
                return false;
            }

            if (!platformAdapter.CanInstall)
            {
                Context.SetState(MutantVrLifecycleState.Failed);
                Context.LogError($"Install failed: Adapter '{platformAdapter.AdapterKey}' is not ready.");
                return false;
            }

            if (Context.State == MutantVrLifecycleState.Ready || Context.State == MutantVrLifecycleState.Running)
            {
                return true;
            }

            try
            {
                Context.SetState(MutantVrLifecycleState.Installing);
                platformAdapter.Install(Context);

                if (!platformAdapter.IsInstalled)
                {
                    Context.SetState(MutantVrLifecycleState.Failed);
                    Context.LogError($"Install failed: Adapter '{platformAdapter.AdapterKey}' did not enter installed state.");
                    return false;
                }

                Context.SetState(MutantVrLifecycleState.Ready);
                Context.LogVerbose($"Installed with adapter '{platformAdapter.AdapterKey}'.");
                return true;
            }
            catch (Exception exception)
            {
                Context.SetState(MutantVrLifecycleState.Failed);
                Context.LogError($"Install exception: {exception}");
                return false;
            }
        }

        public void Tick(float deltaTime)
        {
            if (Context.State == MutantVrLifecycleState.Failed || Context.State == MutantVrLifecycleState.Stopped)
            {
                return;
            }

            if (Context.State == MutantVrLifecycleState.Configured && !Install())
            {
                return;
            }

            IMutantVrPlatformAdapter platformAdapter = Context.PlatformAdapter;
            if (platformAdapter == null || !platformAdapter.IsInstalled)
            {
                return;
            }

            platformAdapter.Tick(deltaTime);
            Context.SetState(MutantVrLifecycleState.Running);
        }

        public void Shutdown()
        {
            IMutantVrPlatformAdapter platformAdapter = Context.PlatformAdapter;
            if (platformAdapter != null && platformAdapter.IsInstalled)
            {
                platformAdapter.Shutdown();
            }

            Context.ServiceRegistry.Clear();
            Context.SetState(MutantVrLifecycleState.Stopped);
            Context.LogVerbose("Shutdown completed.");
        }
    }
}
