using UnityEngine;
using System.Collections;

public class MistController : MonoBehaviour {

	public float platformLength = 7.5f;
	public int nPlatforms = 10;
	public GameObject player;
	// Use this for initialization
	void Start () {
		transform.position = new Vector3 (0, 0, player.transform.position.z + (nPlatforms - 2) * platformLength);
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3 (0, 0, player.transform.position.z + (nPlatforms -2)  * platformLength);
	}
}
