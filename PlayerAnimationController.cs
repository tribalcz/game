using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Stavy pohybu
public enum MovingState
{
    Idle = 0,
    Walking = 1,
    Running = 2,
}

/**
 * Ovládá animace hráče
 */
public class PlayerAnimationController : MonoBehaviour
{
    //Reference na instanci Animátoru
    [SerializeField] private Animator animator = null;
    
    //Reference na instanci hráče
    private Player player = null;
    
    //Inicializace třídy a nastavení hráče
    public void Init(Player pPlayer)
    {
        player = pPlayer;
    }
    
    /**
     * Nastaví stav pohybu
     */
    public void SetMovementState(MovingState pState)
    {
        //Výchozí stav je 0 tedy idle
        int state = 0;
        switch (pState)
        {
            case MovingState.Idle:
                state = 0;
                break;
            case MovingState.Walking:
                state = 1;
                break;
            case MovingState.Running:
                state = 2;
                break;
        }
        
        Debug.Log("Setting MovingState to: " + state); //Nějaký ten debug výpis do konzole
        animator.SetInteger("MovingState", state); //Nastavení stavu pohybu
    }
    
    // Nastavení state pro skok
    public void SetJumpingState(bool pIsJumping)
    {
        Debug.Log("Setting IsJumping to: " + pIsJumping);
        animator.SetBool("isJumping", pIsJumping);
    }

    // Nastavení state pro smrt
    public void SetDeathState()
    {
        animator.SetBool("isDead", true);
    }
}
