using Mutant.Core.Modules;
using UnityEngine;

namespace Mutant.LSL.Samples
{
    public sealed class MutantLslBasicResolveSample : MonoBehaviour
    {
        [ContextMenu("Resolve Streams")]
        public void ResolveStreams()
        {
            MutantLslModule lslModule = ModuleManager.Instance.GetModule<MutantLslModule>();
            if (lslModule == null || !lslModule.IsInitialized)
            {
                Debug.LogError("MutantLslModule is not registered or initialized.");
                return;
            }

            IMutantLslService lslService = lslModule.Service;
            if (lslService == null)
            {
                Debug.LogError("MutantLslModule Service is null.");
                return;
            }

            MutantLslResolvedStreamInfo[] results = lslService.Resolve(
                new MutantLslResolverConfig
                {
                    resolveMode = MutantLslResolveMode.ByProperty,
                    propertyName = "name",
                    propertyValue = "Mutant.Marker",
                    minimumResultCount = 1,
                    timeoutSeconds = 2.0
                });

            Debug.Log(results == null ? "No results." : $"Found {results.Length} streams.");
        }
    }
}