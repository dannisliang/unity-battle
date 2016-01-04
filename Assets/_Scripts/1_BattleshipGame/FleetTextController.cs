using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof(Text))]
public class FleetTextController : MonoBehaviour
{

	void OnValidate ()
	{
		string t = "";
		for (int i = 0; i < Grid.fleet.Length; i++) {
			BoatConfiguration config = Grid.fleet [i];
			t += config.designation + "\n- " + config.size + " units\n\n";
		}
		GetComponent<Text> ().text = t;
	}
	
}
