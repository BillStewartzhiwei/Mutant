using UnityEngine;
using Mutant.Log.Models;

namespace Mutant.Log.Sinks
{
	public sealed class MutantUnityConsoleLogSink : IMutantLogSink
	{
		public string SinkDisplayName => "UnityConsole";

		public void Write(MutantLogRecord logRecord)
		{
			if (logRecord == null)
				return;

			string formattedMessage = FormatMessage(logRecord);

			switch (logRecord.Severity)
			{
				case MutantLogSeverity.Trace:
				case MutantLogSeverity.Info:
					Debug.Log(formattedMessage);
					break;

				case MutantLogSeverity.Warning:
					Debug.LogWarning(formattedMessage);
					break;

				case MutantLogSeverity.Error:
				case MutantLogSeverity.Fatal:
					Debug.LogError(formattedMessage);
					break;
			}
		}

		public void Flush()
		{
		}

		public void Dispose()
		{
		}

		private static string FormatMessage(MutantLogRecord logRecord)
		{
			string baseText =
				$"[{logRecord.Severity}] [{logRecord.CategoryText}] " +
				$"{logRecord.MessageText} " +
				$"(frame={logRecord.FrameIndex}, thread={logRecord.ManagedThreadId})";

			if (string.IsNullOrEmpty(logRecord.ExceptionText))
				return baseText;

			return baseText + "\n" + logRecord.ExceptionText;
		}
	}
}
