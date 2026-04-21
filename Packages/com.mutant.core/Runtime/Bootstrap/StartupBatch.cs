using System;
using System.Collections.Generic;

namespace Mutant.Core
{
	/// <summary>
	/// 启动批次。
	/// 同一批次中的模块不存在“同阶段内的直接依赖”。
	/// </summary>
	public sealed class StartupBatch
	{
		public StartupBatch(MutantBootPhase phase, int batchIndex, IReadOnlyList<ModuleDescriptor> modules)
		{
			if (modules == null)
			{
				throw new ArgumentNullException(nameof(modules));
			}

			Phase = phase;
			BatchIndex = batchIndex;
			Modules = modules;
		}

		public MutantBootPhase Phase { get; }

		public int BatchIndex { get; }

		public IReadOnlyList<ModuleDescriptor> Modules { get; }
	}
}
