using System;

namespace Mutant.Core
{
	/// <summary>
	/// 模块生命周期状态。
	/// </summary>
	public enum MutantModuleState
	{
		None = 0,

		Registered = 10,
		Resolved = 20,

		Initializing = 30,
		Initialized = 40,

		Starting = 50,
		Started = 60,

		Stopping = 70,
		Stopped = 80,

		Disposing = 90,
		Disposed = 100,

		Failed = 999
	}
}
