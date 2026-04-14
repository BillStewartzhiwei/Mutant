namespace Mutant.Core
{
	/// <summary>
	/// 每帧更新接口。
	/// </summary>
	public interface IMutantUpdatable
	{
		void OnUpdate(float deltaTime);
	}
}
