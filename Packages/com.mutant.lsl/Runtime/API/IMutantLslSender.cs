using System;

namespace Mutant.LSL
{
	public interface IMutantLslSender : IDisposable
	{
		string StreamName { get; }
		string SourceId { get; }
		int ChannelCount { get; }
		bool SupportsString { get; }
		bool SupportsFloat { get; }
		MutantLslConnectionState State { get; }

		void SendString(string[] channelValues, double? timestampSeconds = null);
		void SendFloat(float[] channelValues, double? timestampSeconds = null);
	}
}
