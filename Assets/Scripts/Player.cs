using UnityEngine;
using System.Collections;

public class Player : Photon.MonoBehaviour {

	public enum SPECTATORMODE
	{
		TARGET,
		AUTO,
		MANUAL,
	}

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

	//private bool jump = false;

	[SerializeField]
	private float movementSpeed;

	private bool facingRight = true;

	private bool attack = false;

	private bool attacked = false;

	[SerializeField]
	private float attackForce = 1;

	private bool dead = false;

	private Vector3 target = Vector3.zero;

	private int targetNo = 0;

	private Vector3 deadPos = Vector3.zero;

	Vector3 realPosition;

	SPECTATORMODE spectatorMode = SPECTATORMODE.TARGET;

	void Awake() {
		rb2d = GetComponent<Rigidbody2D> ();
		bc2d = GetComponent<BoxCollider2D> ();
		anim = GetComponent<Animator> ();
	}

	// Use this for initialization
	void Start () {
		realPosition = transform.position;
		if (!photonView.isMine) transform.GetChild(1).gameObject.SetActive(false);
	}

	void FixedUpdate ()
	{
		// Reset attacked
		if (anim.GetCurrentAnimatorStateInfo (0).IsName ("player_Idle")) {
			// Reset attack
			anim.SetBool ("attack", false);
			attack = false;

			Debug.Log ("A");

			anim.SetBool ("attacked", false);
			attacked = false;
			anim.SetBool ("falling", false);
		}

		if (photonView.isMine) {
			if (dead) {
				UpdateSpectatorMode ();
				return;
			}

			// updates the grounded value
			grounded = isGrounded ();

			if (anim.GetCurrentAnimatorStateInfo (0).IsName ("player_Dizzy")) {
				return;
			}

			// Handles the jump 
			if (Input.GetKey (KeyCode.Space) && grounded && !anim.GetBool("jump") && anim.GetCurrentAnimatorStateInfo (0).IsTag ("free")) {
				rb2d.AddForce (Vector2.up * jumpForce);
				anim.SetBool ("jump", true);
			}

			// Handles the movement for left and right
			float horizontal = Input.GetAxis ("Horizontal");
			transform.position += new Vector3 (horizontal * movementSpeed * Time.deltaTime, 0, 0);



			// Handles the attack
			if (Input.GetMouseButton (0) || Input.GetKey(KeyCode.C)) {
				if (!attack) {
					anim.SetBool ("attack", true);
					attack = true;
				}
			}

			if (anim.GetCurrentAnimatorStateInfo (0).IsTag ("Attack")) {
				attack = true;
			} else {
				attack = false;
			}

			// We do not want the player head to collide with the platform above
			if (anim.GetBool("jump")) {
				if (rb2d.velocity.y > 0) {
					//bc2d.enabled = false;
					anim.SetBool ("falling", false);
				} else {
					//bc2d.enabled = true;
					anim.SetBool ("falling", true);
				}
			}

			if (horizontal != 0 && !anim.GetBool("jump") && anim.GetCurrentAnimatorStateInfo (0).IsTag ("free")) {
				anim.SetBool ("isMoving", true);
			} else {
				anim.SetBool ("isMoving", false);
			}

			// Sprite flipping
			if ((horizontal > 0 && !facingRight) || (horizontal < 0 && facingRight)) 
			{
				Vector3 scale = transform.localScale;
				scale.x *= -1;

				transform.localScale = scale;
				facingRight = !facingRight;
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
			anim.SetBool("jump", false);
			return true;
		}
		return false;
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		// If the player collided with another object and is attacking, knock the other player back
		if (attack && other.gameObject.tag == "Player") {
			Debug.Log ("Attack");
			//Rigidbody2D otherRb2d = other.gameObject.GetComponent<Rigidbody2D> ();
			//Vector2 direction = (other.transform.position - transform.position).normalized;
			//otherRb2d.AddForce (direction * attackForce);
			//other.gameObject.GetComponent<Player> ().Attacked ();
			if (other.gameObject.GetComponent<Player> ().attack) {
				Vector2 direction = (transform.position - other.gameObject.transform.position).normalized;
				rb2d.AddForce (direction * attackForce);
				anim.SetBool ("attacked", true);
				attacked = true;
			}
		} 
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Water") {
			Debug.Log ("DEATH");
			dead = true;
			deadPos = transform.position;
			rb2d.isKinematic = true;
			bc2d.enabled = false;
			GetComponent<SpriteRenderer> ().enabled = false;
		}
	}

	void UpdateSpectatorMode()
	{
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");

		if (Input.GetKeyDown(KeyCode.Tab)) {
			++targetNo;

			if (targetNo < players.Length) {
				target = players [targetNo].transform.position;
			} else { // wrap back to your dead position
				target = deadPos;
			}

			spectatorMode = SPECTATORMODE.TARGET;
		}

		if (Input.GetKeyDown (KeyCode.D)) {
			spectatorMode = SPECTATORMODE.AUTO;
		}

		if (Input.GetKey (KeyCode.LeftArrow) || Input.GetKey (KeyCode.RightArrow) ||
			Input.GetKey (KeyCode.UpArrow) || Input.GetKey (KeyCode.DownArrow)) {
			spectatorMode = SPECTATORMODE.MANUAL;

			if (Input.GetKey (KeyCode.LeftArrow))
				transform.position -= new Vector3(movementSpeed * Time.deltaTime, 0, 0);
			if (Input.GetKey (KeyCode.RightArrow))
				transform.position += new Vector3(movementSpeed * Time.deltaTime, 0, 0);
			if(Input.GetKey(KeyCode.UpArrow))
				transform.position += new Vector3(0, movementSpeed * Time.deltaTime, 0);
			if (Input.GetKey (KeyCode.DownArrow))
				transform.position -= new Vector3(0, movementSpeed * Time.deltaTime, 0);
		}

		if (spectatorMode != SPECTATORMODE.MANUAL) {
			transform.position = target;
		}
	}

	public void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext(transform.position);
			stream.SendNext(transform.localScale);
			stream.SendNext(anim.GetBool ("isMoving"));
			stream.SendNext(anim.GetBool("jump"));
			stream.SendNext(attack);
			stream.SendNext (attacked);
		}
		else
		{
			realPosition = (Vector3)stream.ReceiveNext();
			transform.localScale = (Vector3)stream.ReceiveNext();
 			anim.SetBool("isMoving", (bool)stream.ReceiveNext());
			anim.SetBool("jump", (bool)stream.ReceiveNext());
			anim.SetBool("attack", (bool)stream.ReceiveNext());
			anim.SetBool ("attacked", (bool)stream.ReceiveNext ());
		}
	}
}
