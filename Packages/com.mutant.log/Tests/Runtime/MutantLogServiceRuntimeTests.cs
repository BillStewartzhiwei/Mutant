using NUnit.Framework;
using Mutant.Log.Models;
using Mutant.Log.Services;
using Mutant.Log.Sinks;

namespace Mutant.Log.Tests.Runtime
{
    public class MutantLogServiceRuntimeTests
    {
        [Test]
        public void Write_BelowMinimumSeverity_IsIgnored()
        {
            MutantLogService serviceInstance = new MutantLogService(MutantLogSeverity.Warning);
            MutantInMemoryLogSink memorySinkInstance = new MutantInMemoryLogSink(32);

            serviceInstance.AddSink(memorySinkInstance);
            serviceInstance.Write(MutantLogSeverity.Info, "Test", "ignored");

            Assert.AreEqual(0, memorySinkInstance.GetSnapshot().Count);

            serviceInstance.Dispose();
        }

        [Test]
        public void Write_AtOrAboveMinimumSeverity_IsBuffered()
        {
            MutantLogService serviceInstance = new MutantLogService(MutantLogSeverity.Info);
            MutantInMemoryLogSink memorySinkInstance = new MutantInMemoryLogSink(32);

            serviceInstance.AddSink(memorySinkInstance);
            serviceInstance.Write(MutantLogSeverity.Warning, "Test", "accepted");

            Assert.AreEqual(1, memorySinkInstance.GetSnapshot().Count);

            serviceInstance.Dispose();
        }

        [Test]
        public void InMemorySink_RespectsCapacity()
        {
            MutantInMemoryLogSink memorySinkInstance = new MutantInMemoryLogSink(2);

            memorySinkInstance.Write(new MutantLogRecord(
                System.DateTime.UtcNow,
                MutantLogSeverity.Info,
                "Test",
                "A",
                null,
                0,
                1));

            memorySinkInstance.Write(new MutantLogRecord(
                System.DateTime.UtcNow,
                MutantLogSeverity.Info,
                "Test",
                "B",
                null,
                0,
                1));

            memorySinkInstance.Write(new MutantLogRecord(
                System.DateTime.UtcNow,
                MutantLogSeverity.Info,
                "Test",
                "C",
                null,
                0,
                1));

            Assert.AreEqual(2, memorySinkInstance.GetSnapshot().Count);
            Assert.AreEqual("B", memorySinkInstance.GetSnapshot()[0].MessageText);
            Assert.AreEqual("C", memorySinkInstance.GetSnapshot()[1].MessageText);
        }
    }
}