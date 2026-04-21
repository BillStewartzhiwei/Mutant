using System;
using LSL;

namespace Mutant.LSL
{
	public sealed class MutantLslStringReceiver : MutantLslReceiverBase
	{
		public override bool SupportsString => true;

		public MutantLslStringReceiver( StreamInfo resolvedInfo, MutantLslReceiverConfig receiverConfig)
			: base(resolvedInfo, receiverConfig)
		{
		}

		public override bool TryPullString(string[] reusableBuffer, out double timestampSeconds, double timeoutSeconds = 0.0)
		{
			ThrowIfDisposed();

			if (reusableBuffer == null)
			{
				throw new MutantLslException("String receiver buffer is null.");
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
				throw new MutantLslException("Failed to pull LSL string sample.", exception);
			}
		}
	}
}
