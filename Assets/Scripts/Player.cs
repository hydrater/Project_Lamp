﻿using UnityEngine;
using System.Collections;

public class Player : Photon.MonoBehaviour {

	[HideInInspector]
	public Rigidbody2D rb2d;

	[HideInInspector]
	public BoxCollider2D bc2d;

	[HideInInspector]
	public Animator anim;

	public Transform groundCheck;

	[SerializeField]
	private float jumpForce = 1;

	private bool grounded = false;

	private bool jump = false;

	[SerializeField]
	private float movementSpeed;

	private bool facingRight = true;

	private bool attack = true;

	[SerializeField]
	private float attackForce = 1;

	Vector3 realPosition;

	// Use this for initialization
	void Start () {
		rb2d = GetComponent<Rigidbody2D> ();
		bc2d = GetComponent<BoxCollider2D> ();
		anim = GetComponent<Animator> ();
		realPosition = transform.position;
		if (!photonView.isMine) transform.GetChild(1).gameObject.SetActive(false);
	}

	void FixedUpdate()
	{
		if (photonView.isMine)
		{
			// updates the grounded value
			grounded = isGrounded ();

			// Handles the jump 
			if (Input.GetKey (KeyCode.Space) && grounded && !jump) {
				rb2d.AddForce (Vector2.up * jumpForce);
				jump = true;
			}

			// Handles the movement for left and right
			float horizontal = Input.GetAxis ("Horizontal");
			transform.position += new Vector3(horizontal * movementSpeed * Time.deltaTime, 0, 0);

			// We do not want the player head to collide with the platform above
			if (rb2d.velocity.y > 0) {
				bc2d.enabled = false;
			} else {
				bc2d.enabled = true;
			}

			// Sprite flipping
			if (horizontal > 0 && !facingRight) {
				Flip ();
			} else if (horizontal < 0 && facingRight) {
				Flip ();
			}
		}
		else
		{
			transform.position = Vector3.Lerp(transform.position, realPosition, 0.1f);
		}

	}

	bool isGrounded()
	{
		if (Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Platform")))
		{
			//anim.ResetTrigger("Jump");
			//anim.SetBool("Land", false);
			jump = false;
			return true;
		}
		return false;
	}

	// Flip the player sprite
	void Flip()
	{
		facingRight = !facingRight;

		Vector3 scale = transform.localScale;
		scale.x *= -1;

		transform.localScale = scale;
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		// If the player collided with another object and is attacking, knock the other player back
		if (anim != null && attack && other.gameObject.tag == "Player") {
			Debug.Log ("Attacking");
			//anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack")
			Rigidbody2D otherRb2d = other.gameObject.GetComponent<Rigidbody2D> ();
			Vector2 direction = (other.transform.position - transform.position).normalized;
			otherRb2d.AddForce (direction * attackForce);
		}
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