using Mutant.Core.Modules;
using UnityEngine;

namespace Mutant.LSL.Samples
{
    public sealed class MutantLslBasicMarkerSenderSample : MonoBehaviour
    {
        [SerializeField] private string sourceId = "mutant:samples:marker:sender01";
        [SerializeField] private string producerPackageId = "com.mutant.lsl.samples";
        [SerializeField] private string producerId = "com.mutant.lsl.samples.marker";
        [SerializeField] private string sessionId = "sample_session_001";
        [SerializeField] private string role = "sender";
        [SerializeField] private KeyCode sendKey = KeyCode.Space;

        private MutantLslMarkerSender _markerSender;

        private void Start()
        {
            MutantLslModule lslModule = ModuleManager.Instance.GetModule<MutantLslModule>();
            if (lslModule == null || !lslModule.IsInitialized)
            {
                Debug.LogError("[MutantLslBasicMarkerSenderSample] MutantLslModule is not registered or initialized.");
                enabled = false;
                return;
            }

            MutantLslSenderConfig senderConfig = new MutantLslSenderConfig
            {
                sourceId = sourceId,
                producerPackageId = producerPackageId,
                producerId = producerId,
                sessionId = sessionId,
                role = role
            };

            _markerSender = lslModule.Service.CreateMarkerSender(senderConfig);
            Debug.Log("[MutantLslBasicMarkerSenderSample] Marker sender created from MutantLslModule.");
        }

        private void Update()
        {
            if (Input.GetKeyDown(sendKey))
            {
                _markerSender.Send(
                    "sample.marker.sent",
                    "SampleMarkerSent",
                    "{\"input\":\"Space\",\"sample\":\"Basic Marker Demo\"}");
            }
        }

        private void OnDestroy()
        {
            _markerSender?.Dispose();
        }
    }
}