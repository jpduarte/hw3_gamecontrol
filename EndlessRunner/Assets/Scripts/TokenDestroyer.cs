using UnityEngine;
using System.Collections;

public class TokenDestroyer : MonoBehaviour {

	void onTriggerEnter (Collider other) {

			Destroy (gameObject);
			print ("Token COllision!!!");
	}
}
