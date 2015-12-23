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
			controller.RecreateBoats ();
//			EditorUtility.SetDirty (controller.gameObject);
  			EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
//			Debug.Log ("SetDirty");
		}

		serializedObject.ApplyModifiedProperties ();
	}

}
