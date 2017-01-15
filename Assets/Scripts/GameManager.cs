using UnityEngine;
using System.Collections;

public class GameManager : Photon.MonoBehaviour
{

    const string VERSION = "Prototype";
    public GameObject loadingScreen, win, lose;
	public Water water;
    public GameObject[] onScreenControls;
    GameObject[] players;
    public Player myPlayer;

    void Awake()
    {
        if (!PhotonNetwork.connected)
            PhotonNetwork.ConnectUsingSettings(VERSION);
        PhotonNetwork.playerName = PlayerPrefs.GetString("Player Name");
    }

    void OnJoinedLobby()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;
        PhotonNetwork.JoinOrCreateRoom("Only", roomOptions, TypedLobby.Default);
    }

    void OnJoinedRoom()
    {
    	Debug.Log("Player has joined!");
        foreach (GameObject b in onScreenControls)
            b.SetActive(true);
		
        StartCoroutine(FinishedLoading());
        //Player spawning with water issue
    }

	

	IEnumerator FinishedLoading()
	{
//		if (!photonView.isMine)
//		{
//			Debug.Log("please fix");
//		}
		yield return null;

		UpdatePlayers();

		if (players.Length < 3)
        {
			Debug.Log(players.Length);
			PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity, 0);
        }

		UpdatePlayers();

		if (players.Length > 1)
        {
            if (!water.canMove)
                photonView.RPC("GameStart", PhotonTargets.All);
            else
                water.canMove = true;
        }

		loadingScreen.SetActive(false);
	}

    private void UpdatePlayers()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    void Start()
    {
    }

    void Update()
    {
    }

    [PunRPC]
    void GameStart()
    {
        //Snap players to start location
        if (PhotonNetwork.isMasterClient)
            water.transform.position = new Vector2(0.3f, -74);
        water.canMove = true;
        UpdatePlayers();
        myPlayer.Respawn();
    }

    public bool CheckForWinner()
    {
        UpdatePlayers();
        int deathCount = 0;

        foreach (GameObject p in players)
            if (p.GetComponent<Player>().dead)
                deathCount++;

        return (deathCount == players.Length && deathCount != 1);
    }

	public void NewGame()
	{
		win.SetActive(true);
        water.canMove = false;
		StartCoroutine(NewGameTimer());
	}

 	IEnumerator NewGameTimer()
 	{
		yield return new WaitForSeconds(3);
		win.SetActive(false);
		photonView.RPC("GameStart", PhotonTargets.All);
 	}
}