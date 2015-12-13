using UnityEngine;
using UnityEditor;
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
		}

		serializedObject.ApplyModifiedProperties ();
	}

}
