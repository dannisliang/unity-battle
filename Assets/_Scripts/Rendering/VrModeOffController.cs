using UnityEngine;
using System.Collections;

public class VrModeOffController : MonoBehaviour
{

	void Start ()
	{
		Cardboard.SDK.VRModeEnabled = false;
	}
	
}
