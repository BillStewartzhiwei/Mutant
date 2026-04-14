using Mutant.Core;
using UnityEngine;

namespace Mutant.Log
{
	public sealed class LogModule : ModuleBase
	{
		private ILogService _logService;

		public override string ModuleId
		{
			get { return "Mutant.Log"; }
		}

		public override MutantBootPhase BootPhase
		{
			get { return MutantBootPhase.Infrastructure; }
		}

		public override int Order
		{
			get { return 0; }
		}

		public ILogService LogService
		{
			get { return _logService; }
		}

		public override void OnRegister()
		{
			Debug.Log("[Mutant.Log] OnRegister");
		}

		public override void OnInit()
		{
			_logService = new UnityConsoleLogService();
			MutantLog.Bind(_logService);

			MutantLog.Info("Mutant.Log", "OnInit");
		}

		public override void OnStart()
		{
			MutantLog.Info("Mutant.Log", "OnStart");
		}

		public override void OnStop()
		{
			MutantLog.Info("Mutant.Log", "OnStop");
		}

		public override void OnDispose()
		{
			MutantLog.Info("Mutant.Log", "OnDispose");
			MutantLog.Unbind();
			_logService = null;
		}
	}
}
