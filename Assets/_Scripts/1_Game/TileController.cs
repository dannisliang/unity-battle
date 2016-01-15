using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class TileController : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
	PositionMarkerController positionMarkerController;

	bool tileHasBeenFiredUpon;

	void Awake ()
	{
		positionMarkerController = GetComponent<PositionMarkerController> ();
	}

	public void OnPointerEnter (PointerEventData eventData)
	{
		eventData.Use ();
		Highlight (!tileHasBeenFiredUpon);
		RealtimeBattle.EncodeAndSendAim (positionMarkerController.position);
	}

	public void OnPointerExit (PointerEventData eventData)
	{
		if (SceneMaster.quitting) {
			return;
		}
		eventData.Use ();
		Highlight (false);
	}

	public void OnPointerDown (PointerEventData eventData)
	{
		if (tileHasBeenFiredUpon) {
			BattleController.instance.FalseFire ();
			return;
		}
		eventData.Use ();
		if (BattleController.instance.FireAt (positionMarkerController.position)) {
			tileHasBeenFiredUpon = true;
		}
	}

	void Highlight (bool highlight)
	{
		BattleController.instance.AimAt (Whose.Theirs, highlight ? positionMarkerController.position : null);
#if UNITY_EDITOR
		if (!tileHasBeenFiredUpon && Input.GetKey (KeyCode.X)) {
			RealtimeBattle.EncodeAndSendHit (positionMarkerController.position);
			StrikeResult result = BattleController.instance.Strike (Whose.Theirs, positionMarkerController.position);
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