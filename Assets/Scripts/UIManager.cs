using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{



    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnAttackButton()
    {
        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
            player.GetComponent<Player>().HandleAttack();
    }

    public void OnJumpButton()
    {
        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
            player.GetComponent<Player>().HandleJump();
    }

    public void OnLeftButton()
    {
        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
            player.GetComponent<Player>().OnLeftButton();
    }

    public void OffLeftButton()
    {
        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
            player.GetComponent<Player>().OffLeftButton();
    }

    public void OnRightButton()
    {
        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
            player.GetComponent<Player>().OnRightButton();
    }

    public void OffRightButton()
    {
        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
            player.GetComponent<Player>().OffRightButton();
    }
}
