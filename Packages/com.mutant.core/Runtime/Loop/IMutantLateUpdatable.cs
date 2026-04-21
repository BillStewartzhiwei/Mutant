namespace Mutant.Core
{
	/// <summary>
	/// LateUpdate 更新接口。
	/// </summary>
	public interface IMutantLateUpdatable
	{
		void OnLateUpdate(float deltaTime);
	}
}
