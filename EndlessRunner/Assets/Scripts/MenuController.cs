using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

	public Text scoreText;

	// Use this for initialization
	void Start () {

		gameObject.SetActive (false);
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ToggleEndMenu(float score) {
		gameObject.SetActive (true);
		scoreText.text = "Score: " + ((int)score).ToString();
		
	}

	public void Restart() {
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}

	public void ToMenu() {
		SceneManager.LoadScene ("Menu");
	}
}
