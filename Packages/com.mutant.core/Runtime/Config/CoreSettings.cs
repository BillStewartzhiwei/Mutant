using UnityEngine;

namespace Mutant.Core.Config
{
	[CreateAssetMenu(menuName = "Mutant/Core/Core Settings", fileName = "CoreSettings")]
	public sealed class CoreSettings : ScriptableObject
	{
		[Header("Bootstrap")]
		public bool dontDestroyOnLoad = true;
		public bool preventDuplicateBootstrap = true;

		[Header("Modules")]
		public bool enableAutoRegister = false;
		public bool logLifecycle = true;

		[Header("Events")]
		public bool logEventBus = false;
	}
}
