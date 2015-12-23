using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections;

[CustomEditor (typeof(GridPlacementController))]
[CanEditMultipleObjects]
public class GridHolderEditor : Editor
{

	public override void OnInspectorGUI ()
	{
		GridPlacementController controller = (GridPlacementController)target;

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
