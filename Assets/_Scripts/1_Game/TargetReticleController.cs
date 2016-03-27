using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Animator), typeof(CardboardAudioSource))]
public class TargetReticleController : MonoBehaviour
{
	public Material materialVrMmode;
	public Material materialMagicWindow;

	const float velocity = 8f;

	MeshRenderer meshRenderer;
	Animator animator;
	CardboardAudioSource audioSource;
	Vector3 targetPos;

	void Awake ()
	{
		meshRenderer = GetComponentInChildren<MeshRenderer> ();
		animator = GetComponent<Animator> ();
		audioSource = GetComponent<CardboardAudioSource> ();
		gameObject.SetActive (false);
	}

	void OnEnable ()
	{
		Game.instance.OnGameStateChange += HandleGameStateChange;
	}

	void OnDisable ()
	{
		Game.instance.OnGameStateChange -= HandleGameStateChange;
	}

	void Update ()
	{
		transform.localPosition = Vector3.Lerp (transform.localPosition, targetPos, Time.deltaTime * velocity);
	}

	void HandleGameStateChange (GameState state)
	{
		meshRenderer.material = Cardboard.SDK.VRModeEnabled ? materialVrMmode : materialMagicWindow;
	}

	public void SetTargetPosition (Position position, bool lockOnTarget)
	{
		gameObject.SetActive (position != null);
		if (position != null) {
			targetPos = position.AsGridLocalPosition (Marker.Target);
			if (lockOnTarget) {
				audioSource.Play ();
				animator.SetTrigger ("LockOnTarget");
			}
		}
		GetComponent<Animator> ().enabled = lockOnTarget;
	}

}
