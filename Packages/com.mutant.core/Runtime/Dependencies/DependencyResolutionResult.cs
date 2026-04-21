using System;
using System.Collections.Generic;

namespace Mutant.Core
{
	/// <summary>
	/// 依赖解析结果。
	/// </summary>
	public sealed class DependencyResolutionResult
	{
		public DependencyResolutionResult(IReadOnlyList<ModuleDescriptor> orderedModules)
		{
			if (orderedModules == null)
			{
				throw new ArgumentNullException(nameof(orderedModules));
			}

			OrderedModules = orderedModules;
		}

		public IReadOnlyList<ModuleDescriptor> OrderedModules { get; }
	}
}
