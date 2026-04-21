namespace Mutant.Core
{
	/// <summary>
	/// 固定步长更新接口。
	/// </summary>
	public interface IMutantFixedUpdatable
	{
		void OnFixedUpdate(float fixedDeltaTime);
	}
}
