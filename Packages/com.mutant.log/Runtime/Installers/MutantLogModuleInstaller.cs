using UnityEngine;
using Mutant.Core.Modules;
using Mutant.Log.Config;
using Mutant.Log.Modules;

namespace Mutant.Log.Installers
{
	public sealed class MutantLogModuleInstaller : MonoBehaviour
	{
		[SerializeField] private MutantLogRuntimeSettings _logSettingsAsset;

		private void Awake()
		{
			MutantLogModule.Configure(_logSettingsAsset);
			ModuleManager.Instance.Register<MutantLogModule>();
		}
	}
}
