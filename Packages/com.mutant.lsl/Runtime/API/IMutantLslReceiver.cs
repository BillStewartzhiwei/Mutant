using System;

namespace Mutant.LSL
{
	public interface IMutantLslReceiver : IDisposable
	{
		string StreamName { get; }
		string SourceId { get; }
		int ChannelCount { get; }
		bool SupportsString { get; }
		bool SupportsFloat { get; }
		MutantLslConnectionState State { get; }

		bool TryPullString(string[] reusableBuffer, out double timestampSeconds, double timeoutSeconds = 0.0);
		bool TryPullFloat(float[] reusableBuffer, out double timestampSeconds, double timeoutSeconds = 0.0);
	}
}
