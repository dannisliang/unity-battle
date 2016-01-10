using UnityEngine;
using System.Collections;

[RequireComponent (typeof(MeshFilter), typeof(MeshRenderer))]
public class AimReticleController : MonoBehaviour
{

	public float inner = .8f;
	public float outer = 1f;

	Vector3[] vertices;
	int[] triangles;

	void Awake ()
	{
		GenerateMesh ();
	}

	void OnValidate ()
	{
		inner = Mathf.Max (0, inner);
		outer = Mathf.Max (inner + 0.01f, outer);
		GenerateMesh ();
	}

	// Thanks to http://catlikecoding.com/unity/tutorials/procedural-grid/
	void GenerateMesh ()
	{
		// https://docs.google.com/drawings/d/18noGlM8Zkx9jDpK4sWDgrIlREdgPIF84i_8z7dtF2ho/edit
		float a = .5f * (outer - inner);
		float b = a + inner;
		vertices = new Vector3[] {
			new Vector2 (0, 0),
			new Vector2 (b, 0),
			new Vector3 (b, a),

			new Vector3 (outer, 0),
			new Vector3 (outer, b),
			new Vector3 (b, b),

			new Vector3 (outer, outer),
			new Vector3 (a, outer),
			new Vector3 (a, b),

			new Vector3 (0, outer),
			new Vector3 (0, a),
			new Vector3 (a, a),
		};

		triangles = new int[] {
			// quad 0
			0, 2, 1,
			0, 10, 2,

			// quad 1
			1, 4, 3,
			1, 5, 4,

			// quad 2
			8, 6, 4,
			8, 7, 6,

			// quad 3
			10, 9, 7,
			10, 7, 11,
		};

		Mesh mesh = new Mesh ();
		GetComponent<MeshFilter> ().mesh = mesh;
		mesh.name = "Aim Reticle Mesh";
		mesh.vertices = vertices;
		mesh.uv = null;
		mesh.triangles = triangles;
		mesh.RecalculateNormals ();
	}

}
