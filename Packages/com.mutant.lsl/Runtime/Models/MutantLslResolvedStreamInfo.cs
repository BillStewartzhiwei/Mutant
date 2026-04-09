using LSL;

namespace Mutant.LSL
{
	public sealed class MutantLslResolvedStreamInfo
	{
		public string streamName;
		public string streamType;
		public string sourceId;
		public int channelCount;
		public double nominalRateHz;

		public static MutantLslResolvedStreamInfo FromNative(StreamInfo nativeInfo)
		{
			if (nativeInfo == null)
			{
				return null;
			}

			return new MutantLslResolvedStreamInfo
			{
				streamName = nativeInfo.name(),
				streamType = nativeInfo.type(),
				sourceId = nativeInfo.source_id(),
				channelCount = nativeInfo.channel_count(),
				nominalRateHz = nativeInfo.nominal_srate()
			};
		}
	}
}
