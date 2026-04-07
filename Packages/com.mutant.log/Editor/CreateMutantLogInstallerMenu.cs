using UnityEditor;
using UnityEngine;
using Mutant.Log.Installers;

namespace Mutant.Log.Editor
{
	public static class CreateMutantLogInstallerMenu
	{
		[MenuItem("GameObject/Mutant/Create Log Installer", false, 11)]
		public static void Create()
		{
			GameObject logInstallerObject = new GameObject("[MutantLog]");
			logInstallerObject.AddComponent<MutantLogModuleInstaller>();

			if (Selection.activeTransform != null)
				logInstallerObject.transform.SetParent(Selection.activeTransform, false);

			Undo.RegisterCreatedObjectUndo(logInstallerObject, "Create Log Installer");
			Selection.activeGameObject = logInstallerObject;
		}
	}
}
