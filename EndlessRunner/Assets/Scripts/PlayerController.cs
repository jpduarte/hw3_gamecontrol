using UnityEngine;
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
	//private Transform hips;
	private Light powerUpLight;
	private int pushUpNumber = 5;
	private int pushUpCount = 0;
	private int detectCount = 0;

	public float vSpeed = 0.5f;
	public float hSpeed = 5.0f;
	public float verticalVelocity = 0.0f;
	public float gravity = 12.0f;
	public Text pushUpText;
	public CameraController cameraController;
	public ScoreController scoreController;
	public PressureMatController pressureMatController;
	public float powerUpTimeLimit = 10.0f;

	private bool activeMagnet = false;
	private bool activeStar = false;
	private bool pushUpDetected = false;
	//define Input Controller class
	public InputController _inputController;



	// Use this for initialization
	void Start () {

		powerUpLight = transform.FindChild ("PowerUpLight").GetComponent<Light>();

			//(Behaviour)GetComponent("Halo");
		powerUpLight.enabled = false;

		//Get Character Controller Component
		//cameraController = GetComponent<CameraController> ();
		controller = GetComponent<CharacterController> ();
		anim = GetComponent<Animator> ();
		pushUpText.text = "";
		timeStart = Time.time;

		_inputController = new InputController();
		_inputController.Begin("172.20.10.11", 23);

	}
	
	// Update is called once per frame
	void Update () {

		timeAfterPushup = Time.time;

		if (((timeAfterPushup - timeFinishPushUp) > 1.0f) && !isIdleToPushUp) {
			pushUpText.text = "";
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

		if (isIdleToPushUp) {

			/*
			while(vSpeed > 0.0f) {
				vSpeed -= vSpeed/5.0f * Time.deltaTime ;
				if (vSpeed < 0.0f)
					vSpeed = 0.0f;
			}
			*/
			vStop = 0.0f;
			pushUpDetected = pressureMatController.GetPushUpDetected ();
			if (pushUpDetected) {
				detectCount++;
				if (detectCount > 6) {
					pushUpCount++;
					detectCount = 0;
				}
				pushUpText.text = pushUpCount.ToString() + " Out of "  + pushUpNumber.ToString() + " Push UPs!";
				pushUpDetected = false;
			}

			//transform.Rot
			print ("PushUp");
		} else {
			vStop = 1.0f;
		}


		float animateHorizontal;
		float moveHorizontal;

		if (_inputController.connection_status)
		{
			print(_inputController.stringtoprint);
			string[] move = _inputController.stringtoprint.Split(',');
			animateHorizontal = float.Parse(move[0]);
			moveHorizontal = float.Parse(move[0]);
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
		Animate (animateHorizontal,isIdleToPushUp,isPushUp,isPushUpToIdle);




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
		//print ("SetPushup");
		isIdleToPushUp = true;
		pushUpText.text = pushUpCount.ToString() + " Out of "  + pushUpNumber.ToString() + " Push UPs!";

	}

	public void ContinuePushUp() {
		isPushUp = true;

	}

	public void PushUpCount () {
		//pushUpCount++;
		//pushUpText.text = pushUpCount.ToString() + " Out of "  + pushUpNumber.ToString() + " Push UPs!";
	}

	public void FinishPushUp() {
		if (pushUpCount >= pushUpNumber-1) {
			isPushUpToIdle = true;
		}

		
	}

	public void DonePushUp() {
			isIdleToPushUp = false;
			isPushUp = false;
			isPushUpToIdle = false;
			pushUpCount = 0;
			//float offCenter = transform.localEulerAngles.y;
			pushUpText.text = "Good Job!";
			transform.rotation = Quaternion.identity;
			timeFinishPushUp = Time.time;
	}
		

	public bool GetPushUp() {
		return isIdleToPushUp;
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
}
