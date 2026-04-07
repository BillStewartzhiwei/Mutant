using System;
using System.IO;
using System.Text;
using UnityEngine;
using Mutant.Log.Models;

namespace Mutant.Log.Sinks
{
    public sealed class MutantFileLogSink : IMutantLogSink
    {
        private readonly object _writeGuard = new object();
        private readonly StreamWriter _streamWriter;
        private readonly bool _flushAfterEachWrite;

        public string SinkDisplayName => "File";
        public string AbsoluteFilePath { get; }

        public MutantFileLogSink(
            string outputFolderName,
            string fileNamePrefix,
            bool appendDateToFileName,
            bool flushAfterEachWrite)
        {
            _flushAfterEachWrite = flushAfterEachWrite;

            string sanitizedFolderName = string.IsNullOrWhiteSpace(outputFolderName) ? "MutantLogs" : outputFolderName;
            string sanitizedFilePrefix = string.IsNullOrWhiteSpace(fileNamePrefix) ? "mutant-log" : fileNamePrefix;

            string folderPath = Path.Combine(Application.persistentDataPath, sanitizedFolderName);
            Directory.CreateDirectory(folderPath);

            string fileName = appendDateToFileName
                ? $"{SanitizeFileSegment(sanitizedFilePrefix)}-{DateTime.UtcNow:yyyyMMdd-HHmmss}.log"
                : $"{SanitizeFileSegment(sanitizedFilePrefix)}.log";

            AbsoluteFilePath = Path.Combine(folderPath, fileName);

            FileStream fileStream = new FileStream(
                AbsoluteFilePath,
                FileMode.Append,
                FileAccess.Write,
                FileShare.Read);

            _streamWriter = new StreamWriter(fileStream, new UTF8Encoding(false));
        }

        public void Write(MutantLogRecord logRecord)
        {
            if (logRecord == null)
                return;

            lock (_writeGuard)
            {
                _streamWriter.WriteLine(FormatLine(logRecord));

                if (_flushAfterEachWrite)
                    _streamWriter.Flush();
            }
        }

        public void Flush()
        {
            lock (_writeGuard)
            {
                _streamWriter.Flush();
            }
        }

        public void Dispose()
        {
            lock (_writeGuard)
            {
                _streamWriter.Flush();
                _streamWriter.Dispose();
            }
        }

        private static string FormatLine(MutantLogRecord logRecord)
        {
            string messageText = EscapeForSingleLine(logRecord.MessageText);
            string exceptionText = EscapeForSingleLine(logRecord.ExceptionText);

            return
                $"{logRecord.TimestampUtc:O}|{logRecord.Severity}|{logRecord.CategoryText}|frame={logRecord.FrameIndex}|thread={logRecord.ManagedThreadId}|message={messageText}|exception={exceptionText}";
        }

        private static string EscapeForSingleLine(string sourceText)
        {
            if (string.IsNullOrEmpty(sourceText))
                return string.Empty;

            return sourceText
                .Replace("\r", "\\r")
                .Replace("\n", "\\n");
        }

        private static string SanitizeFileSegment(string sourceText)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            StringBuilder builder = new StringBuilder(sourceText.Length);

            for (int i = 0; i < sourceText.Length; i++)
            {
                char currentChar = sourceText[i];
                bool isInvalid = false;

                for (int j = 0; j < invalidChars.Length; j++)
                {
                    if (currentChar == invalidChars[j])
                    {
                        isInvalid = true;
                        break;
                    }
                }

                builder.Append(isInvalid ? '_' : currentChar);
            }

            return builder.ToString();
        }
    }
}