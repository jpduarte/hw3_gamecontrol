using UnityEngine;
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

		_inputController.Begin("192.168.2.4", 23);

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


		float animateHorizontal = 0;
		float moveHorizontal = 0;

		if (_inputController.connection_status)
		{
			//print(_inputController.stringtoprint);

			string[] move = _inputController.stringtoprint.Split(',');
			//string[] mat_data_string = ;
			//Array.Copy(move, 2, mat_data_string, 0, move.Length - 2);
			//int[] mat_data = Array.ConvertAll(mat_data_string.Select(t => t.string).ToArray(), System.Convert.ToInt32);
			//int[] mat_data = Array.ConvertAll(move, int.Parse);
			//int[] mat_data = Array.ConvertAll<string, int>(mat_data_string, int.Parse);

			if (move[1] != null) {
				//print ("Accel Data: "+ move[1]);
				for (int i = 0; i < move.Length - 3 ; i++) {
					mat_data [i] = float.Parse (move [i+2]);
				}
				animateHorizontal = float.Parse (move [1]);
				moveHorizontal = float.Parse (move [1]);
				//animateHorizontal = Input.GetAxis("Horizontal");
				//moveHorizontal = Input.GetAxisRaw("Horizontal");
				//print ("Move Horizontal " + moveHorizontal);
			}
		} else
		{
			animateHorizontal = Input.GetAxis("Horizontal");
			moveHorizontal = Input.GetAxisRaw("Horizontal");
		}

		//Get inputs here
		/*
		float animateHorizontal = Input.GetAxis ("Horizontal");
		float moveHorizontal = Input.GetAxisRaw ("Horizontal");
		*/


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
		bool isFeet = true;
		bool isHands = true;

		//Don't get into pushup position until player gets off board
		//if (!isFeet) {
		if(true) {
			isIdleToPushUp = true;
		}

		//Don't start pushup until hands on board
		if (isIdleToPushUp && isHands) {

			//Check if pushup detected
			pushUpDetected = pressureMatController.GetPushUpDetected ();
			if (!pushUpDetected) {
				pushUpDone = true;
			}
			if (pushUpDetected && pushUpDone) {
				pushUpCount++;
				exerciseText.text = pushUpCount.ToString() + " Out of "  + pushUpNumber.ToString() + " PushUps!";
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
		

	public bool GetPushUp() {
		return isIdleToPushUp;
	}

	public void StartSquat() {
		isIdle = true;
		SquatState = true;
	}

	public void SquatAnimation() {
		bool isFeet = true;

		// Don't start squat animation until both feet on
		if (isFeet) {
			squatDetected = pressureMatController.GetPushUpDetected ();
			if (!squatDetected) {
				squatDone = true;
			}
			if (squatDetected && squatDone) {
				squatCount++;
				exerciseText.text = squatCount.ToString() + " Out of "  + squatNumber.ToString() + " Squats!";
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

	public void NewIPAddress() {
		if (IPText != null) {
			print ("IP Text" + IPText.text);
			_inputController.Begin (IPText.text, 23);
		}
	}
		
}
