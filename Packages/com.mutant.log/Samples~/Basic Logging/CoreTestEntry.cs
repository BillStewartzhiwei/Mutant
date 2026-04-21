using UnityEngine;
using Mutant.Log.API;
using Mutant.Log.Modules;

namespace Mutant.Log.Samples.BasicLogging
{
    public sealed class LoggingSampleEntry : MonoBehaviour
    {
        private void Start()
        {
            MutantLogger.Info(MutantLogCategories.Log, "Basic logging sample started.");
            MutantLogger.Warning(MutantLogCategories.Log, "This is a warning record.");
            MutantLogger.Error(MutantLogCategories.Log, "This is an error record.");

            var bufferedRecords = MutantLogModule.GetBufferedRecordSnapshot();
            Debug.Log("[LoggingSampleEntry] Buffered record count = " + bufferedRecords.Count);

            if (bufferedRecords.Count > 0)
            {
                var lastRecord = bufferedRecords[bufferedRecords.Count - 1];
                Debug.Log(
                    "[LoggingSampleEntry] Last buffered record = " +
                    $"[{lastRecord.Severity}] [{lastRecord.CategoryText}] {lastRecord.MessageText}");
            }
        }
    }
}