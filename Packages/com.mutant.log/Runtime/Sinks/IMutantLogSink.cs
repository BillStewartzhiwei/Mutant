using System;
using Mutant.Log.Models;

namespace Mutant.Log.Sinks
{
	public interface IMutantLogSink : IDisposable
	{
		string SinkDisplayName { get; }
		void Write(MutantLogRecord logRecord);
		void Flush();
	}
}
