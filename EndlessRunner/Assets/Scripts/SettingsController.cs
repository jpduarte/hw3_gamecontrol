using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour {

	private InputField IPField;
	private bool settingsActive = false;
	// Use this for initialization

	void Start () {
		gameObject.SetActive (false);

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SettingsMenu () {
		if (!settingsActive)
			gameObject.SetActive (true);
		else {
			gameObject.SetActive (false);
		}
		settingsActive = !settingsActive;
	}
}
