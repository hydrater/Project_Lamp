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
        loadingScreen.SetActive(false);
        foreach (GameObject b in onScreenControls)
            b.SetActive(true);
        UpdatePlayers();

        if (players.Length < 3)
            PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity, 0);
        if (players.Length > 1)
        {
            if (!water.canMove)
                photonView.RPC("GameStart", PhotonTargets.All, photonView.viewID);
            else
                water.canMove = true;
        }
        //Player spawning with water issue
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
