using System;

namespace Mutant.LSL
{
	public sealed class MutantLslNumericSender : IDisposable
	{
		private readonly MutantLslFloatSender _innerSender;

		public MutantLslNumericSender(MutantLslFloatSender innerSender)
		{
			_innerSender = innerSender ?? throw new MutantLslException("Numeric sender inner sender is null.");
		}

		public void Send(float[] channelValues, double? timestampSeconds = null)
		{
			_innerSender.SendFloat(channelValues, timestampSeconds);
		}

		public void Dispose()
		{
			_innerSender?.Dispose();
		}
	}
}
