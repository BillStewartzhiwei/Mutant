using System;
using System.Threading;

namespace Mutant.LSL
{
	public sealed class MutantLslEventSender : IDisposable
	{
		private readonly MutantLslStringSender _innerSender;
		private readonly string _producerId;
		private readonly string _sessionId;
		private long _sequenceId;

		public MutantLslEventSender(MutantLslStringSender innerSender, MutantLslSenderConfig senderConfig)
		{
			_innerSender = innerSender ?? throw new MutantLslException("Event sender inner sender is null.");
			_producerId = string.IsNullOrWhiteSpace(senderConfig.producerId) ? MutantLslConstants.DefaultProducerId : senderConfig.producerId;
			_sessionId = senderConfig.sessionId;
		}

		public void Send(string eventKey, string eventLabel = null, string contextJson = null, double? timestampSeconds = null)
		{
			if (string.IsNullOrWhiteSpace(eventKey))
			{
				throw new MutantLslException("Event eventKey is empty.");
			}

			MutantLslEventMessage message = new MutantLslEventMessage
			{
				sequenceId = Interlocked.Increment(ref _sequenceId),
				eventKey = eventKey,
				eventLabel = eventLabel,
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
