using System.Collections.Generic;

namespace Mutant.Core
{
	/// <summary>
	/// Mutant 模块统一接口。
	/// 模块只声明自身事实，不自己决定启动时机。
	/// </summary>
	public interface IMutantModule
	{
		/// <summary>
		/// 模块唯一 ID，例如：Mutant.Log、Mutant.VR。
		/// </summary>
		string ModuleId { get; }

		/// <summary>
		/// 模块所属启动阶段。
		/// </summary>
		MutantBootPhase BootPhase { get; }

		/// <summary>
		/// 同阶段内的排序权重，越小越早。
		/// </summary>
		int Order { get; }

		/// <summary>
		/// 依赖的模块 ID 列表。
		/// </summary>
		IReadOnlyList<string> Dependencies { get; }

		/// <summary>
		/// 进入 Core 管理体系时调用。
		/// </summary>
		void OnRegister();

		/// <summary>
		/// 初始化资源、依赖和内部状态。
		/// </summary>
		void OnInit();

		/// <summary>
		/// 真正开始对外提供服务。
		/// </summary>
		void OnStart();

		/// <summary>
		/// 停止服务。
		/// </summary>
		void OnStop();

		/// <summary>
		/// 销毁资源。
		/// </summary>
		void OnDispose();
	}
}
