using System;

namespace Mutant.Core
{
	/// <summary>
	/// 把已启动模块绑定进 MutantLoop。
	/// 绑定顺序直接复用 StartupPlan.OrderedModules 的顺序，
	/// 这样启动顺序和运行顺序尽可能保持一致。
	/// </summary>
	public sealed class LoopBindingBuilder
	{
		public void BindStartedModules(ModuleManager moduleManager)
		{
			if (moduleManager == null)
			{
				throw new ArgumentNullException(nameof(moduleManager));
			}

			MutantLoop.Clear();

			var startedModules = moduleManager.GetStartedDescriptorsInStartupOrder();
			for (int i = 0; i < startedModules.Count; i++)
			{
				var descriptor = startedModules[i];
				MutantLoop.RegisterModule(descriptor.ModuleId, descriptor.Instance);
			}
		}
	}
}
