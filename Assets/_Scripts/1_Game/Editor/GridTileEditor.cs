using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections;

[CustomEditor (typeof(GridTileController))]
[CanEditMultipleObjects]
public class GridTileEditor : Editor
{

	public override void OnInspectorGUI ()
	{
		GridTileController controller = (GridTileController)target;

		GUILayout.Button ("RECREATE GRID");
		DrawDefaultInspector ();
		
		if (GUI.changed) {
			Undo.SetCurrentGroupName ("Recreate " + controller.name);
			controller.RecreateGrid ();
			EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
		}

		serializedObject.ApplyModifiedProperties ();
	}

}
