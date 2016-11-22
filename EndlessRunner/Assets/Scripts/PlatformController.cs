using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlatformController : MonoBehaviour {

	public GameObject[] platformPrefabs;
	public GameObject tokenPrefab;
	public GameObject magnetPowerUpPrefab;
	public GameObject starPowerUpPrefab;
	public GameObject player;
	public int nPlatformsOnScreen = 7;
	public int nTokenSpawn = 4;
	public float tokenSpacing = 2.0f;

	private Transform playerTransform;

	public float platformLength = 8f;
	private float spawnZ = -7.6f;

	private float safeZone = 15.0f;

	private List<GameObject> activePlatforms;
	private List<GameObject> activeTokens;
	public GameObject magnetPowerUp;
	public GameObject starPowerUp;

	private float tokenBound = 0;
	private float score = 0.0f;


	private int lastPrefabIndex = 0;

	private bool activeMagnet = false;
	private bool magnetSpawned = false;
	private float TokenSpeed = 8.0f;

	private bool activeStar = false;
	private bool starSpawned = false;

	private float powerUpStartTime = 0.0f;
	private float powerUpTimeLimit = 10.0f;


	private PlayerController playerController;

	// Use this for initialization
	private void Start () {

		//Initialize list of platforms
		activePlatforms = new List<GameObject>();
		//Initialize list of Tokens
		activeTokens = new List<GameObject>();

		//Find the player's transform
		playerTransform = player.transform;
		playerController = player.GetComponent<PlayerController> ();


		//Spawn the beginning platforms and tokens
		for(int i = 0; i < nPlatformsOnScreen; i++) {
			if (i < 6) {
				SpawnPlatform (0);
			} else {
				SpawnPlatform ();
				SpawnToken ();
			}
		}
	}
	
	// Update is called once per frame
	private void Update () {


		//Spawn a platform with tokens if the player's z position exceeds 1 platform length by checking
		//	the spawn point for platforms minus the position of the last platform
		if (playerTransform.position.z - safeZone > (spawnZ - nPlatformsOnScreen * platformLength)) {
			//1 in 4 chance of spawning tokens
			SpawnPlatform ();

			if (Random.Range (1, 4) == 1) {
				SpawnToken ();
			}
			DeletePlatform ();
		}


		if (activeMagnet) {
			TokenMagnetActive ();
		}

		if ((Time.time - powerUpStartTime) > powerUpTimeLimit) {
			activeMagnet = false;
			activeStar = false;
			playerController.MagnetPowerUp (activeMagnet);
			playerController.StarPowerUp (activeStar);
		}

		TokenCollision ();
		if (magnetSpawned) {
			MagnetCollision ();
		}
		if (starSpawned) {
			StarCollision ();
		}



		/*
		//Check if token was already removed and if it was delete it from list
		int nullToken = activeTokens.FindIndex (tokenSearch => tokenSearch == null);
		if (nullToken > -1) {
			TokenCleanup (nullToken);
		} */

		//Remove Token if player has passed it
		int activeTokenIndex = activeTokens.FindIndex (tokenSearch => tokenSearch.transform.position.z < playerTransform.position.z - safeZone);
		//print ("Token Index: " + activeTokenIndex);
		if(activeTokenIndex > -1) {
			//print ("Last Token Position: " + activeTokens [activeTokenIndex].transform.position.z);
			DeleteToken (activeTokenIndex);
		}
			
		if (magnetPowerUp != null) {
			//Remove Token if player has passed it
			if (magnetPowerUp.transform.position.z < playerTransform.position.z - safeZone) {
				//print ("Last Token Position: " + activeTokens [activeTokenIndex].transform.position.z);
				Destroy(magnetPowerUp);
				magnetSpawned = false;
			}
		}

		if (starPowerUp != null) {
			//Remove Token if player has passed it
			if (starPowerUp.transform.position.z < playerTransform.position.z - safeZone) {
				//print ("Last Token Position: " + activeTokens [activeTokenIndex].transform.position.z);
				Destroy(starPowerUp);
				starSpawned = false;
			}
		}



	}

	private void SpawnPlatform(int prefabIndex = -1) {

		GameObject platform;

		if (prefabIndex == -1) {
			//Called in main loop
			//platform = Instantiate (platformPrefabs [RandomPrefabIndex ()]) as GameObject;
			platform = Instantiate (platformPrefabs [Random.Range(0,platformPrefabs.Length)]) as GameObject;
		} else {
			//Called at startup to make sure no hazards for first few platforms
			platform = Instantiate (platformPrefabs [prefabIndex]) as GameObject;
		}
		//Make sure all the platforms have same parent
		tokenBound = Mathf.Abs(platform.GetComponentsInChildren<Collider>()[0].bounds.extents.x);
		print("Token Bound: " + tokenBound);
		platform.transform.SetParent (transform);
		//Place the platfor at the new spawn point
		platform.transform.position = Vector3.forward * spawnZ;
		//Update the spawn point to be a platform length away
		spawnZ += platformLength;
		//Add the platform to the active list of platforms to keep track
		activePlatforms.Add (platform);
		
	}

	private void DeletePlatform() {
		//Delete the oldest platform we shouldn't see this one
		Destroy (activePlatforms [0]);
		//Remove it from the list
		activePlatforms.RemoveAt (0);
	}

	//Makes sure no Prefab is repeated
	private int RandomPrefabIndex() {

		if (platformPrefabs.Length <= 1) {
			return 0;
		}

		int randomIndex = lastPrefabIndex;
		while (randomIndex == lastPrefabIndex) {
			randomIndex = Random.Range (0, platformPrefabs.Length);
		}

		lastPrefabIndex = randomIndex;
		return randomIndex;
	}

	private void TokenCollision() {
		//Remove Token if player has passed it
		List<int> results = Enumerable.Range(0, activeTokens.Count) .Where(i => Vector3.Distance(activeTokens[i].transform.position, playerTransform.position) < 1.25f)
			.ToList();
		for (int i = 0; i < results.Count; i++) {
			if (activeTokens [results[i]] != null) {
				Destroy (activeTokens [results[i]]);
				score++;
			}
			activeTokens.RemoveAt (results[i]);
		}
	}


	private void TokenMagnetActive() {
		Vector3 xzMagnet;

		List<int> results = Enumerable.Range(0, activeTokens.Count) .Where(i => Vector3.Distance(activeTokens[i].transform.position, playerTransform.position) < 20.0f)
			.ToList();
		for (int i = 0; i < results.Count; i++) {
			if (activeTokens[results[i]] != null) {
				xzMagnet = Vector3.MoveTowards (activeTokens[results[i]].transform.position, playerTransform.position, Time.deltaTime * TokenSpeed);
				activeTokens[results[i]].transform.position = new Vector3 (xzMagnet.x, activeTokens[results[i]].transform.position.y, xzMagnet.z);
			}
		}
	}

	private void MagnetCollision() {

		if(Vector3.Distance(magnetPowerUp.transform.position, playerTransform.position) < 1.25f) {
			Destroy (magnetPowerUp);
			activeMagnet = true;
			activeStar = false;
			magnetSpawned = false;
			powerUpStartTime = Time.time;
			playerController.StarPowerUp (activeStar);
			playerController.MagnetPowerUp (activeMagnet);
		}
	}

	private void StarCollision() {

		if(Vector3.Distance(starPowerUp.transform.position, playerTransform.position) < 1.25f) {
			Destroy (starPowerUp);
			activeStar = true;
			activeMagnet = false;
			starSpawned = false;
			powerUpStartTime = Time.time;
			playerController.StarPowerUp (activeStar);
			playerController.MagnetPowerUp (activeMagnet);

		}
	}

	private void SpawnToken () {
		
		GameObject token;
		float tokenX = ((Random.value * (tokenBound - 0.5f) * ((Random.Range (0, 2) * 2) - 1)));

		if ((Random.Range (1, 4) == 1) && (magnetSpawned == false)) {
			magnetPowerUp = Instantiate (magnetPowerUpPrefab) as GameObject;
			magnetPowerUp.transform.SetParent (transform);
			//print ("p or n: " + (Random.Range (0, 2)));
			magnetPowerUp.transform.position = new Vector3 (tokenX, 1.0f, (spawnZ - 1.5f*platformLength));
			magnetSpawned = true;
		} else if ((Random.Range (1, 4) == 1) && (starSpawned == false)) {
			starPowerUp = Instantiate (starPowerUpPrefab) as GameObject;
			starPowerUp.transform.SetParent (transform);
			//print ("p or n: " + (Random.Range (0, 2)));
			starPowerUp.transform.position = new Vector3 (tokenX, 1.0f, (spawnZ - 1.5f*platformLength));
			starSpawned = true;
		} else {
			// Spawn n tokens each tokenSpacing away from each other
			for (int i = 0; i < nTokenSpawn; i++) {
				token = Instantiate (tokenPrefab) as GameObject;
				token.transform.SetParent (transform);
				//print ("p or n: " + (Random.Range (0, 2)));
				token.transform.position = new Vector3 (tokenX, 1.0f, (spawnZ - platformLength) - i * tokenSpacing);

				//spawnZ += platformLength;
				activeTokens.Add (token);
			}
		}
	}
		

	private void TokenCleanup (int nullToken) {
		activeTokens.RemoveAt (nullToken);

	}



	private void DeleteToken(int activeTokenIndex) {

		//for(int i = 0; i < nTokenSpawn-1; i++) {
		if(activeTokens[activeTokenIndex] != null){
			Destroy (activeTokens [activeTokenIndex]);
		}
		activeTokens.RemoveAt (activeTokenIndex);

		//}
		//activeTokens.RemoveRange (0,nTokenSpawn-1);
	}

	public int GetTokenListIndex(GameObject tokenToDelete) { 
		return activeTokens.IndexOf (tokenToDelete);
	}

	public void UpdateTokenList(int deletedTokenIndex) { 
		activeTokens.RemoveAt (deletedTokenIndex);
	}

	public float GetScore() {
		return score;
	}
}
