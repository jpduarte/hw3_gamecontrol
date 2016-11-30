using UnityEngine;
using System.Collections;

public class PushUpStation : MonoBehaviour {

	//public PlayerController playerController;

	void Start () {

		//Get Character Controller Component
		//playerController = GetComponent<PlayerController> ();
	}


	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Player") {
			other.gameObject.GetComponent<PlayerController> ().StartPushUp ();
			print ("PushUp Collider");
			//playerController.GetScore ();;
		}
		Destroy (gameObject);
	}
}
