using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformController : MonoBehaviour {

	public GameObject[] platformPrefabs;
	public GameObject tokenPrefab;
	public GameObject magnetPowerUpPrefab;
	public int nPlatformsOnScreen = 7;
	public int nTokenSpawn = 4;
	public float tokenSpacing = 2.0f;

	private Transform playerTransform;

	public float platformLength = 8f;
	private float spawnZ = -7.6f;

	private float safeZone = 15.0f;

	private List<GameObject> activePlatforms;
	private List<GameObject> activeTokens;
	GameObject magnetPowerUp;

	private float tokenBound = 0;


	private int lastPrefabIndex = 0;

	private bool activeMagnet = false;
	private bool magnetTurn = false;


	// Use this for initialization
	private void Start () {

		//Initialize list of platforms
		activePlatforms = new List<GameObject>();
		//Initialize list of Tokens
		activeTokens = new List<GameObject>();

		//Find the player's transform
		playerTransform = GameObject.FindGameObjectWithTag ("Player").transform;

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
			//Only 1 out of ten chance of getting a magnet powerup
			// and only when there isn't already a magnet powerup out there
			if (Random.Range (1, 4) == 1 && (!activeMagnet || magnetPowerUp == null)) {
				SpawnMagnetPowerUp ();
				magnetTurn = true;
			}
			if (Random.Range (1, 4) == 1 && !magnetTurn) {
				SpawnToken ();
			}
			DeletePlatform ();

			magnetTurn = false;
		}
		//Check if token was already removed and if it was delete it from list
		int nullToken = activeTokens.FindIndex (tokenSearch => tokenSearch == null);
		if (nullToken > -1) {
			TokenCleanup (nullToken);
		}

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
				activeMagnet = false;
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

	private void SpawnToken () {
		
		GameObject token;
		float tokenX = ((Random.value * (tokenBound - 0.5f) * ((Random.Range (0, 2) * 2) - 1)));
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

	private void SpawnMagnetPowerUp () {


		float tokenX = ((Random.value * (tokenBound - 0.5f) * ((Random.Range (0, 2) * 2) - 1)));

		// Spawn Magnet PowerUp
		magnetPowerUp = Instantiate (magnetPowerUpPrefab) as GameObject;
		magnetPowerUp.transform.SetParent (transform);
		magnetPowerUp.transform.position = new Vector3 (tokenX, 1.0f, (spawnZ - platformLength/2) );

		activeMagnet = true;
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
}
