using UnityEngine;
using System.Collections;

public class Water : Photon.MonoBehaviour {

	[SerializeField]
	float movementSpeed = 0;

	Vector3 realPosition;
	// Use this for initialization

	void Awake()
	{
		realPosition = transform.position;
	}

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(PhotonNetwork.isMasterClient)
			transform.position += Vector3.up * movementSpeed * Time.deltaTime;
		else
			transform.position = Vector3.Lerp(transform.position, realPosition, 0.1f);
	}

	public void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext(transform.position);
		}
		else
		{
			realPosition = (Vector3)stream.ReceiveNext();
		}
	}
}
