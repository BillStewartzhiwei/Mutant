using System;
using System.Threading;

namespace Mutant.LSL
{
	public sealed class MutantLslStateSender : IDisposable
	{
		private readonly MutantLslStringSender _innerSender;
		private readonly string _producerId;
		private readonly string _sessionId;
		private long _sequenceId;

		public MutantLslStateSender(MutantLslStringSender innerSender, MutantLslSenderConfig senderConfig)
		{
			_innerSender = innerSender ?? throw new MutantLslException("State sender inner sender is null.");
			_producerId = string.IsNullOrWhiteSpace(senderConfig.producerId) ? MutantLslConstants.DefaultProducerId : senderConfig.producerId;
			_sessionId = senderConfig.sessionId;
		}

		public void Send(string stateKey, string stateValue, string previousValue = null, string contextJson = null, double? timestampSeconds = null)
		{
			if (string.IsNullOrWhiteSpace(stateKey))
			{
				throw new MutantLslException("State stateKey is empty.");
			}

			if (string.IsNullOrWhiteSpace(stateValue))
			{
				throw new MutantLslException("State stateValue is empty.");
			}

			MutantLslStateMessage message = new MutantLslStateMessage
			{
				sequenceId = Interlocked.Increment(ref _sequenceId),
				stateKey = stateKey,
				stateValue = stateValue,
				previousValue = previousValue,
				producer = _producerId,
				sessionId = _sessionId,
				sentAtUtc = DateTime.UtcNow.ToString("O"),
				contextJson = contextJson
			};

			_innerSender.SendString(new[] { message.ToJson() }, timestampSeconds);
		}

		public void Dispose()
		{
			_innerSender?.Dispose();
		}
	}
}
