using System;
using System.Collections.Generic;

namespace Mutant.Core.Diagnostics
{
    /// <summary>
    /// Lightweight runtime recorder for Core package diagnostics.
    /// </summary>
    public static class CoreRecorder
    {
        private static readonly List<CoreRecordEntry> Entries = new();

        /// <summary>
        /// Enable/disable recording globally.
        /// </summary>
        public static bool Enabled { get; set; }

        /// <summary>
        /// Maximum number of entries kept in memory.
        /// </summary>
        public static int MaxEntries { get; set; } = 512;

        public static void Record(string category, string message)
        {
            if (!Enabled)
                return;

            Entries.Add(new CoreRecordEntry(DateTime.UtcNow, category, message));

            if (Entries.Count > MaxEntries)
                Entries.RemoveAt(0);
        }

        public static IReadOnlyList<CoreRecordEntry> GetEntries()
        {
            return Entries;
        }

        public static void Clear()
        {
            Entries.Clear();
        }
    }

    public readonly struct CoreRecordEntry
    {
        public CoreRecordEntry(DateTime utcTime, string category, string message)
        {
            UtcTime = utcTime;
            Category = category;
            Message = message;
        }

        public DateTime UtcTime { get; }
        public string Category { get; }
        public string Message { get; }
    }
}
