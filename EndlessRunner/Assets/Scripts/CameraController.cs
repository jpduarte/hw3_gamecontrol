using UnityEngine;
using System.Collections;
using System;

public class CameraController : MonoBehaviour {

	private Transform playerTransform;
	private Vector3 startOffset;
	private Vector3 moveVector;

	private float transition = 0.0f;
	private float animationDuration = 3.0f;
	private Vector3 animationOffset = new Vector3(0,5,5);

	// Use this for initialization
	void Start () {

		//Find player GameObject's transform
		playerTransform = GameObject.FindGameObjectWithTag ("Player").transform;

		//Get the starting position of the camera relative to the player to keep that offset
		startOffset = transform.position - playerTransform.position;
	
	}
	
	// Update is called once per frame
	void Update () {

		// Get the new position of the player and add the camera offset
		moveVector = playerTransform.position + startOffset;

		// X Don't change X
		moveVector.x = 0;

		// Y Limit the change in Y (Height of Camera)
		//moveVector.y = Mathf.Clamp(moveVector.y,3,5);
		moveVector.y = 3.0f;


		if (transition > 1.0f) {
			
			//Don't move Y camera position if player only moves a little 
			if (Math.Abs (playerTransform.position.y - 3.0f) < 0.5f) {
				moveVector.y = 3.0f;
			}
			transform.position = moveVector;

		} else {
			
			//Animation at the start of the game
			transform.position = Vector3.Lerp(moveVector + animationOffset, moveVector, transition);
			transition += Time.deltaTime * 1 / animationDuration;
			transform.LookAt (playerTransform.position + Vector3.up);
		}
			
	}

	public void onDeath() {
		transition = 0.0f;
	}
}
