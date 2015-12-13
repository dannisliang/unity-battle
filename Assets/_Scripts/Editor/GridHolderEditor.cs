using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof(GridHolderController))]
[CanEditMultipleObjects]
public class GridHolderEditor : Editor
{

	public override void OnInspectorGUI ()
	{
		GridHolderController controller = (GridHolderController)target;

		GUILayout.Button ("RECREATE CHILDREN");
		DrawDefaultInspector ();
		
		if (GUI.changed) {
			controller.RecreateGrid ();
		}

		serializedObject.ApplyModifiedProperties ();
	}

}
