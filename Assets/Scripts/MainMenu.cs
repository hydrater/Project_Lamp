using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenu : Photon.MonoBehaviour {

	public InputField nameInput;

	void Awake () 
	{
		nameInput.text = PlayerPrefs.GetString("Player Name");
		if (nameInput.text == "")
			nameInput.text = string.Format("Player{0}",Random.Range(0,999));
	}

	void Login()
	{
		PlayerPrefs.SetString("Player Name", nameInput.text);
	}


	// Update is called once per frame
	void Update () {
	
	}
}
