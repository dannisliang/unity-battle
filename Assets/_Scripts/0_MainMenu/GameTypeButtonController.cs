using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameTypeButtonController : MonoBehaviour
{
	public GameType gameType;

	void Awake ()
	{
		GetComponent<Button> ().onClick.AddListener (delegate {
			ButlerController.instance.gameType = gameType;
		});
	}

	void Start ()
	{
		ButlerController.instance.OnGameTypeChanged += UpdateActive;
		ButlerController.instance.InvokeGameTypeChanged (UpdateActive);
	}

	void OnDestroy ()
	{
		if (!SceneMaster.quitting) {
			ButlerController.instance.OnGameTypeChanged -= UpdateActive;
		}
	}

	void UpdateActive (GameType gameType)
	{
		gameObject.SetActive ((gameType == GameType.NONE_SELECTED && this.gameType != GameType.NONE_SELECTED) || (gameType != GameType.NONE_SELECTED && this.gameType == GameType.NONE_SELECTED));
	}

}
