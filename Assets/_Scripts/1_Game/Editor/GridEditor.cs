using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections;

[CustomEditor (typeof(GridController))]
[CanEditMultipleObjects]
public class GridEditor : Editor
{

	public override void OnInspectorGUI ()
	{
		GridController controller = (GridController)target;

		if (GUILayout.Button ("RECREATE BOATS")) {
			Undo.SetCurrentGroupName ("Recreate " + controller.name);
			controller.Init (typeof(GridEditor).Name);
			EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
		}

		DrawDefaultInspector ();
		serializedObject.ApplyModifiedProperties ();
	}

}
