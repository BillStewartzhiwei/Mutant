using System;
using LSL;

namespace Mutant.LSL
{
	public sealed class MutantLslFloatSender : MutantLslSenderBase
	{
		public override bool SupportsFloat => true;

		public MutantLslFloatSender(MutantLslStreamDefinition streamDefinition, MutantLslSenderConfig senderConfig, IMutantLslClock clock = null)
			: base(streamDefinition, senderConfig, clock)
		{
			if (streamDefinition.channelFormat != MutantLslChannelFormat.Float32)
			{
				throw new MutantLslException("MutantLslFloatSender requires a Float32 stream definition.");
			}
		}

		public override void SendFloat(float[] channelValues, double? timestampSeconds = null)
		{
			ThrowIfDisposed();

			if (channelValues == null)
			{
				throw new MutantLslException("Float sample is null.");
			}

			ValidateSampleLength(channelValues.Length);

			try
			{
				NativeStreamOutlet.push_sample(channelValues, ResolveTimestamp(timestampSeconds));
			}
			catch (Exception exception)
			{
				State = MutantLslConnectionState.Faulted;
				throw new MutantLslException("Failed to send LSL float sample.", exception);
			}
		}
	}
}
