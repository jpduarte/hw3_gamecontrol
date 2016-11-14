using UnityEngine;
using System.Collections;

public class Destroyer : MonoBehaviour {

	void onTriggerEnter (Collider other) {
		print ("Collided");

		if (other.tag == "Player") {
			Destroy (other.gameObject);
			print ("Player");
		}
	}
}
