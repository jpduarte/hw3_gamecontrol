using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour {

	public Text highScoreText;

	// Use this for initialization
	void Start () {

		highScoreText.text = "High Score: " + PlayerPrefs.GetString("Champ") + " " + (int)PlayerPrefs.GetFloat ("HighScore");
	
	}



	public void ToMenu() {

		SceneManager.LoadScene ("EndlessRunner");

	}
}
