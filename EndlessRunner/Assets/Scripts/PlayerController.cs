﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class PlayerController : MonoBehaviour {


	private CharacterController controller;
	private Vector3 moveVector;
	private float animationDuration = 3.0f;
	private Animator anim;
	private bool isDead = false;
	private float score = 0.0f;
	private float vStop = 1.0f;

	private float timeFinishPushUp = 0.0f;
	private float timeAfterPushup = 0.0f;

	private bool isIdleToPushUp = false;
	private bool isPushUpToIdle = false;
	private bool isPushUp = false;
	private float timeStart = 0.0f;
	private Transform hips;
	private Behaviour halo;
	private float powerUpStartTime = 0.0f;
	

	public float vSpeed = 0.5f;
	public float hSpeed = 5.0f;
	public float verticalVelocity = 0.0f;
	public float gravity = 12.0f;
	public Text pushUpText;
	public CameraController cameraController;
	public float powerUpTimeLimit = 10.0f;

	private bool activeMagnet = false;


	// Use this for initialization
	void Start () {

		hips = transform.FindChild ("Boy:Hips");

		halo = (Behaviour)hips.GetComponent ("Halo");
			//(Behaviour)GetComponent("Halo");
		halo.enabled = false;

		//Get Character Controller Component
		//cameraController = GetComponent<CameraController> ();
		controller = GetComponent<CharacterController> ();
		anim = GetComponent<Animator> ();
		pushUpText.text = "";
		timeStart = Time.time;

	}
	
	// Update is called once per frame
	void Update () {

		timeAfterPushup = Time.time;

		if (((timeAfterPushup - timeFinishPushUp) > 1.0f) && !isIdleToPushUp) {
			pushUpText.text = "";
		}

		//If Dead player doesn't do anything
		if(isDead){
			return;
		}

		// Just go straight during beginning camera animation
		if ((Time.time - timeStart) < animationDuration) {

			controller.Move (Vector3.forward * vSpeed * Time.deltaTime);
			return;
		}

		if ((Time.time - powerUpStartTime) > powerUpTimeLimit) {
			activeMagnet = false;
			halo.enabled = false;
		}

		if (isIdleToPushUp) {

			/*
			while(vSpeed > 0.0f) {
				vSpeed -= vSpeed/5.0f * Time.deltaTime ;
				if (vSpeed < 0.0f)
					vSpeed = 0.0f;
			}
			*/
			vStop = 0.0f;

			//transform.Rot
			print ("PushUp");
		} else {
			vStop = 1.0f;
		}


		//Get inputs here
		float animateHorizontal = Input.GetAxis ("Horizontal");
		float moveHorizontal = Input.GetAxisRaw ("Horizontal");

		//Move the player
		Move (moveHorizontal);

		//Talk to animator controller
		Animate (animateHorizontal,isIdleToPushUp,isPushUp,isPushUpToIdle);




	}

	void Move(float moveHorizontal) {


		moveVector = Vector3.zero;


		//Check to see if player is grounded to add gravity

		if (controller.isGrounded) {
			verticalVelocity = 0f;
		} else {
			verticalVelocity -= gravity * Time.deltaTime;
		}

		// X - Left and Right
		moveVector.x = moveHorizontal * hSpeed;

		//print ("Horizontal Movement: " + moveVector.x);

		// Y - Up and Down
		moveVector.y = verticalVelocity;

		// Z - Forward and Backward
		moveVector.z = 1.0f * vSpeed;

		//Tell character to move forward every second at speed (speed m/s)
		controller.Move (moveVector * Time.deltaTime * vStop);
	}

	void Animate(float animateHorizontal, bool isIdleToPushUp, bool isPushUp, bool isPushUpToIdle) {

		anim.SetFloat ("hSpeed", animateHorizontal);

		anim.SetBool ("isIdleToPushUp", isIdleToPushUp);
		anim.SetBool ("isPushUp", isPushUp);
		anim.SetBool ("isPushUpToIdle", isPushUpToIdle);
	}

	public void SetSpeed(float modifier) {

		//Make faster if level increases
		vSpeed += 1.5f;
	}

	private void OnControllerColliderHit(ControllerColliderHit hit) {
		///print ("Hit Tag: " + hit.gameObject.tag);

		//If player hits a hazard, he dies
		if (hit.gameObject.tag == "Hazard") {
			//print ("Hit Point: " + hit.point.z);
			Death ();
		}

		if (hit.gameObject.tag == "Magnet") {
			MagnetPowerUp ();
			Destroy(hit.gameObject);
		}

		//if player hits a token he scores a point
		if (hit.gameObject.tag == "Token") {
			score += 1;
			//int deletedTokenIndex = GetComponent<PlatformController> ().GetTokenListIndex (hit.gameObject);
			Destroy (hit.gameObject);
			//GetComponent<PlatformController> ().UpdateTokenList (deletedTokenIndex);
			print("hit");
		}
			

	}

	public void StartPushUp() {
		//print ("SetPushup");
		isIdleToPushUp = true;
		pushUpText.text = "Do 2 PushUPs!";

	}

	public void ContinuePushUp() {
		isPushUp = true;
	}

	public void FinishPushUp() {
		isPushUpToIdle = true;
	}

	public void DonePushUp() {
		isIdleToPushUp = false;
		isPushUp = false;
		isPushUpToIdle = false;
		//float offCenter = transform.localEulerAngles.y;
		pushUpText.text = "Good Job!";
		transform.rotation = Quaternion.identity;
		timeFinishPushUp = Time.time;

		//float zPosition = transform.position.z;
		//transform.position = new Vector3 (0.0f, 0.0f, zPosition);

		//Vector3 goStraight;
		//int i = 0;
		//while (offCenter != 0.0f) {
		//GetComponent<Transform>().Rotate (new Vector3 (0.0f, -90.0f, 0.0f));
		//	offCenter = transform.localEulerAngles.y;
		//print ("OffCenter: " + offCenter);

		//}

		//transform.localEularAngles = new Vector3 (0f, 0f, 0f);
		//Quaternion offCenter = transform.rotation;
		//Vector3 goStraight = offCenter.eulerAngles*-5.0f;
		//transform.localEulerAngles = new Vector3.zero;

	}
		
	public float GetScore() {
		return score;
	}

	private void MagnetPowerUp() {
		halo.enabled = true;
		activeMagnet = true;
		powerUpStartTime = Time.time;
	}

	public bool GetMagnetStatus() {
		return activeMagnet;
	}

	private void Death() {
		//print ("Dead!");
		isDead = true;
		GetComponent<ScoreController> ().onDeath ();
		cameraController.onDeath ();
	}
}