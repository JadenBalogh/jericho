using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
[Serializable]
public class PlayerAnimations {

    public AnimationClip IdleR;
    public AnimationClip IdleL;
    public AnimationClip MoveR;
    public AnimationClip MoveL;
    public AnimationClip StealthIdleR;
    public AnimationClip StealthIdleL;
    public AnimationClip StealthMoveR;
    public AnimationClip StealthMoveL;
    public AnimationClip JumpR;
    public AnimationClip JumpL;
    public AnimationClip DoubleJumpR;
    public AnimationClip DoubleJumpL;
    public AnimationClip FallR;
    public AnimationClip FallL;
    public AnimationClip MeleeAttackR;
    public AnimationClip MeleeAttackL;
    public AnimationClip RangedAttackR;
    public AnimationClip RangedAttackL;
    public AnimationClip DazeSpellR;
    public AnimationClip DazeSpellL;
    public AnimationClip HealSpellR;
    public AnimationClip HealSpellL;
    public AnimationClip ClimbUp;
    public AnimationClip ClimbDown;
}
*/

public class PlayerAnimator : MonoBehaviour {
    
    //General variables
    PlayerController pc;
    Rigidbody2D rb;
    Animator anim;

    public void RollDodge()
    {
        anim.SetTrigger("RollDodge");
    }

    public void Jump()
    {
        anim.SetTrigger("Jump");
    }

    public void DoubleJump()
    {
        anim.SetTrigger("DoubleJump");
    }

    public void ParryHit()
    {
        anim.SetTrigger("ParryHit");
    }

    public void MeleeAttack()
    {
        anim.SetTrigger("MeleeAttack");
    }

    public void RangedAttack()
    {
        anim.SetTrigger("RangedAttack");
    }

    public void DazeSpell()
    {
        anim.SetTrigger("DazeSpell");
    }

    public void HealSpell()
    {
        anim.SetTrigger("HealSpell");
    }

    void Start()
    {
        pc = GetComponent<PlayerController>();

        rb = GetComponent<Rigidbody2D>();

        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x >= transform.position.x)
        {
            anim.SetLayerWeight(1, 1);
            anim.SetLayerWeight(2, 0);
            anim.SetLayerWeight(3, 1);
            anim.SetLayerWeight(4, 0);
        }
        else
        {
            anim.SetLayerWeight(1, 0);
            anim.SetLayerWeight(2, 1);
            anim.SetLayerWeight(3, 0);
            anim.SetLayerWeight(4, 1);
        }

        if (rb.velocity.y > 0)
        {
            anim.SetBool("IsFalling", false);
            anim.SetFloat("ClimbState", 1);
        }
        else if (rb.velocity.y == 0)
        {
            anim.SetBool("IsFalling", false);
            anim.SetFloat("ClimbState", 0);
        }
        else if (rb.velocity.y < 0)
        {
            anim.SetBool("IsFalling", true);
            anim.SetFloat("ClimbState", 1);
        }

        anim.SetBool("IsParrying", pc.GetIsParrying());
        anim.SetBool("IsMoving", pc.GetIsMoving());
        anim.SetBool("IsStealthed", pc.GetIsStealthed());
        anim.SetBool("IsClimbing", pc.GetIsClimbing());
    }
}
