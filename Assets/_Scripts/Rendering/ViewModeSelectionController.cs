using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

[RequireComponent (typeof(Button))]
public class ViewModeSelectionController : MonoBehaviour,IPointerDownHandler
{
	public bool vrMode;

	public void OnPointerDown (PointerEventData eventData)
	{
		Game.instance.SelectViewMode (vrMode);
	}
}
