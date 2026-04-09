using System;
using System.Threading;

namespace Mutant.LSL
{
	public sealed class MutantLslMarkerSender : IDisposable
	{
		private readonly MutantLslStringSender _innerSender;
		private readonly string _producerId;
		private readonly string _sessionId;
		private long _sequenceId;

		public MutantLslMarkerSender(MutantLslStringSender innerSender, MutantLslSenderConfig senderConfig)
		{
			_innerSender = innerSender ?? throw new MutantLslException("Marker sender inner sender is null.");
			_producerId = string.IsNullOrWhiteSpace(senderConfig.producerId) ? MutantLslConstants.DefaultProducerId : senderConfig.producerId;
			_sessionId = senderConfig.sessionId;
		}

		public void Send(string eventKey, string eventLabel = null, string contextJson = null, double? timestampSeconds = null)
		{
			if (string.IsNullOrWhiteSpace(eventKey))
			{
				throw new MutantLslException("Marker eventKey is empty.");
			}

			MutantLslMarkerMessage message = new MutantLslMarkerMessage
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
