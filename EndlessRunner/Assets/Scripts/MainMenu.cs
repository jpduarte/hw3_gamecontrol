﻿using UnityEngine;
using System.Collections;
//using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour {

	public Text highScoreText;

	// Use this for initialization
	void Start () {

		highScoreText.text = "High Score: " + (int)PlayerPrefs.GetFloat ("HighScore");
	
	}



	public void ToMenu() {
    UnityEngine.SceneManagement.SceneManager.LoadScene("EndlessRunner");
		//SceneManager.LoadScene ("EndlessRunner");

	}
}
