using UnityEngine;
using System.Collections;

public class SquatStation : MonoBehaviour {

	void Start () {

		//Get Character Controller Component
		//playerController = GetComponent<PlayerController> ();
	}


	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Player") {
			other.gameObject.GetComponent<PlayerController> ().StartSquat ();
			print ("Squat Collider");
			//playerController.GetScore ();;
		}
		Destroy (gameObject);
	}
}

