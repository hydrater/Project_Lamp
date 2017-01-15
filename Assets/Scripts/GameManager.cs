using UnityEngine;
using System.Collections;

public class GameManager : Photon.MonoBehaviour
{

    const string VERSION = "Prototype";
    public GameObject loadingScreen, win, lose;
	public Water water;
    public GameObject[] onScreenControls;
    GameObject[] players;

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
        foreach (GameObject b in onScreenControls)
            b.SetActive(true);
		
        StartCoroutine(FinishedLoading());
        //Player spawning with water issue
    }

	void OnPhotonPlayerDisconnected(PhotonPlayer other)
 	{
    	//Check whether it is the last player and assign winner if so	
 	}

	IEnumerator FinishedLoading()
	{
//		if (!photonView.isMine)
//		{
//			Debug.Log("please fix");
//		}
		yield return null;

		UpdatePlayers();

		Debug.Log("test start" + players.Length);


		if (players.Length < 3)
        {
			Debug.Log(players.Length);
			PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity, 0);
        }

		UpdatePlayers();

		if (players.Length > 1)
        {
        	Debug.Log("reached2");
            if (!water.canMove)
                photonView.RPC("GameStart", PhotonTargets.All);
            else
                water.canMove = true;
        }

		loadingScreen.SetActive(false);
		Debug.Log("test success" + players.Length);

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
        GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().Respawn();
    }

    public bool CheckForWinner()
    {
        UpdatePlayers();
        int deathCount = 0;

        foreach (GameObject p in players)
            if (p.GetComponent<Player>().dead)
                deathCount++;

        return (deathCount == players.Length && deathCount != 1);
        //Timer, game start
    }

	public void NewGame()
	{
		win.SetActive(true);
        water.canMove = false;
	}

 	IEnumerator NewGameTimer()
 	{
		yield return new WaitForSeconds(3);
		win.SetActive(false);
		photonView.RPC("GameStart", PhotonTargets.All);
 	} 
}