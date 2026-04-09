namespace Mutant.LSL
{
    public static class MutantLslConstants
    {
        public const string DefaultProfileVersion = "1.0.0";

        public const string StreamNameMarker = "Mutant.Marker";
        public const string StreamNameEvent = "Mutant.Event";
        public const string StreamNameState = "Mutant.State";
        public const string StreamNameNumeric = "Mutant.Numeric";
        public const string StreamNamePose = "Mutant.Pose";

        public const string StreamTypeEvent = "Mutant.Event";
        public const string StreamTypeSignal = "Mutant.Signal";
        public const string StreamTypePose = "Mutant.Pose";

        public const string ProfileIdMarker = "mutant.marker.v1";
        public const string ProfileIdEvent = "mutant.event.v1";
        public const string ProfileIdState = "mutant.state.v1";
        public const string ProfileIdNumeric = "mutant.numeric.v1";
        public const string ProfileIdPose = "mutant.pose.v1";

        public const string MetadataProfileId = "mutant_profile_id";
        public const string MetadataProfileVersion = "mutant_profile_version";
        public const string MetadataPackageId = "mutant_package_id";
        public const string MetadataProducerId = "mutant_producer_id";
        public const string MetadataSessionId = "mutant_session_id";
        public const string MetadataSubjectId = "mutant_subject_id";
        public const string MetadataRole = "mutant_role";
        public const string MetadataUnits = "mutant_units";
        public const string MetadataCoordinateSpace = "mutant_coordinate_space";
        public const string MetadataPayloadEncoding = "mutant_payload_encoding";

        public const string DefaultProducerPackageId = "com.mutant.lsl";
        public const string DefaultProducerId = "com.mutant.lsl";
        public const string DefaultPayloadEncoding = "json";

        public const double IrregularRate = 0.0;
        public const double DefaultResolveTimeoutSeconds = 1.0;
        public const double DefaultPullTimeoutSeconds = 0.0;

        public const int DefaultMaxBufferedSeconds = 360;
        public const int DefaultMaxBufferLengthSeconds = 360;
        public const int DefaultMaxChunkLengthSamples = 0;
    }
}