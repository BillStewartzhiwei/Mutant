using System;

namespace Mutant.LSL
{
	[Serializable]
	public sealed class MutantLslReceiverConfig
	{
		public MutantLslResolverConfig resolver = new MutantLslResolverConfig();
		public int maxBufferLengthSeconds = MutantLslConstants.DefaultMaxBufferLengthSeconds;
		public int maxChunkLengthSamples = MutantLslConstants.DefaultMaxChunkLengthSamples;
		public bool recover = true;
		public bool openStreamOnCreate = true;
		public double defaultPullTimeoutSeconds = MutantLslConstants.DefaultPullTimeoutSeconds;
	}
}
