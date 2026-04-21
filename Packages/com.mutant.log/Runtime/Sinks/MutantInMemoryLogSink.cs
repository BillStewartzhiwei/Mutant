using System;
using System.Collections.Generic;
using Mutant.Log.Models;

namespace Mutant.Log.Sinks
{
	public sealed class MutantInMemoryLogSink : IMutantLogSink
	{
		private readonly List<MutantLogRecord> _recordBuffer;
		private readonly int _recordCapacity;

		public string SinkDisplayName => "InMemory";

		public MutantInMemoryLogSink(int retainedCapacity)
		{
			_recordCapacity = Math.Max(10, retainedCapacity);
			_recordBuffer = new List<MutantLogRecord>(_recordCapacity);
		}

		public void Write(MutantLogRecord logRecord)
		{
			if (logRecord == null)
				return;

			if (_recordBuffer.Count >= _recordCapacity)
				_recordBuffer.RemoveAt(0);

			_recordBuffer.Add(logRecord);
		}

		public IReadOnlyList<MutantLogRecord> GetSnapshot()
		{
			return _recordBuffer.ToArray();
		}

		public void Flush()
		{
		}

		public void Dispose()
		{
			_recordBuffer.Clear();
		}
	}
}
