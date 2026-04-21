using LSL;

namespace Mutant.LSL
{
    public sealed class MutantLslService : IMutantLslService
    {
        private readonly IMutantLslClock _clock;
        private readonly MutantLslResolver _resolver;

        public MutantLslService(IMutantLslClock clock = null)
        {
            _clock = clock ?? new MutantLslClock();
            _resolver = new MutantLslResolver();
        }

        public MutantLslStringSender CreateStringSender(MutantLslStreamDefinition streamDefinition, MutantLslSenderConfig senderConfig)
        {
            return new MutantLslStringSender(streamDefinition, senderConfig, _clock);
        }

        public MutantLslFloatSender CreateFloatSender(MutantLslStreamDefinition streamDefinition, MutantLslSenderConfig senderConfig)
        {
            return new MutantLslFloatSender(streamDefinition, senderConfig, _clock);
        }

        public MutantLslMarkerSender CreateMarkerSender(MutantLslSenderConfig senderConfig)
        {
            MutantLslStringSender innerSender = CreateStringSender(MutantLslDefinitionCatalog.CreateMarkerDefinition(), senderConfig);
            return new MutantLslMarkerSender(innerSender, senderConfig);
        }

        public MutantLslEventSender CreateEventSender(MutantLslSenderConfig senderConfig)
        {
            MutantLslStringSender innerSender = CreateStringSender(MutantLslDefinitionCatalog.CreateEventDefinition(), senderConfig);
            return new MutantLslEventSender(innerSender, senderConfig);
        }

        public MutantLslStateSender CreateStateSender(MutantLslSenderConfig senderConfig)
        {
            MutantLslStringSender innerSender = CreateStringSender(MutantLslDefinitionCatalog.CreateStateDefinition(), senderConfig);
            return new MutantLslStateSender(innerSender, senderConfig);
        }

        public MutantLslNumericSender CreateNumericSender(string streamName, string[] channelLabels, double nominalRateHz, MutantLslSenderConfig senderConfig, string units = "")
        {
            MutantLslFloatSender innerSender = CreateFloatSender(
                MutantLslDefinitionCatalog.CreateNumericDefinition(streamName, channelLabels, nominalRateHz, units),
                senderConfig);

            return new MutantLslNumericSender(innerSender);
        }

        public MutantLslPoseSender CreatePoseSender(MutantLslSenderConfig senderConfig, string streamName = null, double nominalRateHz = 90.0, string coordinateSpace = "world")
        {
            if (string.IsNullOrWhiteSpace(senderConfig.coordinateSpace))
            {
                senderConfig.coordinateSpace = coordinateSpace;
            }

            MutantLslFloatSender innerSender = CreateFloatSender(
                MutantLslDefinitionCatalog.CreatePoseDefinition(streamName, nominalRateHz, coordinateSpace),
                senderConfig);

            return new MutantLslPoseSender(innerSender);
        }

        public MutantLslStringReceiver CreateStringReceiver(MutantLslReceiverConfig receiverConfig)
        {
             StreamInfo resolvedInfo = _resolver.ResolveFirstNative(receiverConfig.resolver);
            return new MutantLslStringReceiver(resolvedInfo, receiverConfig);
        }

        public MutantLslFloatReceiver CreateFloatReceiver(MutantLslReceiverConfig receiverConfig)
        {
             StreamInfo resolvedInfo = _resolver.ResolveFirstNative(receiverConfig.resolver);
            return new MutantLslFloatReceiver(resolvedInfo, receiverConfig);
        }

        public MutantLslResolvedStreamInfo[] Resolve(MutantLslResolverConfig config)
        {
            return _resolver.Resolve(config);
        }
    }
}