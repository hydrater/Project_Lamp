using UnityEngine;
using System.Collections;

public class GameManager : Photon.MonoBehaviour {

	const string VERSION = "Prototype";
	public GameObject loadingScreen, win, lose, water;
	GameObject[] players;
	
	void Awake()
	{
		if (!PhotonNetwork.connected)
			PhotonNetwork.ConnectUsingSettings(VERSION);
//			if (PhotonNetwork.offlineMode)
//			{
//				OnJoinedRoom();
//				Instantiate(Resources.Load("Player"), Vector3.zero, Quaternion.identity);
//			}
		PhotonNetwork.playerName = PlayerPrefs.GetString("Player Name");

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
		UpdatePlayers();

		if (players.Length < 3)
			PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity, 0);
		if (players.Length > 1)
		{
			if (!water.GetActive)
				photonView.RPC ("GameStart", PhotonTargets.All, photonView.viewID);
			else
				water.SetActive(true);
		}
		//Player spawning with water issue
	}

	private void UpdatePlayers()
	{
		players = GameObject.FindGameObjectsWithTag ("Player");
	}

	void Start () 
	{
	}
	
	void Update () {
	
	}

	[PunRPC]
	void GameStart()
	{
		if (PhotonNetwork.isMasterClient)
			water.transform.position = new Vector2(0.3f, -74);
		water.SetActive(true);
		UpdatePlayers();
		foreach (GameObject p in players)
			p.GetComponent<Player>().Respawn();
	}

	public void CheckForWinner()
	{
		UpdatePlayers();
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
