using UnityEngine;
using System.Collections;

public class TokenRotator : MonoBehaviour {

	
	// Update is called once per frame
	void Update () {

		transform.Rotate (new Vector3 (15, 30, 45) * Time.deltaTime * 3.0f);
	}
}
