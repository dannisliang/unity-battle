using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameModePanelController : MonoBehaviour
{
	public GameType gameType;

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
		gameObject.SetActive (gameType == this.gameType);
	}
}
