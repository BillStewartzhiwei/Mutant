using System;

namespace Mutant.LSL
{
	[Serializable]
	public sealed class MutantLslStreamDefinition
	{
		public string streamName;
		public string streamType;
		public string profileId;
		public string profileVersion = MutantLslConstants.DefaultProfileVersion;
		public MutantLslChannelFormat channelFormat = MutantLslChannelFormat.Float32;
		public double nominalRateHz = MutantLslConstants.IrregularRate;
		public MutantLslChannelDefinition[] channels;
		public MutantLslMetadataEntry[] metadataEntries;

		public int ChannelCount
		{
			get { return channels == null ? 0 : channels.Length; }
		}
	}
}
