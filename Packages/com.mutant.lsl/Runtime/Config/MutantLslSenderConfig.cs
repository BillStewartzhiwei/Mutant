using System;

namespace Mutant.LSL
{
	[Serializable]
	public sealed class MutantLslSenderConfig
	{
		public string sourceId = "mutant:com.mutant.lsl:main:default";
		public string streamNameOverride = "";
		public string streamTypeOverride = "";
		public double nominalRateOverrideHz = -1.0;
		public int chunkSizeSamples = 0;
		public int maxBufferedSeconds = MutantLslConstants.DefaultMaxBufferedSeconds;
		public bool useLocalClockIfTimestampMissing = true;

		public string producerPackageId = MutantLslConstants.DefaultProducerPackageId;
		public string producerId = MutantLslConstants.DefaultProducerId;
		public string sessionId = "";
		public string subjectId = "";
		public string role = "";
		public string payloadEncoding = MutantLslConstants.DefaultPayloadEncoding;
		public string coordinateSpace = "";
		public string unitsOverride = "";

		public MutantLslMetadataEntry[] extraMetadata;
	}
}
