using System;
using LSL;

namespace Mutant.LSL
{
    public abstract class MutantLslSenderBase : IMutantLslSender
    {
        private readonly IMutantLslClock _clock;
        private readonly bool _useLocalClockIfTimestampMissing;
        private bool _isDisposed;

        protected readonly  StreamInfo NativeStreamInfo;
        protected readonly  StreamOutlet NativeStreamOutlet;

        public string StreamName { get; }
        public string SourceId { get; }
        public int ChannelCount { get; }
        public MutantLslConnectionState State { get; protected set; }

        public virtual bool SupportsString => false;
        public virtual bool SupportsFloat => false;

        protected MutantLslSenderBase(MutantLslStreamDefinition streamDefinition, MutantLslSenderConfig senderConfig, IMutantLslClock clock)
        {
            if (streamDefinition == null)
            {
                throw new MutantLslException("Stream definition is null.");
            }

            if (senderConfig == null)
            {
                throw new MutantLslException("Sender config is null.");
            }

            _clock = clock ?? new MutantLslClock();
            _useLocalClockIfTimestampMissing = senderConfig.useLocalClockIfTimestampMissing;

            try
            {
                NativeStreamInfo = MutantLslMetadataBuilder.BuildStreamInfo(streamDefinition, senderConfig);
                NativeStreamOutlet = new  StreamOutlet(
                    NativeStreamInfo,
                    senderConfig.chunkSizeSamples,
                    senderConfig.maxBufferedSeconds);

                StreamName = NativeStreamInfo.name();
                SourceId = NativeStreamInfo.source_id();
                ChannelCount = NativeStreamInfo.channel_count();
                State = MutantLslConnectionState.Connected;
            }
            catch (Exception exception)
            {
                State = MutantLslConnectionState.Faulted;
                throw new MutantLslException("Failed to create LSL sender.", exception);
            }
        }

        public virtual void SendString(string[] channelValues, double? timestampSeconds = null)
        {
            throw new MutantLslException($"{GetType().Name} does not support string samples.");
        }

        public virtual void SendFloat(float[] channelValues, double? timestampSeconds = null)
        {
            throw new MutantLslException($"{GetType().Name} does not support float samples.");
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            State = MutantLslConnectionState.Disposed;

            NativeStreamOutlet?.Dispose();
            NativeStreamInfo?.Dispose();
        }

        protected double ResolveTimestamp(double? timestampSeconds)
        {
            if (timestampSeconds.HasValue)
            {
                return timestampSeconds.Value;
            }

            return _useLocalClockIfTimestampMissing ? _clock.Now() : 0.0;
        }

        protected void ValidateSampleLength(int sampleLength)
        {
            if (sampleLength != ChannelCount)
            {
                throw new MutantLslException($"Sample length mismatch. Expected {ChannelCount}, got {sampleLength}.");
            }
        }

        protected void ThrowIfDisposed()
        {
            if (_isDisposed)
            {
                throw new MutantLslException($"{GetType().Name} has already been disposed.");
            }
        }
    }
}