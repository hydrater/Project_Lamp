﻿using UnityEngine;
using System.Collections;

public class Player : Photon.MonoBehaviour
{

    public enum SPECTATORMODE
    {
        TARGET,
        MANUAL,
    }

    [HideInInspector]
    public Rigidbody2D rb2d;

    [HideInInspector]
    public BoxCollider2D bc2d;

    [HideInInspector]
    public Animator anim;

    [SerializeField]
    private float jumpForce = 1;

    private bool grounded = false;

    //private bool jump = false;

    [SerializeField]
    private float movementSpeed;

    private bool facingRight = true;

    private bool attack = false;

    [SerializeField]
    private float attackForce = 100;

    public bool dead = false;

    private Vector3 target = Vector3.zero;

    private int targetNo = 0;

    private Vector3 deadPos = Vector3.zero;

    Vector3 realPosition;

    public AudioClip[] splashes;

    SPECTATORMODE spectatorMode = SPECTATORMODE.MANUAL;

    [SerializeField]
    private float attackCooldown = 1;
    private float attackTimer = 1;

    private bool leftButtonDown = false;
    private bool rightButtonDown = false;
    private bool jumpButton = false;
    private bool attackButton = false;

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        bc2d = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        if (!photonView.isMine)
            GetComponent<Rigidbody2D>().isKinematic = true;
        //Destroy (GetComponent<Rigidbody2D>());
    }

    // Use this for initialization
    void Start()
    {
        realPosition = transform.position;
        if (!photonView.isMine) transform.GetChild(0).gameObject.SetActive(false);
    }

    void FixedUpdate()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("player_Dizzy"))
        {
            //anim.ResetTrigger("attacked");
            //transform.GetChild(1).gameObject.SetActive(false);
            //anim.ResetTrigger ("attack");
            anim.ResetTrigger("attacked");
        }

        if (photonView.isMine)
        {
            if (dead)
                return;

            // updates the grounded value
            grounded = isGrounded();

            if (anim.GetCurrentAnimatorStateInfo(0).IsName("player_Dizzy"))
            {
                //transform.GetChild(1).gameObject.SetActive(false);
                
                return;
            }

            // Handles the jump 
            if ((Input.GetKey(KeyCode.Space) || jumpButton) && grounded && anim.GetCurrentAnimatorStateInfo(0).IsTag("free"))
            {
                rb2d.AddForce(Vector2.up * jumpForce);
                anim.SetBool("jump", true);
                attack = false;
                jumpButton = false;
            }

            // Handles the movement for left and right

            // Pc Control
            float horizontal = Input.GetAxis("Horizontal");
            transform.position += new Vector3(horizontal * movementSpeed * Time.deltaTime, 0, 0);

            // Button for phone controls
            if (leftButtonDown)
            {
                transform.position += new Vector3(-1 * movementSpeed * Time.deltaTime, 0, 0);
                horizontal = -1;
            }
            if (rightButtonDown)
            {
                transform.position += new Vector3(movementSpeed * Time.deltaTime, 0, 0);
                horizontal = 1;
            }

            // We do not want the player head to collide with the platform above
            //if (anim.GetBool("jump")) {

            if (rb2d.velocity.y < 0)
            {
                //bc2d.enabled = true;
                anim.SetBool("falling", true);
            }

            if (rb2d.velocity.y > 0 || grounded)
            {
                //bc2d.enabled = false;
                anim.SetBool("falling", false);
            }

            if (horizontal != 0 && !anim.GetBool("jump") && anim.GetCurrentAnimatorStateInfo(0).IsTag("free"))
            {
                anim.SetBool("isMoving", true);
            }
            else
            {
                anim.SetBool("isMoving", false);
            }

            // Sprite flipping
            if ((horizontal > 0 && !facingRight) || (horizontal < 0 && facingRight))
            {
                Vector3 scale = transform.localScale;
                scale.x *= -1;

                transform.localScale = scale;
                facingRight = !facingRight;
            }

            // If grounded and cooldown finish, player is able to attack. Timer is to ensure that player cannot spam attack while grounded
            if (attack)
            {
                attackTimer -= Time.deltaTime;
                if (attackTimer <= 0 && grounded)
                {
                    attack = false;
                    attackTimer = 1;
                }
            }

            // Handles the attack
            if (Input.GetKey(KeyCode.C))
            {
                HandleAttack();
            }
        }
        else
        {
            //transform.position = Vector3.Lerp(transform.position, realPosition, 0.1f);
            //transform.position = realPosition;
            transform.position = Vector3.MoveTowards(transform.position, realPosition, 20.0f * Time.deltaTime);
            if (dead)
                if (GetComponent<SpriteRenderer>().enabled)
                    RIP();
        }
    }

    public void HandleJump()
    {
        jumpButton = true;
    }

    public void HandleAttack()
    {
        if (!attack)
        {
            photonView.RPC("SetAttack", PhotonTargets.All, photonView.viewID);
            attack = true;
            attackTimer = 1;
        }
    }

    public void OnLeftButton()
    {
        leftButtonDown = true;
    }

    public void OffLeftButton()
    {
        leftButtonDown = false;
    }

    public void OnRightButton()
    {
        rightButtonDown = true;
    }

    public void OffRightButton()
    {
        rightButtonDown = false;
    }

    bool isGrounded()
    {
        if (Physics2D.Linecast(transform.position, new Vector2(transform.position.x, transform.position.y - 2.4f), 1 << LayerMask.NameToLayer("Platform")))
        {
            anim.SetBool("jump", false);
            return true;
        }
        if (Physics2D.Linecast(transform.position, new Vector2(transform.position.x - 1.2f, transform.position.y - 2.4f), 1 << LayerMask.NameToLayer("Platform")))
        {
            anim.SetBool("jump", false);
            return true;
        }
        if (Physics2D.Linecast(transform.position, new Vector2(transform.position.x + 1.2f, transform.position.y - 2.4f), 1 << LayerMask.NameToLayer("Platform")))
        {
            anim.SetBool("jump", false);
            return true;
        }
        return false;
    }

    void RIP()
    {
        Debug.Log("DEATH");
        dead = true;
        deadPos = transform.position;
        rb2d.isKinematic = true;
        bc2d.enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
        transform.GetChild(1).gameObject.SetActive(false);

        // If there no winner, go to spectator mode
        if (!GameObject.FindGameObjectsWithTag("Manager")[0].GetComponent<GameManager>().CheckForWinner())
            StartCoroutine(UpdateSpectatorMode());
        else
            Debug.Log("Winner");
    }

    public void Respawn()
    {
        if (!dead)
            return;
        GetComponent<SpriteRenderer>().enabled = true;
        rb2d.isKinematic = false;
        bc2d.enabled = true;
    }

    [PunRPC]
    void SetDizzy(int id)
    {
        if (photonView.viewID == id)
        {
            Debug.Log("Set dizzy");
            anim.SetTrigger("attacked");

            // Disable the attack collider if you are hit.
            transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    [PunRPC]
    void SetAttack(int id)
    {
        if (photonView.viewID == id)
        {
            // Activate the attack collider
            transform.GetChild(1).gameObject.SetActive(true);
            anim.SetTrigger("attack");

            StartCoroutine(waitForOneSecond());
        }
    }

    IEnumerator waitForOneSecond()
    {
        yield return new WaitForSeconds(1);

        anim.ResetTrigger("attack");
        transform.GetChild(1).gameObject.SetActive(false);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Water")
        {
            RIP();
            AudioSource audioS = GetComponent<AudioSource>();
            audioS.clip = splashes[Random.Range(0, 3)];
            audioS.Play();
        }
        
        // If the player collided with another player which is attacking, knock the player back
        if (photonView.isMine)
        {
            if (other.gameObject.tag == "Hit")
            {
                Vector2 direction = (transform.position - other.transform.parent.position).normalized;
                rb2d.AddForce(direction * attackForce);
                photonView.RPC("SetDizzy", PhotonTargets.All, photonView.viewID);
                Debug.Log("Ouch!");
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }
    }

    IEnumerator UpdateSpectatorMode()
    {
        Debug.Log("You died");

        yield return new WaitForSeconds(1);

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        target = deadPos;
        spectatorMode = SPECTATORMODE.TARGET;

        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                ++targetNo;

                if (targetNo >= players.Length)
                    targetNo = 0;

                if (targetNo == 0)
                    target = deadPos;
                else
                    target = players[targetNo].transform.position;

                spectatorMode = SPECTATORMODE.TARGET;
            }

            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) ||
                Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow))
            {
                spectatorMode = SPECTATORMODE.MANUAL;

                if (Input.GetKey(KeyCode.LeftArrow))
                    transform.position -= new Vector3(movementSpeed * Time.deltaTime, 0, 0);
                if (Input.GetKey(KeyCode.RightArrow))
                    transform.position += new Vector3(movementSpeed * Time.deltaTime, 0, 0);
                if (Input.GetKey(KeyCode.UpArrow))
                    transform.position += new Vector3(0, movementSpeed * Time.deltaTime, 0);
                if (Input.GetKey(KeyCode.DownArrow))
                    transform.position -= new Vector3(0, movementSpeed * Time.deltaTime, 0);
            }

            if (spectatorMode != SPECTATORMODE.MANUAL)
            {
                transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * 5);
            }

            yield return null;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.localScale);
            stream.SendNext(anim.GetBool("isMoving"));
            stream.SendNext(anim.GetBool("jump"));
            stream.SendNext(dead);
        }
        else
        {
            realPosition = (Vector3)stream.ReceiveNext();
            transform.localScale = (Vector3)stream.ReceiveNext();
            anim.SetBool("isMoving", (bool)stream.ReceiveNext());
            anim.SetBool("jump", (bool)stream.ReceiveNext());
            dead = (bool)stream.ReceiveNext();
        }
    }
}
