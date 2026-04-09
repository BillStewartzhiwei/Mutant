using LSL;

namespace Mutant.LSL
{
    public static class MutantLslMetadataBuilder
    {
        public static  StreamInfo BuildStreamInfo(MutantLslStreamDefinition streamDefinition, MutantLslSenderConfig senderConfig)
        {
            ValidateStreamDefinition(streamDefinition);

            string finalStreamName = string.IsNullOrWhiteSpace(senderConfig.streamNameOverride)
                ? streamDefinition.streamName
                : senderConfig.streamNameOverride;

            string finalStreamType = string.IsNullOrWhiteSpace(senderConfig.streamTypeOverride)
                ? streamDefinition.streamType
                : senderConfig.streamTypeOverride;

            double finalNominalRate = senderConfig.nominalRateOverrideHz >= 0.0
                ? senderConfig.nominalRateOverrideHz
                : streamDefinition.nominalRateHz;

            string finalSourceId = string.IsNullOrWhiteSpace(senderConfig.sourceId)
                ? "mutant:com.mutant.lsl:main:default"
                : senderConfig.sourceId;

             StreamInfo streamInfo = new  StreamInfo(
                finalStreamName,
                finalStreamType,
                streamDefinition.ChannelCount,
                finalNominalRate,
                ToNativeChannelFormat(streamDefinition.channelFormat),
                finalSourceId);

             XMLElement description = streamInfo.desc();

            description.append_child_value(MutantLslConstants.MetadataProfileId, streamDefinition.profileId);
            description.append_child_value(MutantLslConstants.MetadataProfileVersion, streamDefinition.profileVersion);
            description.append_child_value(MutantLslConstants.MetadataPackageId, string.IsNullOrWhiteSpace(senderConfig.producerPackageId)
                ? MutantLslConstants.DefaultProducerPackageId
                : senderConfig.producerPackageId);
            description.append_child_value(MutantLslConstants.MetadataProducerId, string.IsNullOrWhiteSpace(senderConfig.producerId)
                ? MutantLslConstants.DefaultProducerId
                : senderConfig.producerId);

            AppendIfHasValue(description, MutantLslConstants.MetadataSessionId, senderConfig.sessionId);
            AppendIfHasValue(description, MutantLslConstants.MetadataSubjectId, senderConfig.subjectId);
            AppendIfHasValue(description, MutantLslConstants.MetadataRole, senderConfig.role);
            AppendIfHasValue(description, MutantLslConstants.MetadataPayloadEncoding, senderConfig.payloadEncoding);
            AppendIfHasValue(description, MutantLslConstants.MetadataCoordinateSpace, senderConfig.coordinateSpace);
            AppendIfHasValue(description, MutantLslConstants.MetadataUnits, senderConfig.unitsOverride);

            AppendChannels(description, streamDefinition.channels);
            AppendMutantNode(description, streamDefinition, senderConfig);

            return streamInfo;
        }

        private static  channel_format_t ToNativeChannelFormat(MutantLslChannelFormat channelFormat)
        {
            switch (channelFormat)
            {
                case MutantLslChannelFormat.String:
                    return  channel_format_t.cf_string;
                case MutantLslChannelFormat.Float32:
                    return  channel_format_t.cf_float32;
                case MutantLslChannelFormat.Double64:
                    return  channel_format_t.cf_double64;
                default:
                    throw new MutantLslException($"Unsupported channel format: {channelFormat}");
            }
        }

        private static void AppendChannels( XMLElement description, MutantLslChannelDefinition[] channels)
        {
             XMLElement channelsNode = description.append_child("channels");

            for (int index = 0; index < channels.Length; index++)
            {
                MutantLslChannelDefinition channel = channels[index];
                 XMLElement channelNode = channelsNode.append_child("channel");

                channelNode.append_child_value("label", channel.label);
                AppendIfHasValue(channelNode, "unit", channel.unit);
                AppendIfHasValue(channelNode, "type", channel.semantic);
                channelNode.append_child_value("index", index.ToString());
            }
        }

        private static void AppendMutantNode( XMLElement description, MutantLslStreamDefinition streamDefinition, MutantLslSenderConfig senderConfig)
        {
             XMLElement mutantNode = description.append_child("mutant");

            mutantNode.append_child_value("profile_id", streamDefinition.profileId);
            mutantNode.append_child_value("profile_version", streamDefinition.profileVersion);
            mutantNode.append_child_value("package_id", string.IsNullOrWhiteSpace(senderConfig.producerPackageId)
                ? MutantLslConstants.DefaultProducerPackageId
                : senderConfig.producerPackageId);
            mutantNode.append_child_value("producer_id", string.IsNullOrWhiteSpace(senderConfig.producerId)
                ? MutantLslConstants.DefaultProducerId
                : senderConfig.producerId);

            AppendIfHasValue(mutantNode, "session_id", senderConfig.sessionId);
            AppendIfHasValue(mutantNode, "subject_id", senderConfig.subjectId);
            AppendIfHasValue(mutantNode, "role", senderConfig.role);
            AppendIfHasValue(mutantNode, "payload_encoding", senderConfig.payloadEncoding);
            AppendIfHasValue(mutantNode, "coordinate_space", senderConfig.coordinateSpace);
            AppendIfHasValue(mutantNode, "units", senderConfig.unitsOverride);

            if (streamDefinition.metadataEntries != null)
            {
                for (int index = 0; index < streamDefinition.metadataEntries.Length; index++)
                {
                    MutantLslMetadataEntry entry = streamDefinition.metadataEntries[index];
                    if (!string.IsNullOrWhiteSpace(entry.key) && !string.IsNullOrWhiteSpace(entry.value))
                    {
                        mutantNode.append_child_value(entry.key, entry.value);
                    }
                }
            }

            if (senderConfig.extraMetadata != null)
            {
                for (int index = 0; index < senderConfig.extraMetadata.Length; index++)
                {
                    MutantLslMetadataEntry entry = senderConfig.extraMetadata[index];
                    if (!string.IsNullOrWhiteSpace(entry.key) && !string.IsNullOrWhiteSpace(entry.value))
                    {
                        mutantNode.append_child_value(entry.key, entry.value);
                    }
                }
            }
        }

        private static void AppendIfHasValue( XMLElement element, string childName, string childValue)
        {
            if (!string.IsNullOrWhiteSpace(childValue))
            {
                element.append_child_value(childName, childValue);
            }
        }

        private static void ValidateStreamDefinition(MutantLslStreamDefinition streamDefinition)
        {
            if (streamDefinition == null)
            {
                throw new MutantLslException("Stream definition is null.");
            }

            if (string.IsNullOrWhiteSpace(streamDefinition.streamName))
            {
                throw new MutantLslException("Stream definition streamName is empty.");
            }

            if (string.IsNullOrWhiteSpace(streamDefinition.streamType))
            {
                throw new MutantLslException("Stream definition streamType is empty.");
            }

            if (string.IsNullOrWhiteSpace(streamDefinition.profileId))
            {
                throw new MutantLslException("Stream definition profileId is empty.");
            }

            if (streamDefinition.channels == null || streamDefinition.channels.Length == 0)
            {
                throw new MutantLslException("Stream definition must contain at least one channel.");
            }
        }
    }
}