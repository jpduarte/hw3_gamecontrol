using UnityEngine;
using System.Collections;

public class TokenMagnet : MonoBehaviour {

	//public Transform player;
	public float xCoinSpeed = 10.0f;
	public float zCoinSpeed = 5.0f;

	private float xMagnet;
	private float zMagnet;
	private GameObject player;
	private PlayerController playerController;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		playerController = player.GetComponent<PlayerController> ();
	}

	// Update is called once per frame
	void Update () {

		if (playerController.GetMagnetStatus ()) {
			
			if (Vector3.Distance (transform.position, player.transform.position) > 0.0f) {
				zMagnet = Vector3.MoveTowards (transform.position, player.transform.position, Time.deltaTime * zCoinSpeed).z;
				xMagnet = Vector3.MoveTowards (transform.position, player.transform.position, Time.deltaTime * xCoinSpeed).x;
				transform.position = new Vector3 (xMagnet, transform.position.y, zMagnet);
				//transform.Translate(new Vector3 (player.position.x, transform.position.y, player.position.z));
			}
		}
	
	}
}
