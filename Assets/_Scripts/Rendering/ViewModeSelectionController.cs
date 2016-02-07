using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class ViewModeSelectionController : MonoBehaviour,IPointerDownHandler
{
	public bool vrMode;
	public KeyCode keyCode;

	public void OnPointerDown (PointerEventData eventData)
	{
		DoSelectVrMode ();
	}

	void Update ()
	{
		if (Input.GetKeyDown (keyCode)) {
			DoSelectVrMode ();
		}
	}

	void DoSelectVrMode ()
	{
		Game.instance.SelectViewMode (vrMode);
	}
}
