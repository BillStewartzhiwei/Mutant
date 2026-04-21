using System;
using System.Collections.Generic;

namespace Mutant.Core
{
	/// <summary>
	/// 模块基类。
	/// </summary>
	public abstract class ModuleBase : IMutantModule
	{
		public abstract string ModuleId { get; }

		public virtual MutantBootPhase BootPhase
		{
			get { return MutantBootPhase.Infrastructure; }
		}

		public virtual int Order
		{
			get { return 0; }
		}

		public virtual IReadOnlyList<string> Dependencies
		{
			get { return Array.Empty<string>(); }
		}

		public virtual void OnRegister()
		{
		}

		public virtual void OnInit()
		{
		}

		public virtual void OnStart()
		{
		}

		public virtual void OnStop()
		{
		}

		public virtual void OnDispose()
		{
		}
	}
}
