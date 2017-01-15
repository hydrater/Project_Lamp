using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
	public Player myPlayer;

    public void OnAttackButton()
    {
		if (myPlayer != null)
			myPlayer.HandleAttack();
    }

    public void OnJumpButton()
    {
		if (myPlayer != null)
			myPlayer.HandleJump();
    }

    public void OnLeftButton()
    {
		if (myPlayer != null)
			myPlayer.OnLeftButton();
    }

    public void OffLeftButton()
    {
		if (myPlayer != null)
			myPlayer.OffLeftButton();
    }

    public void OnRightButton()
    {
		if (myPlayer != null)
			myPlayer.OnRightButton();
    }

    public void OffRightButton()
    {
		if (myPlayer != null)
			myPlayer.OffRightButton();
    }
}
