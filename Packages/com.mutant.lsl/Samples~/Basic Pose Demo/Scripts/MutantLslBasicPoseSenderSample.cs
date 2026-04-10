using Mutant.Core.Modules;
using UnityEngine;

namespace Mutant.LSL.Samples
{
    public sealed class MutantLslBasicPoseSenderSample : MonoBehaviour
    {
        [SerializeField]
        private Transform poseSource;

        [SerializeField]
        private string sourceId = "mutant:samples:pose:sender01";

        [SerializeField]
        private string producerPackageId = "com.mutant.lsl.samples";

        [SerializeField]
        private string producerId = "com.mutant.lsl.samples.pose";

        [SerializeField]
        private string role = "pose_sender";

        [SerializeField]
        private string streamName = "Mutant.Pose";

        [SerializeField]
        private string coordinateSpace = "world";

        [SerializeField]
        private double nominalRateHz = 90.0;

        private MutantLslPoseSender _poseSender;

        private void Start()
        {
            MutantLslModule lslModule = ModuleManager.Instance.GetModule<MutantLslModule>();
            if (lslModule == null || !lslModule.IsInitialized)
            {
                Debug.LogError("[MutantLslBasicPoseSenderSample] MutantLslModule is not registered or initialized.");
                enabled = false;
                return;
            }

            if (lslModule.Service == null)
            {
                Debug.LogError("[MutantLslBasicPoseSenderSample] MutantLslModule Service is null.");
                enabled = false;
                return;
            }

            MutantLslSenderConfig senderConfig = new MutantLslSenderConfig
            {
                sourceId = sourceId,
                producerPackageId = producerPackageId,
                producerId = producerId,
                role = role,
                coordinateSpace = coordinateSpace
            };

            _poseSender = lslModule.Service.CreatePoseSender(
                senderConfig,
                streamName,
                nominalRateHz,
                coordinateSpace);

            Debug.Log("[MutantLslBasicPoseSenderSample] Pose sender created from MutantLslModule.");
        }

        private void Update()
        {
            if (_poseSender == null || poseSource == null)
            {
                return;
            }

            _poseSender.SendPose(poseSource);
        }

        private void OnDestroy()
        {
            _poseSender?.Dispose();
        }
    }
}