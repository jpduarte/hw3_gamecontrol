﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;


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
	private bool PushUpState = false;
	private bool isIdle = false;
	private bool isSquat = false;
	private bool SquatState = false;
	//private bool isPushUp = false;

	private DateTime MatTimeStamp = DateTime.Now;



	private float timeStart = 0.0f;
	//private Transform hips;
	private Light powerUpLight;

	private int pushUpNumber = 5;
	private int pushUpCount = 0;
	private bool pushUpDetected = false;
	private bool pushUpDone = true;

	private int squatNumber = 5;
	private int squatCount = 0;
	private bool squatDetected = false;
	private bool squatDone = true;

	public float vSpeed = 0.5f;
	public float hSpeed = 5.0f;
	public float verticalVelocity = 0.0f;
	public float gravity = 12.0f;
	public Text exerciseText;
	public CameraController cameraController;
	public ScoreController scoreController;
	public PressureMatController pressureMatController;
	public SettingsController settingsController;
	public InputController _inputController;
	public MenuController menuController;
	public float powerUpTimeLimit = 10.0f;
	public Text IPText;
	private int tagNum = 0;

	private bool activeMagnet = false;
	private bool activeStar = false;
	//define Input Controller class

	float[] mat_data = new float[96];



	// Use this for initialization
	void Start () {
		Time.timeScale = 1;
		powerUpLight = transform.FindChild ("PowerUpLight").GetComponent<Light>();

			//(Behaviour)GetComponent("Halo");
		powerUpLight.enabled = false;

		//Get Character Controller Component
		//cameraController = GetComponent<CameraController> ();
		controller = GetComponent<CharacterController> ();
		anim = GetComponent<Animator> ();
		exerciseText.text = "";
		timeStart = Time.time;

		_inputController.Begin("172.20.10.4", 23);

	}
	
	// Update is called once per frame
	void Update () {


		if (Input.GetKeyDown (KeyCode.Escape)) {
			menuController.Restart ();
		}

		if (Input.GetKeyDown (KeyCode.Space)) {
			settingsController.SettingsMenu ();
			if (Time.timeScale == 1) {
				Time.timeScale = 0;
			} else {
				Time.timeScale = 1;
			}
		}





		timeAfterPushup = Time.time;

		if (((timeAfterPushup - timeFinishPushUp) > 1.0f) && !isIdleToPushUp) {
			exerciseText.text = "";
		}

		//If Dead player doesn't do anything
		if(isDead){
			_inputController.connection_status = false;
			return;
		}

		// Just go straight during beginning camera animation
		if ((Time.time - timeStart) < animationDuration) {

			controller.Move (Vector3.forward * vSpeed * Time.deltaTime);
			return;
		}



		if (activeMagnet || activeStar) {
			powerUpLight.enabled = true;
			if (activeMagnet) {
				powerUpLight.color = Color.red;
			}
			if (activeStar) {
				powerUpLight.color = Color.blue;
			}
		} else {
			powerUpLight.enabled = false;
		}

		if (PushUpState || SquatState) {
			vStop = 0.0f;
			if (SquatState) {
				SquatAnimation ();
			}
			if (PushUpState) {
				PushUpAnimation ();
			}


			//transform.Rot
			//print ("PushUp");
		} else {
			vStop = 1.0f;
		}

		//float animateHorizontal = 0;
		float moveHorizontal = 0;

		if (_inputController.connection_status) {
			//print(_inputController.stringtoprint);

			/*
			if (_inputController.RedBearData.Count > 0) {
				string RedBearLine = _inputController.RedBearData.Dequeue ();
				//MatTimeStamp = _inputController.TimeFIFO.Dequeue ();
				tagNum = _inputController.RedBeatTag.Dequeue();
				//print ("tagNum" + tagNum);
				string[] move = RedBearLine.Split (',');
				//print ("RedBearLine " + RedBearLine);

				if (move [move.Length - 1] != null) {
					//print ("Accel Data: "+ move[1]);
					for (int i = 0; i < move.Length - 3; i++) {
						mat_data [i] = float.Parse (move [i + 2]);
						if (mat_data [i] < 0)
							print ("Negative: " + mat_data [i].ToString ());
					}
					//	animateHorizontal = float.Parse (move [1]);
					//moveHorizontal = float.Parse (move [1]);
					//animateHorizontal = Input.GetAxis("Horizontal");
					moveHorizontal = Input.GetAxisRaw ("Horizontal");
					//print ("Move Horizontal " + moveHorizontal);
				}

			}*/
		} else {
			//animateHorizontal = Input.GetAxis("Horizontal");
			moveHorizontal = Input.GetAxisRaw("Horizontal");
		}


		//Move the player
		Move (moveHorizontal);


		//Talk to animator controller
		Animate ();




	}


	void Move(float moveHorizontal) {


		moveVector = Vector3.zero;


		//Check to see if player is grounded to add gravity

		if (controller.isGrounded || activeStar) {
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

	void Animate() {

		anim.SetBool ("isIdleToPushUp", isIdleToPushUp);
		anim.SetBool ("isPushUp", isPushUp);
		anim.SetBool ("isPushUpToIdle", isPushUpToIdle);
		anim.SetBool ("isSquat", isSquat);
		anim.SetBool ("isIdle", isIdle);
	}

	public void SetSpeed(float modifier) {

		//Make faster if level increases
		vSpeed = vSpeed + score*0.05f;;
	}

	private void OnControllerColliderHit(ControllerColliderHit hit) {
		///print ("Hit Tag: " + hit.gameObject.tag);

		//If player hits a hazard, he dies
		if (hit.gameObject.tag == "Hazard" && !activeStar) {
			//print ("Hit Point: " + hit.point.z);
			Death ();
		}

		if (hit.gameObject.tag == "Magnet") {
			//MagnetPowerUp ();
			//Destroy(hit.gameObject);
		}

		//if player hits a token he scores a point
		if (hit.gameObject.tag == "Token") {
			//score += 1;
			//int deletedTokenIndex = GetComponent<PlatformController> ().GetTokenListIndex (hit.gameObject);
			//Destroy (hit.gameObject);
			//GetComponent<PlatformController> ().UpdateTokenList (deletedTokenIndex);
			//print("hit");
		}
			

	}

	public void StartPushUp() {
		isIdle = true;
		PushUpState = true;

	}

	public void PushUpAnimation () {
		bool isFeet;
		bool isHands;
		int state = pressureMatController.GetState ();
		if (state == 2 || state == 3) {
			isFeet = true;
			isHands = true;
		} else {
			isFeet = false;
			isHands = false;
		}
		exerciseText.text = pushUpCount.ToString() + " Out of "  + pushUpNumber.ToString() + " PushUps!";
		//Don't get into pushup position until player gets off board
		//if (!isFeet) {
		if(!isFeet) {
			isIdleToPushUp = true;
		}

		//Don't start pushup until hands on board
		if ((isIdleToPushUp && isHands) || isPushUp) {

			//Check if pushup detected
			pushUpDetected = pressureMatController.GetPushUpDetected ();
			if (!pushUpDetected) {
				pushUpDone = true;
			}
			if (pushUpDetected && pushUpDone) {
				pushUpCount++;
				pushUpDone = false;
			}

			if (pushUpCount < pushUpNumber) {
				isPushUp = true;
			} else {
				isPushUp = false;
				isPushUpToIdle = true;
			}
		}

		if (isPushUpToIdle && isFeet) {
			DoneExercise ();
		}
			
			
			
	}

	public void ContinuePushUp() {
		isPushUp = true;

	}

	public void PushUpCount () {
		//pushUpCount++;
		//pushUpText.text = pushUpCount.ToString() + " Out of "  + pushUpNumber.ToString() + " Push UPs!";
	}

	public void FinishPushUp() {
		if (pushUpCount >= pushUpNumber) {
			isPushUpToIdle = true;
		}

		
	}
		
		

	public bool GetExercise() {
		return (PushUpState || SquatState);
	}

	public void StartSquat() {
		isIdle = true;
		SquatState = true;
	}

	public void SquatAnimation() {
		exerciseText.text = squatCount.ToString() + " Out of "  + squatNumber.ToString() + " Squats!";
		bool isFeet;
		bool isHands;
		int state = pressureMatController.GetState ();
		if (state == 2 || state == 3) {
			isFeet = true;
			isHands = true;
		} else {
			isFeet = false;
			isHands = false;
		}
		// Don't start squat animation until both feet on
		if (isFeet || isSquat) {
			squatDetected = pressureMatController.GetSquatDetected ();
			if (!squatDetected) {
				squatDone = true;
			}
			if (squatDetected && squatDone) {
				squatCount++;
				squatDone = false;
			}

			if (squatCount < squatNumber) {
				isSquat = true;
			} else {
				isSquat = false;
				isIdle = false;
				DoneExercise ();
			}
		}
	}

	public void DoneExercise() {
		isIdle = false;

		pushUpCount = 0;
		PushUpState = false;
		isIdleToPushUp = false;
		isPushUpToIdle = false;
		isPushUp = false;

		squatCount = 0;
		SquatState = false;
		isSquat = false;

		exerciseText.text = "Good Job!";
		timeFinishPushUp = Time.time;
	}
		
	public float GetScore() {
		return score;
	}

	public void MagnetPowerUp(bool getMagnet) {
		activeMagnet = getMagnet;
	}

	public void StarPowerUp(bool getStar) {
		activeStar = getStar;
	}

	public bool GetMagnetStatus() {
		return activeMagnet;
	}



	private void Death() {
		//print ("Dead!");
		isDead = true;
		scoreController.onDeath ();
		cameraController.onDeath ();
	}

	public float[] GetMatData() {
		return mat_data;
	}

	public DateTime GetMatTimeStamp() {
		return MatTimeStamp;
	}

	public int GetTagNum () {
		return tagNum;
	}

	public void FakeSquat() {
		squatCount++;
	}

	public void FakePushUp() {
		pushUpCount++;
	}

	public void NewIPAddress() {
		if (IPText != null) {
			print ("IP Text" + IPText.text);
			_inputController.Begin (IPText.text, 23);
		}
	}
		
}
