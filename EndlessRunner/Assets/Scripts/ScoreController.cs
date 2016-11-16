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
	public MenuController deathMenu;

	// Use this for initialization
	void Start () {
	

	}
	
	// Update is called once per frame
	void Update () {

		if (isDead) {
			return;
		}

		if (score >= scoreToNextLevel) {
			LevelUp ();
		}

		//score += Time.deltaTime;
		score = GetComponent<PlayerController>().GetScore();
		scoreText.text = "Score: " + ((int)score).ToString ();
	}

	void LevelUp() {

		if (difficultyLevel == maxDifficultyLevel)
			return;

		scoreToNextLevel *= 2;

		difficultyLevel++;

		GetComponent<PlayerController>().SetSpeed (difficultyLevel);
	}

	public void onDeath() {
		
		isDead = true;
		if (score > PlayerPrefs.GetFloat ("HighScore"))
			PlayerPrefs.SetFloat ("HighScore", score);
		deathMenu.ToggleEndMenu (score);
	}
}
