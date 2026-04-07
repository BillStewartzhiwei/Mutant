using UnityEngine;
using Mutant.Log.API;
using Mutant.Log.Modules;

namespace Mutant.Log.Samples.BasicLogging
{
	public class LoggingSampleEntry : MonoBehaviour
	{
		private void Start()
		{
			MutantLogger.Info("Sample", "Basic logging sample started.");
			MutantLogger.Warning("Sample", "This is a warning record.");
			MutantLogger.Error("Sample", "This is an error record.");

			var bufferedRecords = MutantLogModule.GetBufferedRecordSnapshot();
			Debug.Log("[LoggingSampleEntry] Buffered record count = " + bufferedRecords.Count);
		}
	}
}
