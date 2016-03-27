using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class TileController : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
	PositionMarkerController positionMarkerController;

	bool tileHasBeenFiredUpon;
	BoxCollider boxCollider;
	Vector3 initialScale;
	Vector3 growScale;

	void Awake ()
	{
		boxCollider = GetComponent<BoxCollider> ();
		positionMarkerController = GetComponent<PositionMarkerController> ();
	}

	void Start ()
	{
		initialScale = boxCollider.transform.localScale;
		growScale = initialScale * 1.5f;
	}

	public void OnPointerEnter (PointerEventData eventData)
	{
		boxCollider.transform.localScale = growScale;
		eventData.Use ();
		Highlight (true);
		RealtimeBattle.EncodeAndSendAim (positionMarkerController.position);
	}

	public void OnPointerExit (PointerEventData eventData)
	{
		if (SceneMaster.quitting) {
			return;
		}
		boxCollider.transform.localScale = initialScale;
		eventData.Use ();
		Highlight (false);
	}

	public void OnPointerDown (PointerEventData eventData)
	{
		eventData.Use ();
		if (BattleController.instance.FireAt (positionMarkerController.position, tileHasBeenFiredUpon)) {
			tileHasBeenFiredUpon = true;
		}
	}

	void Highlight (bool highlight)
	{
		BattleController.instance.AimAt (Whose.Theirs, highlight ? positionMarkerController.position : null);
#if UNITY_EDITOR
		if (!tileHasBeenFiredUpon && Input.GetKey (KeyCode.X) && BattleController.instance.IsGridReady ()) {
			StrikeResult result = BattleController.instance._Strike (Whose.Theirs, positionMarkerController.position);
			switch (result) {
			case StrikeResult.IGNORED_ALREADY_MISSED:
			case StrikeResult.IGNORED_ALREADY_HIT:
			case StrikeResult.MISS:
			case StrikeResult.HIT_NOT_SUNK:
			case StrikeResult.HIT_AND_SUNK:
				tileHasBeenFiredUpon = true;
				break;
			default:
				throw new System.NotImplementedException ();
			}
		}
#endif
	}
}