using UnityEngine;
using System.Collections;

public class GameManager : Photon.MonoBehaviour {

	const string VERSION = "Prototype";
	public GameObject loadingScreen;
	
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
		gameStart();
	}

	void Start () 
	{
	}
	
	void Update () {
	
	}

	[PunRPC]
	void gameStart()
	{
		PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity, 0);
	}
}
