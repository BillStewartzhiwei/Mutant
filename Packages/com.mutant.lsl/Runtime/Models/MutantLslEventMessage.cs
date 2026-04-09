using System;
using System.Text;

namespace Mutant.LSL
{
	[Serializable]
	public sealed class MutantLslEventMessage
	{
		public string schema = MutantLslConstants.ProfileIdEvent;
		public string schemaVersion = MutantLslConstants.DefaultProfileVersion;
		public long sequenceId;
		public string eventKey;
		public string eventLabel;
		public string producer;
		public string sessionId;
		public string sentAtUtc;
		public string contextJson;

		public string ToJson()
		{
			StringBuilder builder = new StringBuilder(256);
			bool hasPrevious = false;

			MutantLslJsonWriter.BeginObject(builder);
			MutantLslJsonWriter.AppendStringProperty(builder, "schema", schema, ref hasPrevious);
			MutantLslJsonWriter.AppendStringProperty(builder, "schemaVersion", schemaVersion, ref hasPrevious);
			MutantLslJsonWriter.AppendInt64Property(builder, "sequenceId", sequenceId, ref hasPrevious);
			MutantLslJsonWriter.AppendStringProperty(builder, "eventKey", eventKey, ref hasPrevious);
			MutantLslJsonWriter.AppendStringProperty(builder, "eventLabel", eventLabel, ref hasPrevious);
			MutantLslJsonWriter.AppendStringProperty(builder, "producer", producer, ref hasPrevious);
			MutantLslJsonWriter.AppendStringProperty(builder, "sessionId", sessionId, ref hasPrevious);
			MutantLslJsonWriter.AppendStringProperty(builder, "sentAtUtc", sentAtUtc, ref hasPrevious);
			MutantLslJsonWriter.AppendRawJsonProperty(builder, "context", string.IsNullOrWhiteSpace(contextJson) ? "{}" : contextJson, ref hasPrevious);
			MutantLslJsonWriter.EndObject(builder);

			return builder.ToString();
		}
	}
}
