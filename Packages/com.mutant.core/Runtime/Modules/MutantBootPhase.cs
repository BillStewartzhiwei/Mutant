using System;

namespace Mutant.Core
{
	/// <summary>
	/// 模块启动阶段。
	/// 先按阶段排序，再按依赖，再按 Order，再按注册顺序兜底。
	/// </summary>
	public enum MutantBootPhase
	{
		Infrastructure = 0,
		Platform = 100,
		Domain = 200,
		Application = 300
	}
}
