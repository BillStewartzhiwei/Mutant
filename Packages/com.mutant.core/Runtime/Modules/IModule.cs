namespace Mutant.Core.Modules
{
	public interface IModule
	{
		string Name { get; }
		int Priority { get; }
		bool IsInitialized { get; }

		void Init();
		void Update();
		void LateUpdate();
		void FixedUpdate();
		void Dispose();
	}
}
