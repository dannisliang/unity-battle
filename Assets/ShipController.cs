using UnityEngine;
using System.Collections;

public class ShipController : MonoBehaviour
{
	ParticleSystem ps;
	MeshRenderer meshRenderer;

	public Color neutral;
	public Color oneHit = Color.red;
	public Color nMinusOneHit = Color.yellow;

	void Awake ()
	{
		ps = GetComponentInChildren<ParticleSystem> ();
		ps.playOnAwake = false;
		ps.Clear ();
	}

	public void SetDamage (int hits, int size)
	{
		Color color = GetColor (hits, size);
		meshRenderer = GetComponentInChildren<MeshRenderer> ();
		meshRenderer.material.color = color;

		if (hits == 0 || hits == size) {
			ps.Stop ();
		} else {
			ParticleSystem.ShapeModule shape = ps.shape;
			shape.box = new Vector3 (2f * hits, 0f, 0f);
			ps.Play ();
		}
	}

	Color GetColor (int hits, int size)
	{
		if (hits == size) {
			return Color.black;
		}
		if (hits == 0) {
			return neutral;
		}
		float damage = (float)hits / (size - 1);
		return Color.Lerp (oneHit, nMinusOneHit, damage);
	}

}
