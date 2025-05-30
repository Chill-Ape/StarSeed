using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    private PlayerMovement playerMovement;
    private PlayerAttack playerAttack;
    private PlayerHealth playerHealth;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        playerAttack = GetComponent<PlayerAttack>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        if (playerHealth.IsDead) return;
        
        // Update movement animation
        animator.SetBool("Moving", playerMovement.IsMoving);
        
        // Update directional movement
        if (playerMovement.IsMoving)
        {
            Vector2 moveDirection = playerMovement.MoveDirection;
            animator.SetFloat("MoveX", moveDirection.x);
            animator.SetFloat("MoveY", moveDirection.y);
        }
        
        // Update attack animation
        animator.SetBool("Attacking", playerAttack.IsAttacking);
    }

    public void SetMoveBoolTransition(bool value)
    {
        animator.SetBool("Moving", value);
    }

    public void SetAttackAnimation(bool value)
    {
        animator.SetBool("Attacking", value);
    }

    public void SetDeadAnimation()
    {
        animator.SetTrigger("Dead");
    }

    public void SetReviveAnimation()
    {
        ResetAllTriggers();
        animator.SetTrigger("Revive");
    }

    public void ResetAllTriggers()
    {
        animator.ResetTrigger("Dead");
        animator.ResetTrigger("Revive");
        animator.SetBool("Moving", false);
        animator.SetBool("Attacking", false);
        animator.SetFloat("MoveX", 0);
        animator.SetFloat("MoveY", 0);
    }

    public void SetBlockingAnimation(bool value)
    {
        animator.SetBool("Blocking", value);
    }
}
