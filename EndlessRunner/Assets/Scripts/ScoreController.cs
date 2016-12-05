using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour {

	private float score = 0.0f;

	private int difficultyLevel = 1;
	private int maxDifficultyLevel = 10;
	private int scoreToNextLevel = 10;
	private bool isDead = false;

	public Text scoreText;
	public Text ChampText;
	public Text highScoreText;
	public MenuController deathMenu;
	public Transform player;
	public PlatformController platformController;

	// Use this for initialization
	void Start () {
		//PlayerPrefs.SetFloat ("HighScore", 0.0f);
		DispHighScore ();
	}
	
	// Update is called once per frame
	void Update () {

		if (isDead) {
			return;
		}

		/*if (score >= scoreToNextLevel) {
			LevelUp ();
		}*/

		//score += Time.deltaTime;
		score = platformController.GetScore();
		scoreText.text = "Score: " + ((int)score).ToString ();
		Faster ();
	}

	void Faster() {
		player.GetComponent<PlayerController>().SetSpeed (score);
	}

	void LevelUp() {

		if (difficultyLevel == maxDifficultyLevel)
			return;

		scoreToNextLevel *= 2;

		difficultyLevel++;

		player.GetComponent<PlayerController>().SetSpeed (difficultyLevel);
	}

	public void onDeath() {
		
		isDead = true;
		if (score > PlayerPrefs.GetFloat ("HighScore")) {
			PlayerPrefs.SetFloat ("HighScore", score);
			deathMenu.ToggleHighScore ();
		}
		deathMenu.ToggleEndMenu (score);
	}

	public void NewChamp() {
		PlayerPrefs.SetString ("Champ", ChampText.text);
	}

	void DispHighScore() {
		highScoreText.text = ("High Score: " + PlayerPrefs.GetString ("Champ") + " " + PlayerPrefs.GetFloat ("HighScore").ToString ());
	}
}
