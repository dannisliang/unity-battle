using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameModePanelController : MonoBehaviour
{
	public GameType gameType;

	void Start ()
	{
		Game.instance.OnGameTypeChanged += UpdateActive;
		Game.instance.InvokeGameTypeChanged (UpdateActive);
	}

	void OnDestroy ()
	{
		if (!SceneMaster.quitting) {
			Game.instance.OnGameTypeChanged -= UpdateActive;
		}
	}

	void UpdateActive (GameType gameType)
	{
		gameObject.SetActive (gameType == this.gameType);
	}
}
