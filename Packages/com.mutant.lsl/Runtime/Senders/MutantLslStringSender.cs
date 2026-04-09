using System;
using LSL;

namespace Mutant.LSL
{
	public sealed class MutantLslStringSender : MutantLslSenderBase
	{
		public override bool SupportsString => true;

		public MutantLslStringSender(MutantLslStreamDefinition streamDefinition, MutantLslSenderConfig senderConfig, IMutantLslClock clock = null)
			: base(streamDefinition, senderConfig, clock)
		{
			if (streamDefinition.channelFormat != MutantLslChannelFormat.String)
			{
				throw new MutantLslException("MutantLslStringSender requires a String stream definition.");
			}
		}

		public override void SendString(string[] channelValues, double? timestampSeconds = null)
		{
			ThrowIfDisposed();

			if (channelValues == null)
			{
				throw new MutantLslException("String sample is null.");
			}

			ValidateSampleLength(channelValues.Length);

			try
			{
				NativeStreamOutlet.push_sample(channelValues, ResolveTimestamp(timestampSeconds));
			}
			catch (Exception exception)
			{
				State = MutantLslConnectionState.Faulted;
				throw new MutantLslException("Failed to send LSL string sample.", exception);
			}
		}
	}
}
