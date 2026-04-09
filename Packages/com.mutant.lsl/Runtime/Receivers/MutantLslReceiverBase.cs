using System;
using LSL;

namespace Mutant.LSL
{
    public abstract class MutantLslReceiverBase : IMutantLslReceiver
    {
        private readonly double _defaultPullTimeoutSeconds;
        private bool _isDisposed;

        protected readonly StreamInfo NativeResolvedInfo;
        protected readonly StreamInlet NativeStreamInlet;

        public string StreamName { get; }
        public string SourceId { get; }
        public int ChannelCount { get; }
        public MutantLslConnectionState State { get; protected set; }

        public virtual bool SupportsString => false;
        public virtual bool SupportsFloat => false;

        protected MutantLslReceiverBase( StreamInfo resolvedInfo, MutantLslReceiverConfig receiverConfig)
        {
            if (resolvedInfo == null)
            {
                throw new MutantLslException("Resolved stream info is null.");
            }

            if (receiverConfig == null)
            {
                throw new MutantLslException("Receiver config is null.");
            }

            try
            {
                State = MutantLslConnectionState.Resolving;
                NativeResolvedInfo = resolvedInfo;
                NativeStreamInlet = new StreamInlet(
                    resolvedInfo,
                    receiverConfig.maxBufferLengthSeconds,
                    receiverConfig.maxChunkLengthSamples,
                    receiverConfig.recover);

                if (receiverConfig.openStreamOnCreate)
                {
                    NativeStreamInlet.open_stream(receiverConfig.resolver.timeoutSeconds);
                }

                StreamName = resolvedInfo.name();
                SourceId = resolvedInfo.source_id();
                ChannelCount = resolvedInfo.channel_count();
                State = MutantLslConnectionState.Connected;
                _defaultPullTimeoutSeconds = receiverConfig.defaultPullTimeoutSeconds;
            }
            catch (Exception exception)
            {
                State = MutantLslConnectionState.Faulted;
                throw new MutantLslException("Failed to create LSL receiver.", exception);
            }
        }

        public virtual bool TryPullString(string[] reusableBuffer, out double timestampSeconds, double timeoutSeconds = 0.0)
        {
            timestampSeconds = 0.0;
            throw new MutantLslException($"{GetType().Name} does not support string samples.");
        }

        public virtual bool TryPullFloat(float[] reusableBuffer, out double timestampSeconds, double timeoutSeconds = 0.0)
        {
            timestampSeconds = 0.0;
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

            try
            {
                NativeStreamInlet?.close_stream();
            }
            catch
            {
            }

            NativeStreamInlet?.Dispose();
            NativeResolvedInfo?.Dispose();
        }

        protected double ResolvePullTimeout(double timeoutSeconds)
        {
            return timeoutSeconds > 0.0 ? timeoutSeconds : _defaultPullTimeoutSeconds;
        }

        protected void ValidateBufferLength(int bufferLength)
        {
            if (bufferLength != ChannelCount)
            {
                throw new MutantLslException($"Receiver buffer length mismatch. Expected {ChannelCount}, got {bufferLength}.");
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