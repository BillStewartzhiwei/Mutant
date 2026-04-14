namespace Mutant.Log
{
	public interface ILogService
	{
		void Info(string categoryName, string message);
		void Warning(string categoryName, string message);
		void Error(string categoryName, string message);
	}
}
