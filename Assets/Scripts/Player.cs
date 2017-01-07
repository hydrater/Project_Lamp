using UnityEngine;
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

	private bool isMoving = false;

	float horizontal = 0;

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
		bool tempDir = facingRight;

		if (photonView.isMine)
		{
			// updates the grounded value
			grounded = isGrounded ();

			// Handles the jump 
			if (Input.GetKey (KeyCode.Space) && grounded && !jump && anim.GetCurrentAnimatorStateInfo(0).IsTag("free")) {
				rb2d.AddForce (Vector2.up * jumpForce);
				jump = true;
				anim.SetBool ("jump", true);
			}

			// Handles the movement for left and right
			horizontal = Input.GetAxis ("Horizontal");
			transform.position += new Vector3(horizontal * movementSpeed * Time.deltaTime, 0, 0);

			anim.SetBool ("attack", false);

			// Handles the attack
			if (Input.GetMouseButton (0)) {
				anim.SetBool ("attack", true);
			}

			if (anim.GetCurrentAnimatorStateInfo (0).IsTag ("Attack")) {
				attack = true;
			} else {
				attack = false;
			}

			// We do not want the player head to collide with the platform above
			if (jump) {
				if (rb2d.velocity.y > 0) {
					//bc2d.enabled = false;
					anim.SetBool ("falling", false);
				} else {
					//bc2d.enabled = true;
					anim.SetBool ("falling", true);
				}
			}

			if (horizontal != 0 && !jump && anim.GetCurrentAnimatorStateInfo(0).IsTag("free")) {
				isMoving = true;
			} else {
				isMoving = false;
			}

			anim.SetBool ("isMoving", isMoving);

			// Reset attacked
			if(anim.GetCurrentAnimatorStateInfo(0).IsName("player_Idle")) {
				anim.SetBool ("attacked", false);
				anim.SetBool ("falling", false);
			}

			if ((horizontal > 0 && !facingRight) || (horizontal < 0 && facingRight)) facingRight = !facingRight;

		}
		else
		{
			transform.position = Vector3.Lerp(transform.position, realPosition, 0.1f);
		}
		// Sprite flipping
		if (tempDir != facingRight)
		{
			Vector3 scale = transform.localScale;
			scale.x *= -1;

			transform.localScale = scale;
		}
			
	}

	bool isGrounded()
	{
		if (Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Platform")))
		{
			//anim.ResetTrigger("Jump");
			//anim.SetBool("Land", false);
			anim.SetBool("jump", false);
			jump = false;
			return true;
		}
		return false;
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		// If the player collided with another object and is attacking, knock the other player back
		if (attack && other.gameObject.tag == "Player") {
			Debug.Log ("Attack");
			//
			Rigidbody2D otherRb2d = other.gameObject.GetComponent<Rigidbody2D> ();
			Vector2 direction = (other.transform.position - transform.position).normalized;
			otherRb2d.AddForce (direction * attackForce);
		} 
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Water") {
			Debug.Log ("DEATH");
		}
	}

	public void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext(transform.position);
			stream.SendNext(isMoving);
			stream.SendNext(jump);
			stream.SendNext(facingRight);
		}
		else
		{
			realPosition = (Vector3)stream.ReceiveNext();
			anim.SetBool("isMoving", (bool)stream.ReceiveNext());
			anim.SetBool("jump", (bool)stream.ReceiveNext());
			facingRight = (bool)stream.ReceiveNext();
		}
	}
}
