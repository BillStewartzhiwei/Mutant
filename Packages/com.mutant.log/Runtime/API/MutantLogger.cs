using System;
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

        public static void Exception(string categoryText, Exception exceptionValue, string messageText = null)
        {
            string resolvedMessage =
                string.IsNullOrWhiteSpace(messageText)
                    ? exceptionValue?.Message ?? "Exception"
                    : messageText;

            WriteInternal(
                MutantLogSeverity.Error,
                categoryText,
                resolvedMessage,
                exceptionValue?.ToString());
        }

        public static void Log(MutantLogSeverity severity, string categoryText, string messageText)
        {
            WriteInternal(severity, categoryText, messageText, null);
        }

        public static void Trace(string messageText)
        {
            Trace(MutantLogCategories.General, messageText);
        }

        public static void Info(string messageText)
        {
            Info(MutantLogCategories.General, messageText);
        }

        public static void Warning(string messageText)
        {
            Warning(MutantLogCategories.General, messageText);
        }

        public static void Error(string messageText)
        {
            Error(MutantLogCategories.General, messageText);
        }

        public static void Fatal(string messageText)
        {
            Fatal(MutantLogCategories.General, messageText);
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

            string resolvedCategory =
                string.IsNullOrWhiteSpace(categoryText)
                    ? MutantLogCategories.General
                    : categoryText;

            string resolvedMessage = messageText ?? string.Empty;
            string fallbackText = $"[{severity}] [{resolvedCategory}] {resolvedMessage}";

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