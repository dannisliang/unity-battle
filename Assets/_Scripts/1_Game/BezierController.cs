using UnityEngine;
using System.Collections;

public class BezierController : MonoBehaviour
{
	[HideInInspector]
	public PosRot pr0;

	[HideInInspector]
	public PosRot pr1;

	[HideInInspector]
	public PosRot pr2;

	[HideInInspector]
	public PosRot pr3;

	void OnValidate ()
	{
		InitializePoints ();
	}

	void Awake ()
	{
		InitializePoints ();
	}

	public Transform transform0 {
		get {
			return AdjustTransform (transform.GetChild (0), pr0);
		}
	}

	public Transform transform1 {
		get {
			return AdjustTransform (transform.GetChild (1), pr1);
		}
	}

	public Transform transform2 {
		get {
			return AdjustTransform (transform.GetChild (2), pr2);
		}
	}

	public Transform transform3 {
		get {
			return AdjustTransform (transform.GetChild (3), pr3);
		}
	}

	Transform AdjustTransform (Transform t, PosRot pr)
	{
		t.position = pr.position;
		t.rotation = pr.rotation;
		return t;
	}

	void InitializePoints ()
	{
		pr0 = pr0 ?? new PosRot (transform.GetChild (0));
		pr1 = pr1 ?? new PosRot (transform.GetChild (1));
		pr2 = pr2 ?? new PosRot (transform.GetChild (2));
		pr3 = pr3 ?? new PosRot (transform.GetChild (3));
	}

	public Vector3 GetPoint (float t)
	{
		return GetPoint (pr0.position, pr1.position, pr2.position, pr3.position, t);
	}

	public Vector3 GetVelocity (float t)
	{
		return GetFirstDerivative (pr0.position, pr1.position, pr2.position, pr3.position, t);
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
