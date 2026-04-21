using System;
using System.Collections.Generic;

namespace Mutant.Core
{
	/// <summary>
	/// 启动计划。
	/// </summary>
	public sealed class StartupPlan
	{
		public StartupPlan(IReadOnlyList<StartupBatch> batches)
		{
			if (batches == null)
			{
				throw new ArgumentNullException(nameof(batches));
			}

			Batches = batches;
			OrderedModules = BuildOrderedModules(batches);
		}

		public IReadOnlyList<StartupBatch> Batches { get; }

		public IReadOnlyList<ModuleDescriptor> OrderedModules { get; }

		private static IReadOnlyList<ModuleDescriptor> BuildOrderedModules(IReadOnlyList<StartupBatch> batches)
		{
			var ordered = new List<ModuleDescriptor>();

			for (int i = 0; i < batches.Count; i++)
			{
				var modules = batches[i].Modules;
				for (int m = 0; m < modules.Count; m++)
				{
					ordered.Add(modules[m]);
				}
			}

			return ordered.AsReadOnly();
		}
	}
}
