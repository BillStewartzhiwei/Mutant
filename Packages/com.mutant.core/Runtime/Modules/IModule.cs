namespace Mutant.Core.Modules
{
	public interface IModule
	{
		int Priority { get; }

		void Init();
		void Update();
		void LateUpdate();
		void FixedUpdate();
		void Dispose();
	}
}
