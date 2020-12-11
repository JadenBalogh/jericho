using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAnimator : MonoBehaviour {

    //General variables
    NPCController nc;
    Rigidbody2D rb;
    Animator anim;

    void Start()
    {
        nc = GetComponent<NPCController>();

        rb = GetComponent<Rigidbody2D>();

        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (nc.GetFacingDirection() == 1)
        {
            anim.SetLayerWeight(1, 1);
            anim.SetLayerWeight(2, 0);
        }
        else if (nc.GetFacingDirection() == -1)
        {
            anim.SetLayerWeight(1, 0);
            anim.SetLayerWeight(2, 1);
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
