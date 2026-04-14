using UnityEditor;
using UnityEngine;
// using Mutant.Core.Bootstrap;

namespace Mutant.Core.Editor
{
	public static class CreateCoreBootstrapMenu
	{
		[MenuItem("GameObject/Mutant/Create Core Bootstrap", false, 10)]
		public static void Create()
		{
			// GameObject go = new GameObject("[MutantCore]");
			// go.AddComponent<CoreBootstrap>();
			//
			// if (Selection.activeTransform != null)
			// 	go.transform.SetParent(Selection.activeTransform, false);
			//
			// Undo.RegisterCreatedObjectUndo(go, "Create Core Bootstrap");
			// Selection.activeGameObject = go;
		}
	}
}
