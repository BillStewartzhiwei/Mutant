namespace Mutant.LSL
{
	public interface IMutantLslService
	{
		MutantLslStringSender CreateStringSender(MutantLslStreamDefinition streamDefinition, MutantLslSenderConfig senderConfig);
		MutantLslFloatSender CreateFloatSender(MutantLslStreamDefinition streamDefinition, MutantLslSenderConfig senderConfig);

		MutantLslMarkerSender CreateMarkerSender(MutantLslSenderConfig senderConfig);
		MutantLslEventSender CreateEventSender(MutantLslSenderConfig senderConfig);
		MutantLslStateSender CreateStateSender(MutantLslSenderConfig senderConfig);
		MutantLslNumericSender CreateNumericSender(string streamName, string[] channelLabels, double nominalRateHz, MutantLslSenderConfig senderConfig, string units = "");
		MutantLslPoseSender CreatePoseSender(MutantLslSenderConfig senderConfig, string streamName = null, double nominalRateHz = 90.0, string coordinateSpace = "world");

		MutantLslStringReceiver CreateStringReceiver(MutantLslReceiverConfig receiverConfig);
		MutantLslFloatReceiver CreateFloatReceiver(MutantLslReceiverConfig receiverConfig);

		MutantLslResolvedStreamInfo[] Resolve(MutantLslResolverConfig config);
	}
}
