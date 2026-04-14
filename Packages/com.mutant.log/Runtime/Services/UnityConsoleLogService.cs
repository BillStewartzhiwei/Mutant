using UnityEngine;

namespace Mutant.Log
{
	public sealed class UnityConsoleLogService : ILogService
	{
		public void Info(string categoryName, string message)
		{
			Debug.Log(Format(categoryName, message));
		}

		public void Warning(string categoryName, string message)
		{
			Debug.LogWarning(Format(categoryName, message));
		}

		public void Error(string categoryName, string message)
		{
			Debug.LogError(Format(categoryName, message));
		}

		private static string Format(string categoryName, string message)
		{
			return $"[{categoryName}] {message}";
		}
	}
}
