using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Mutant.Log.Models;
using Mutant.Log.Sinks;

namespace Mutant.Log.Services
{
    public sealed class MutantLogService : IDisposable
    {
        private readonly List<IMutantLogSink> _sinkList = new List<IMutantLogSink>();

        public MutantLogSeverity MinimumSeverity { get; set; }

        public MutantLogService(MutantLogSeverity minimumSeverity)
        {
            MinimumSeverity = minimumSeverity;
        }

        public IReadOnlyList<IMutantLogSink> RegisteredSinks => _sinkList;

        public void AddSink(IMutantLogSink logSinkInstance)
        {
            if (logSinkInstance == null)
                return;

            if (_sinkList.Contains(logSinkInstance))
                return;

            _sinkList.Add(logSinkInstance);
        }

        public void RemoveSink(IMutantLogSink logSinkInstance)
        {
            if (logSinkInstance == null)
                return;

            _sinkList.Remove(logSinkInstance);
        }

        public void Write(
            MutantLogSeverity severity,
            string categoryText,
            string messageText,
            string exceptionText = null)
        {
            if (severity < MinimumSeverity)
                return;

            MutantLogRecord record = new MutantLogRecord(
                DateTime.UtcNow,
                severity,
                string.IsNullOrWhiteSpace(categoryText) ? "General" : categoryText,
                messageText ?? string.Empty,
                exceptionText,
                Time.frameCount,
                Thread.CurrentThread.ManagedThreadId);

            for (int i = 0; i < _sinkList.Count; i++)
                _sinkList[i].Write(record);
        }

        public void Flush()
        {
            for (int i = 0; i < _sinkList.Count; i++)
                _sinkList[i].Flush();
        }

        public void Dispose()
        {
            for (int i = _sinkList.Count - 1; i >= 0; i--)
                _sinkList[i].Dispose();

            _sinkList.Clear();
        }
    }
}