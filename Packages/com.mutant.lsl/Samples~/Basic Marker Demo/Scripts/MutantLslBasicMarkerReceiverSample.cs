using Mutant.Core.Modules;
using UnityEngine;

namespace Mutant.LSL.Samples
{
    public sealed class MutantLslBasicMarkerReceiverSample : MonoBehaviour
    {
        [SerializeField] private string targetStreamName = "Mutant.Marker";
        [SerializeField] private double resolveTimeoutSeconds = 2.0;
        [SerializeField] private double pullTimeoutSeconds = 0.0;

        private MutantLslStringReceiver _receiver;
        private string[] _buffer;

        private void Start()
        {
            MutantLslModule lslModule = ModuleManager.Instance.GetModule<MutantLslModule>();
            if (lslModule == null || !lslModule.IsInitialized)
            {
                Debug.LogError("[MutantLslBasicMarkerReceiverSample] MutantLslModule is not registered or initialized.");
                enabled = false;
                return;
            }

            MutantLslReceiverConfig receiverConfig = new MutantLslReceiverConfig
            {
                resolver = new MutantLslResolverConfig
                {
                    resolveMode = MutantLslResolveMode.ByProperty,
                    propertyName = "name",
                    propertyValue = targetStreamName,
                    minimumResultCount = 1,
                    timeoutSeconds = resolveTimeoutSeconds
                },
                openStreamOnCreate = true,
                defaultPullTimeoutSeconds = pullTimeoutSeconds
            };

            try
            {
                _receiver = lslModule.Service.CreateStringReceiver(receiverConfig);
                _buffer = new string[1];
            }
            catch (MutantLslException exception)
            {
                Debug.LogWarning($"[MutantLslBasicMarkerReceiverSample] Failed to create receiver: {exception.Message}");
            }
        }

        private void Update()
        {
            if (_receiver == null)
            {
                return;
            }

            if (_receiver.TryPullString(_buffer, out double timestampSeconds, pullTimeoutSeconds))
            {
                Debug.Log($"[MutantLslBasicMarkerReceiverSample] Received marker ts={timestampSeconds}, payload={_buffer[0]}");
            }
        }

        private void OnDestroy()
        {
            _receiver?.Dispose();
        }
    }
}