using UnityEngine;
using Mutant.Log.Models;
using Mutant.Log.Modules;

namespace Mutant.Log.API
{
    public static class MutantLogger
    {
        public static void Trace(string categoryText, string messageText)
        {
            WriteInternal(MutantLogSeverity.Trace, categoryText, messageText, null);
        }

        public static void Info(string categoryText, string messageText)
        {
            WriteInternal(MutantLogSeverity.Info, categoryText, messageText, null);
        }

        public static void Warning(string categoryText, string messageText)
        {
            WriteInternal(MutantLogSeverity.Warning, categoryText, messageText, null);
        }

        public static void Error(string categoryText, string messageText)
        {
            WriteInternal(MutantLogSeverity.Error, categoryText, messageText, null);
        }

        public static void Fatal(string categoryText, string messageText)
        {
            WriteInternal(MutantLogSeverity.Fatal, categoryText, messageText, null);
        }

        public static void ErrorWithException(string categoryText, string messageText, System.Exception exceptionValue)
        {
            WriteInternal(
                MutantLogSeverity.Error,
                categoryText,
                messageText,
                exceptionValue == null ? null : exceptionValue.ToString());
        }

        public static void Info(string messageText)
        {
            WriteInternal(MutantLogSeverity.Info, "General", messageText, null);
        }

        public static void Warning(string messageText)
        {
            WriteInternal(MutantLogSeverity.Warning, "General", messageText, null);
        }

        public static void Error(string messageText)
        {
            WriteInternal(MutantLogSeverity.Error, "General", messageText, null);
        }

        private static void WriteInternal(
            MutantLogSeverity severity,
            string categoryText,
            string messageText,
            string exceptionText)
        {
            if (MutantLogModule.ActiveService != null)
            {
                MutantLogModule.ActiveService.Write(severity, categoryText, messageText, exceptionText);
                return;
            }

            string fallbackText =
                $"[{severity}] [{(string.IsNullOrWhiteSpace(categoryText) ? "General" : categoryText)}] {messageText}";

            if (!string.IsNullOrEmpty(exceptionText))
                fallbackText += "\n" + exceptionText;

            switch (severity)
            {
                case MutantLogSeverity.Trace:
                case MutantLogSeverity.Info:
                    Debug.Log(fallbackText);
                    break;

                case MutantLogSeverity.Warning:
                    Debug.LogWarning(fallbackText);
                    break;

                case MutantLogSeverity.Error:
                case MutantLogSeverity.Fatal:
                    Debug.LogError(fallbackText);
                    break;
            }
        }
    }
}