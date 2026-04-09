using System;
using LSL;

namespace Mutant.LSL
{
	public sealed class MutantLslFloatReceiver : MutantLslReceiverBase
	{
		public override bool SupportsFloat => true;

		public MutantLslFloatReceiver(StreamInfo resolvedInfo, MutantLslReceiverConfig receiverConfig)
			: base(resolvedInfo, receiverConfig)
		{
		}

		public override bool TryPullFloat(float[] reusableBuffer, out double timestampSeconds, double timeoutSeconds = 0.0)
		{
			ThrowIfDisposed();

			if (reusableBuffer == null)
			{
				throw new MutantLslException("Float receiver buffer is null.");
			}

			ValidateBufferLength(reusableBuffer.Length);

			try
			{
				timestampSeconds = NativeStreamInlet.pull_sample(reusableBuffer, ResolvePullTimeout(timeoutSeconds));
				return timestampSeconds != 0.0;
			}
			catch (Exception exception)
			{
				State = MutantLslConnectionState.Faulted;
				throw new MutantLslException("Failed to pull LSL float sample.", exception);
			}
		}
	}
}
