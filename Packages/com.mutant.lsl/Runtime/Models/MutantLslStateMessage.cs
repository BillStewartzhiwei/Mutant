using System;
using System.Text;

namespace Mutant.LSL
{
	[Serializable]
	public sealed class MutantLslStateMessage
	{
		public string schema = MutantLslConstants.ProfileIdState;
		public string schemaVersion = MutantLslConstants.DefaultProfileVersion;
		public long sequenceId;
		public string stateKey;
		public string stateValue;
		public string previousValue;
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
			MutantLslJsonWriter.AppendStringProperty(builder, "stateKey", stateKey, ref hasPrevious);
			MutantLslJsonWriter.AppendStringProperty(builder, "stateValue", stateValue, ref hasPrevious);
			MutantLslJsonWriter.AppendStringProperty(builder, "previousValue", previousValue, ref hasPrevious);
			MutantLslJsonWriter.AppendStringProperty(builder, "producer", producer, ref hasPrevious);
			MutantLslJsonWriter.AppendStringProperty(builder, "sessionId", sessionId, ref hasPrevious);
			MutantLslJsonWriter.AppendStringProperty(builder, "sentAtUtc", sentAtUtc, ref hasPrevious);
			MutantLslJsonWriter.AppendRawJsonProperty(builder, "context", string.IsNullOrWhiteSpace(contextJson) ? "{}" : contextJson, ref hasPrevious);
			MutantLslJsonWriter.EndObject(builder);

			return builder.ToString();
		}
	}
}
