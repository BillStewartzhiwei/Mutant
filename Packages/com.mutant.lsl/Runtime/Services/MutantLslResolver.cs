using System;
using LSL;

namespace Mutant.LSL
{
    public sealed class MutantLslResolver : IMutantLslResolver
    {
        public MutantLslResolvedStreamInfo[] Resolve(MutantLslResolverConfig config)
        {
             StreamInfo[] nativeResults = ResolveNative(config);
            if (nativeResults == null || nativeResults.Length == 0)
            {
                return Array.Empty<MutantLslResolvedStreamInfo>();
            }

            MutantLslResolvedStreamInfo[] results = new MutantLslResolvedStreamInfo[nativeResults.Length];
            for (int index = 0; index < nativeResults.Length; index++)
            {
                results[index] = MutantLslResolvedStreamInfo.FromNative(nativeResults[index]);
                nativeResults[index]?.Dispose();
            }

            return results;
        }

        internal  StreamInfo ResolveFirstNative(MutantLslResolverConfig config)
        {
             StreamInfo[] nativeResults = ResolveNative(config);
            if (nativeResults == null || nativeResults.Length == 0)
            {
                throw new MutantLslException("No matching LSL stream was found.");
            }

            for (int index = 1; index < nativeResults.Length; index++)
            {
                nativeResults[index]?.Dispose();
            }

            return nativeResults[0];
        }

        private  StreamInfo[] ResolveNative(MutantLslResolverConfig config)
        {
            if (config == null)
            {
                throw new MutantLslException("Resolver config is null.");
            }

            try
            {
                if (config.resolveMode == MutantLslResolveMode.ByPredicate)
                {
                    if (string.IsNullOrWhiteSpace(config.predicate))
                    {
                        throw new MutantLslException("Resolver predicate is empty.");
                    }

                    return global::LSL.LSL.resolve_stream(
                        config.predicate,
                        config.minimumResultCount,
                        config.timeoutSeconds);
                }

                if (string.IsNullOrWhiteSpace(config.propertyName))
                {
                    throw new MutantLslException("Resolver propertyName is empty.");
                }

                if (string.IsNullOrWhiteSpace(config.propertyValue))
                {
                    throw new MutantLslException("Resolver propertyValue is empty.");
                }

                return  global::LSL.LSL.resolve_stream(
                    config.propertyName,
                    config.propertyValue,
                    config.minimumResultCount,
                    config.timeoutSeconds);
            }
            catch (Exception exception)
            {
                throw new MutantLslException("Failed to resolve LSL streams.", exception);
            }
        }
    }
}