using UnityEngine;
using Mutant.Log.Models;

namespace Mutant.Log.Config
{
	[CreateAssetMenu(menuName = "Mutant/Log/Runtime Settings", fileName = "MutantLogRuntimeSettings")]
	public sealed class MutantLogRuntimeSettings : ScriptableObject
	{
		[Header("Filtering")]
		public MutantLogSeverity minimumSeverity = MutantLogSeverity.Info;

		[Header("Sinks")]
		public bool enableUnityConsoleSink = true;
		public bool enableInMemorySink = true;
		public bool enableFileSink = true;

		[Header("In Memory")]
		[Min(10)]
		public int retainedRecordCapacity = 500;

		[Header("File Output")]
		public string outputFolderName = "MutantLogs";
		public string fileNamePrefix = "mutant-log";
		public bool appendDateToFileName = true;
		public bool flushAfterEachWrite = true;

		public static MutantLogRuntimeSettings CreateRuntimeFallback()
		{
			MutantLogRuntimeSettings runtimeFallbackAsset = CreateInstance<MutantLogRuntimeSettings>();
			runtimeFallbackAsset.hideFlags = HideFlags.HideAndDontSave;
			return runtimeFallbackAsset;
		}
	}
}
