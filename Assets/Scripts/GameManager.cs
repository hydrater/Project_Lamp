using UnityEngine;
using System.Collections;

public class GameManager : Photon.MonoBehaviour {

	const string VERSION = "Prototype";
	public GameObject loadingScreen, win, lose, water;
	
	void Awake()
	{
		if (!PhotonNetwork.connected)
			PhotonNetwork.ConnectUsingSettings(VERSION);
			if (PhotonNetwork.offlineMode)
			{
				OnJoinedRoom();
				Instantiate(Resources.Load("Player"), Vector3.zero, Quaternion.identity);
			}
	}

	void OnJoinedLobby ()
	{
		RoomOptions roomOptions = new RoomOptions();
		roomOptions.MaxPlayers = 20;
		PhotonNetwork.JoinOrCreateRoom ("Only", roomOptions, TypedLobby.Default);
	}

	void OnJoinedRoom()
	{
		loadingScreen.SetActive(false);
		GameStart();
	}

	void Start () 
	{
	}
	
	void Update () {
	
	}

	[PunRPC]
	void GameStart()
	{
		PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity, 0);
	}

	public void CheckForWinner()
	{
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
		int deathCount = 0;

		foreach(GameObject p in players)
			if (p.GetComponent<Player>().dead)
				deathCount++;

		if (deathCount == players.Length-1)
		{
			win.SetActive(true);
			water.SetActive(false);
		}
	}
}
