using System;

namespace Mutant.Core
{
	/// <summary>
	/// 模块启动阶段错误记录。
	/// </summary>
	public sealed class ModuleStartupError
	{
		public ModuleStartupError(string moduleId, string stage, Exception exception)
		{
			ModuleId = moduleId ?? string.Empty;
			Stage = stage ?? string.Empty;
			Exception = exception;
			Message = exception != null ? exception.Message : string.Empty;
		}

		public string ModuleId { get; }

		public string Stage { get; }

		public Exception Exception { get; }

		public string Message { get; }

		public override string ToString()
		{
			return string.Format("[{0}] {1}: {2}", Stage, ModuleId, Message);
		}
	}
}
