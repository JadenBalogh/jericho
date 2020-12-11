using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour {

    //General variables
    EnemyController ec;
    Rigidbody2D rb;
    Animator anim;

    public void MeleeAttack()
    {
        anim.SetTrigger("MeleeAttack");
    }

    public void MeleeWarning()
    {
        anim.SetTrigger("MeleeWarning");
    }

    public void ResetAttack()
    {
        anim.SetTrigger("ResetAttack");
    }

    public void Die()
    {
        anim.SetTrigger("Die");
    }

    void Start()
    {
        ec = GetComponent<EnemyController>();

        rb = GetComponent<Rigidbody2D>();

        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (ec.GetFacingDirection() == 1)
        {
            anim.SetLayerWeight(1, 1);
            anim.SetLayerWeight(2, 0);
            anim.SetLayerWeight(3, 1);
            anim.SetLayerWeight(4, 0);
        }
        else if (ec.GetFacingDirection() == -1)
        {
            anim.SetLayerWeight(1, 0);
            anim.SetLayerWeight(2, 1);
            anim.SetLayerWeight(3, 0);
            anim.SetLayerWeight(4, 1);
        }

        if (rb.velocity.x > 1 || rb.velocity.x < -1)
        {
            anim.SetBool("IsMoving", true);
        }
        else
        {
            anim.SetBool("IsMoving", false);
        }
    }
}
