using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditor.SceneManagement;

[CustomEditor (typeof(BezierController))]
[CanEditMultipleObjects]
public class BezierEditor : Editor
{
	const int STEPS = 10;

	private void OnSceneGUI ()
	{
		BezierController bezier = target as BezierController;

		Handles.color = Color.white;
		Handles.DrawDottedLine (bezier.t0.position, bezier.t3.position, 5f);
		DoPositionHandle (bezier.t0);
		DoPositionHandle (bezier.t1);
		DoPositionHandle (bezier.t2);
		DoPositionHandle (bezier.t3);
		Handles.DrawBezier (bezier.t0.position, bezier.t3.position, bezier.t1.position, bezier.t2.position, Color.white, null, 4f);
		GUIStyle style = new GUIStyle ();
		style.fontSize = 20;
		Handles.Label (bezier.t0.position, "T0", style);
		Handles.Label (bezier.t1.position, "T1", style);
		Handles.Label (bezier.t2.position, "T2", style);
		Handles.Label (bezier.t3.position, "T3", style);
		for (int i = 0; i < STEPS; i++) {
			float t = (float)i / STEPS;
			Handles.color = Color.green;
			Vector3 point = bezier.GetPoint (t);
			Vector3 velocity = bezier.GetVelocity (t);
			Handles.DrawLine (point, point + velocity.normalized * 2f);
		}
	}

	void DoPositionHandle (Transform t)
	{
		EditorGUI.BeginChangeCheck ();
		Vector3 pos = Handles.DoPositionHandle (t.position, Tools.pivotRotation == PivotRotation.Local ? t.rotation : Quaternion.identity);
		if (EditorGUI.EndChangeCheck ()) {
			Undo.RecordObject (t, "Move " + t.name);
			EditorUtility.SetDirty (t);
			t.position = pos;
		}
	}

	public override void OnInspectorGUI ()
	{
		BezierController controller = (BezierController)target;

		GUILayout.Button ("Hi");
		DrawDefaultInspector ();

		if (GUI.changed) {
			Undo.SetCurrentGroupName ("Blah " + controller.name);
			EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
		}

		serializedObject.ApplyModifiedProperties ();
	}

}
