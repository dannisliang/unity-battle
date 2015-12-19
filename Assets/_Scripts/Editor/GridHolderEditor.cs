using UnityEngine;
using UnityEditor;
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
			controller.RecreateGrid ();
		}

		serializedObject.ApplyModifiedProperties ();
	}

}
