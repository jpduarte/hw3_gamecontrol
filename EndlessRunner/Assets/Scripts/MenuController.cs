using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

	public Text scoreText;
	public Image backgroudImg;

	private bool isShown = false;
	private GameObject highScoreObject;

	private float transition = 0.0f;
	//private float fadeTime = 2.0f;
	// Use this for initialization
	void Start () {

		transition = 0.0f;
		highScoreObject = GameObject.FindGameObjectWithTag ("HighScore");
		highScoreObject.SetActive (false);
		gameObject.SetActive (false);
	
	}
	
	// Update is called once per frame
	void Update () {
			

		if (!isShown)
			return;

		transition += Time.deltaTime;// * 1/fadeTime;
		backgroudImg.color = Color.Lerp (new Color (0, 0, 0,0), Color.black, transition);
	
	}

	public void ToggleEndMenu(float score) {
		gameObject.SetActive (true);
		scoreText.text = "Score: " + ((int)score).ToString();
		isShown = true;
	}

	public void ToggleHighScore() {
		highScoreObject.SetActive (true);
	}
		

	public void Restart() {
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}

	public void ToMenu() {
		SceneManager.LoadScene ("Menu");
	}
}
