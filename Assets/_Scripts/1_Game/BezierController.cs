using UnityEngine;
using System.Collections;

public class BezierController : MonoBehaviour
{
	public class Points
	{
		public Transform t0;
		public Transform t1;
		public Transform t2;
		public Transform t3;
	}

	[HideInInspector]
	public Points points;

	void OnValidate ()
	{
		MayInitializePoints ();
	}

	void Awake ()
	{
		MayInitializePoints ();
	}

	void MayInitializePoints ()
	{
		if (points != null) {
			return;
		}
		points = new Points ();
		points.t0 = transform.GetChild (0);
		points.t1 = transform.GetChild (1);
		points.t2 = transform.GetChild (2);
		points.t3 = transform.GetChild (3);
	}

	public Vector3 GetPoint (float t)
	{
		return GetPoint (points.t0.position, points.t1.position, points.t2.position, points.t3.position, t);
	}

	public Vector3 GetVelocity (float t)
	{
		return GetFirstDerivative (points.t0.position, points.t1.position, points.t2.position, points.t3.position, t);
	}

	public static Vector3 GetPoint (Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{
		t = Mathf.Clamp01 (t);
		float oneMinusT = 1f - t;
		return
			oneMinusT * oneMinusT * oneMinusT * p0 +
		3f * oneMinusT * oneMinusT * t * p1 +
		3f * oneMinusT * t * t * p2 +
		t * t * t * p3;
	}

	public static Vector3 GetFirstDerivative (Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{
		t = Mathf.Clamp01 (t);
		float oneMinusT = 1f - t;
		return
			3f * oneMinusT * oneMinusT * (p1 - p0) +
		6f * oneMinusT * t * (p2 - p1) +
		3f * t * t * (p3 - p2);
	}

}
