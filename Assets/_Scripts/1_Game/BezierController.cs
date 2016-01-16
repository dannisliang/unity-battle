using UnityEngine;
using System.Collections;

public class BezierController : MonoBehaviour
{
	[HideInInspector]
	public Transform t0 { get { return transform.GetChild (0); } }

	[HideInInspector]
	public Transform t1 { get { return transform.GetChild (1); } }

	[HideInInspector]
	public Transform t2 { get { return transform.GetChild (2); } }

	[HideInInspector]
	public Transform t3 { get { return transform.GetChild (3); } }

	public Vector3 GetPoint (float t)
	{
		return GetPoint (t0.position, t1.position, t2.position, t3.position, t);
	}

	public Vector3 GetVelocity (float t)
	{
		return GetFirstDerivative (t0.position, t1.position, t2.position, t3.position, t);
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
