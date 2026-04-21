using System;

namespace Mutant.Log.Models
{
	[Serializable]
	public sealed class MutantLogRecord
	{
		public DateTime TimestampUtc { get; }
		public MutantLogSeverity Severity { get; }
		public string CategoryText { get; }
		public string MessageText { get; }
		public string ExceptionText { get; }
		public int FrameIndex { get; }
		public int ManagedThreadId { get; }

		public MutantLogRecord(
			DateTime timestampUtc,
			MutantLogSeverity severity,
			string categoryText,
			string messageText,
			string exceptionText,
			int frameIndex,
			int managedThreadId)
		{
			TimestampUtc = timestampUtc;
			Severity = severity;
			CategoryText = categoryText ?? "General";
			MessageText = messageText ?? string.Empty;
			ExceptionText = exceptionText;
			FrameIndex = frameIndex;
			ManagedThreadId = managedThreadId;
		}
	}
}
