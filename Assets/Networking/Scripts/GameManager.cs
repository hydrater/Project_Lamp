using UnityEngine;
using System.Collections;

public class GameManager : Photon.MonoBehaviour {

	const string VERSION = "Prototype";
	
	void Awake()
	{
		if (!PhotonNetwork.connected)
			PhotonNetwork.ConnectUsingSettings(VERSION);

		RoomOptions roomOptions = new RoomOptions();
		roomOptions.MaxPlayers = 20;
		PhotonNetwork.JoinOrCreateRoom ("Only", roomOptions, TypedLobby.Default);
	}

	void Start () {
	
	}
	
	void Update () {
	
	}

	[PunRPC]
	void gameStart()
	{
		//PhotonNetwork.Instantiate("Player", spawnPoint.position, spawnPoint.rotation, 0);
	}
}
