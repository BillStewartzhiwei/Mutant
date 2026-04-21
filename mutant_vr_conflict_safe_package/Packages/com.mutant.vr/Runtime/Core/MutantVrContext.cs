using Mutant.VR.Config;
using Mutant.VR.Contracts;
using Mutant.VR.Rig;
using UnityEngine;

namespace Mutant.VR.Core
{
    public sealed class MutantVrContext
    {
        public MutantVrSettings Settings { get; private set; }
        public MutantVrRigReferences RigReferences { get; private set; } = new MutantVrRigReferences();
        public IMutantVrPlatformAdapter PlatformAdapter { get; private set; }
        public MutantVrServiceRegistry ServiceRegistry { get; } = new MutantVrServiceRegistry();
        public MutantVrLifecycleState State { get; private set; } = MutantVrLifecycleState.None;

        public void Configure(
            MutantVrSettings settings,
            MutantVrRigReferences rigReferences,
            IMutantVrPlatformAdapter platformAdapter)
        {
            Settings = settings;
            RigReferences = rigReferences != null ? rigReferences.CreateCopy() : new MutantVrRigReferences();
            PlatformAdapter = platformAdapter;
            State = MutantVrLifecycleState.Configured;
        }

        public void ReplaceRigReferences(MutantVrRigReferences rigReferences)
        {
            RigReferences = rigReferences != null ? rigReferences.CreateCopy() : new MutantVrRigReferences();
        }

        public void SetState(MutantVrLifecycleState state)
        {
            State = state;
        }

        public void RegisterService<TService>(TService service)
            where TService : class
        {
            ServiceRegistry.Register(service);
        }

        public bool TryGetService<TService>(out TService service)
            where TService : class
        {
            return ServiceRegistry.TryGet(out service);
        }

        public void LogVerbose(string message)
        {
            if (Settings != null && Settings.EnableVerboseLogging)
            {
                Debug.Log($"[MutantVr] {message}");
            }
        }

        public void LogWarning(string message)
        {
            Debug.LogWarning($"[MutantVr] {message}");
        }

        public void LogError(string message)
        {
            Debug.LogError($"[MutantVr] {message}");
        }
    }
}
