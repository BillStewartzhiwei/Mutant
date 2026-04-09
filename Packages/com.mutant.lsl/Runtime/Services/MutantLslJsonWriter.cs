using System.Globalization;
using System.Text;

namespace Mutant.LSL
{
    internal static class MutantLslJsonWriter
    {
        public static void BeginObject(StringBuilder builder)
        {
            builder.Append('{');
        }

        public static void EndObject(StringBuilder builder)
        {
            builder.Append('}');
        }

        public static void AppendStringProperty(StringBuilder builder, string propertyName, string propertyValue, ref bool hasPrevious)
        {
            if (string.IsNullOrEmpty(propertyValue))
            {
                return;
            }

            AppendCommaIfNeeded(builder, ref hasPrevious);
            builder.Append('\"').Append(Escape(propertyName)).Append("\":");
            builder.Append('\"').Append(Escape(propertyValue)).Append('\"');
        }

        public static void AppendInt64Property(StringBuilder builder, string propertyName, long propertyValue, ref bool hasPrevious)
        {
            AppendCommaIfNeeded(builder, ref hasPrevious);
            builder.Append('\"').Append(Escape(propertyName)).Append("\":");
            builder.Append(propertyValue.ToString(CultureInfo.InvariantCulture));
        }

        public static void AppendRawJsonProperty(StringBuilder builder, string propertyName, string rawJson, ref bool hasPrevious)
        {
            AppendCommaIfNeeded(builder, ref hasPrevious);
            builder.Append('\"').Append(Escape(propertyName)).Append("\":");
            builder.Append(string.IsNullOrWhiteSpace(rawJson) ? "{}" : rawJson);
        }

        private static void AppendCommaIfNeeded(StringBuilder builder, ref bool hasPrevious)
        {
            if (hasPrevious)
            {
                builder.Append(',');
            }

            hasPrevious = true;
        }

        private static string Escape(string rawText)
        {
            if (string.IsNullOrEmpty(rawText))
            {
                return string.Empty;
            }

            StringBuilder builder = new StringBuilder(rawText.Length + 16);

            for (int index = 0; index < rawText.Length; index++)
            {
                char currentChar = rawText[index];
                switch (currentChar)
                {
                    case '\"':
                        builder.Append("\\\"");
                        break;
                    case '\\':
                        builder.Append("\\\\");
                        break;
                    case '\b':
                        builder.Append("\\b");
                        break;
                    case '\f':
                        builder.Append("\\f");
                        break;
                    case '\n':
                        builder.Append("\\n");
                        break;
                    case '\r':
                        builder.Append("\\r");
                        break;
                    case '\t':
                        builder.Append("\\t");
                        break;
                    default:
                        if (currentChar < 32)
                        {
                            builder.Append("\\u");
                            builder.Append(((int)currentChar).ToString("x4"));
                        }
                        else
                        {
                            builder.Append(currentChar);
                        }
                        break;
                }
            }

            return builder.ToString();
        }
    }
}