using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenu : Photon.MonoBehaviour {

	public GameObject nameInput;

	void Awake () 
	{
		nameInput.GetComponent<Text>().text = PlayerPrefs.GetString("Player Name");
		if (nameInput.GetComponent<Text>().text == "")
			nameInput.GetComponent<Text>().text = string.Format("Player{0}",Random.Range(0,999));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
