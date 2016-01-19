using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof(Button))]
public class ViewModeSelectionButtonController : MonoBehaviour
{
	public bool vrMode;

	void Awake ()
	{
		GetComponent<Button> ().onClick.AddListener (delegate {
			Game.instance.SelectViewMode (vrMode);
		});
	}

	//	void Start ()
	//	{
	//		Game.instance.OnGameStateChange += UpdateActive;
	//	}
	//
	//	void OnDestroy ()
	//	{
	//		if (Game.instance != null) {
	//			Game.instance.OnGameStateChange -= UpdateActive;
	//		}
	//	}
	//
	//	void UpdateActive (GameState gameState)
	//	{
	//		gameObject.SetActive (gameState == GameState.NEED_TO_SELECT_GAME_TYPE);
	//	}

}
