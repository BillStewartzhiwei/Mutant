namespace Mutant.LSL
{
    public static class MutantLslDefinitionCatalog
    {
        public static MutantLslStreamDefinition CreateMarkerDefinition()
        {
            return new MutantLslStreamDefinition
            {
                streamName = MutantLslConstants.StreamNameMarker,
                streamType = MutantLslConstants.StreamTypeEvent,
                profileId = MutantLslConstants.ProfileIdMarker,
                channelFormat = MutantLslChannelFormat.String,
                nominalRateHz = MutantLslConstants.IrregularRate,
                channels = new[]
                {
                    new MutantLslChannelDefinition("payload", "", "json")
                },
                metadataEntries = new[]
                {
                    new MutantLslMetadataEntry(MutantLslConstants.MetadataPayloadEncoding, MutantLslConstants.DefaultPayloadEncoding)
                }
            };
        }

        public static MutantLslStreamDefinition CreateEventDefinition()
        {
            return new MutantLslStreamDefinition
            {
                streamName = MutantLslConstants.StreamNameEvent,
                streamType = MutantLslConstants.StreamTypeEvent,
                profileId = MutantLslConstants.ProfileIdEvent,
                channelFormat = MutantLslChannelFormat.String,
                nominalRateHz = MutantLslConstants.IrregularRate,
                channels = new[]
                {
                    new MutantLslChannelDefinition("payload", "", "json")
                },
                metadataEntries = new[]
                {
                    new MutantLslMetadataEntry(MutantLslConstants.MetadataPayloadEncoding, MutantLslConstants.DefaultPayloadEncoding)
                }
            };
        }

        public static MutantLslStreamDefinition CreateStateDefinition()
        {
            return new MutantLslStreamDefinition
            {
                streamName = MutantLslConstants.StreamNameState,
                streamType = MutantLslConstants.StreamTypeEvent,
                profileId = MutantLslConstants.ProfileIdState,
                channelFormat = MutantLslChannelFormat.String,
                nominalRateHz = MutantLslConstants.IrregularRate,
                channels = new[]
                {
                    new MutantLslChannelDefinition("payload", "", "json")
                },
                metadataEntries = new[]
                {
                    new MutantLslMetadataEntry(MutantLslConstants.MetadataPayloadEncoding, MutantLslConstants.DefaultPayloadEncoding)
                }
            };
        }

        public static MutantLslStreamDefinition CreateNumericDefinition(string streamName, string[] channelLabels, double nominalRateHz, string units = "")
        {
            if (channelLabels == null || channelLabels.Length == 0)
            {
                throw new MutantLslException("Mutant.Numeric requires at least one channel label.");
            }

            MutantLslChannelDefinition[] channels = new MutantLslChannelDefinition[channelLabels.Length];
            for (int index = 0; index < channelLabels.Length; index++)
            {
                channels[index] = new MutantLslChannelDefinition(channelLabels[index], units, channelLabels[index]);
            }

            return new MutantLslStreamDefinition
            {
                streamName = string.IsNullOrWhiteSpace(streamName) ? MutantLslConstants.StreamNameNumeric : streamName,
                streamType = MutantLslConstants.StreamTypeSignal,
                profileId = MutantLslConstants.ProfileIdNumeric,
                channelFormat = MutantLslChannelFormat.Float32,
                nominalRateHz = nominalRateHz,
                channels = channels,
                metadataEntries = string.IsNullOrWhiteSpace(units)
                    ? null
                    : new[]
                    {
                        new MutantLslMetadataEntry(MutantLslConstants.MetadataUnits, units)
                    }
            };
        }

        public static MutantLslStreamDefinition CreatePoseDefinition(string streamName = null, double nominalRateHz = 90.0, string coordinateSpace = "world")
        {
            return new MutantLslStreamDefinition
            {
                streamName = string.IsNullOrWhiteSpace(streamName) ? MutantLslConstants.StreamNamePose : streamName,
                streamType = MutantLslConstants.StreamTypePose,
                profileId = MutantLslConstants.ProfileIdPose,
                channelFormat = MutantLslChannelFormat.Float32,
                nominalRateHz = nominalRateHz,
                channels = new[]
                {
                    new MutantLslChannelDefinition("px", "meters", "position_x"),
                    new MutantLslChannelDefinition("py", "meters", "position_y"),
                    new MutantLslChannelDefinition("pz", "meters", "position_z"),
                    new MutantLslChannelDefinition("qx", "", "rotation_quaternion_x"),
                    new MutantLslChannelDefinition("qy", "", "rotation_quaternion_y"),
                    new MutantLslChannelDefinition("qz", "", "rotation_quaternion_z"),
                    new MutantLslChannelDefinition("qw", "", "rotation_quaternion_w")
                },
                metadataEntries = new[]
                {
                    new MutantLslMetadataEntry(MutantLslConstants.MetadataCoordinateSpace, coordinateSpace),
                    new MutantLslMetadataEntry(MutantLslConstants.MetadataUnits, "meters")
                }
            };
        }
    }
}