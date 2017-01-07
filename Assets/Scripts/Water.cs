using UnityEngine;
using System.Collections;

public class Water : MonoBehaviour {

	[SerializeField]
	float movementSpeed = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.position += Vector3.up * movementSpeed * Time.deltaTime;
	}
}
