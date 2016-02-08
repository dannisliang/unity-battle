using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections;

[CustomEditor (typeof(BoatPlacementController))]
[CanEditMultipleObjects]
public class BoatPlacementEditor : Editor
{

	public override void OnInspectorGUI ()
	{
		BoatPlacementController controller = (BoatPlacementController)target;

		GUILayout.Button ("RECREATE BOATS");
		DrawDefaultInspector ();

		if (GUI.changed) {
			Undo.SetCurrentGroupName ("Recreate " + controller.name);
			controller.RecreateBoats (typeof(BoatPlacementEditor).Name);
			EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
		}

		serializedObject.ApplyModifiedProperties ();
	}

}
