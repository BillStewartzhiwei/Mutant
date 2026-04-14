using UnityEngine;

namespace Mutant.Log
{
	public static class MutantLog
	{
		private static ILogService _service;

		public static bool IsReady
		{
			get { return _service != null; }
		}

		internal static void Bind(ILogService service)
		{
			_service = service;
		}

		internal static void Unbind()
		{
			_service = null;
		}

		public static void Info(string categoryName, string message)
		{
			if (_service != null)
			{
				_service.Info(categoryName, message);
				return;
			}

			Debug.Log($"[{categoryName}] {message}");
		}

		public static void Warning(string categoryName, string message)
		{
			if (_service != null)
			{
				_service.Warning(categoryName, message);
				return;
			}

			Debug.LogWarning($"[{categoryName}] {message}");
		}

		public static void Error(string categoryName, string message)
		{
			if (_service != null)
			{
				_service.Error(categoryName, message);
				return;
			}

			Debug.LogError($"[{categoryName}] {message}");
		}
	}
}
