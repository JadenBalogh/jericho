using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardController : MonoBehaviour {

    [Header("Patrol State")]
    public Transform[] moveLocations;
    public float patrolVelocity;
    public float patrolDragVelocity;
    public FloatRange idleDurationRange;

    [Space]
    [Header("Attack State")]
    [Header("Movement")]
    public float runVelocity;
    public float meleeWarningVelocity;
    public float adjustVelocity;
    public float adjustDelay;
    public float meleeAttackDistance;
    public float adjacentDistance;

    [Header("Combat")]
    public LayerMask playerMask;
    public int meleeDamage;
    public float meleeCooldown;
    [Tooltip("A portion of the Melee Cooldown time spent loading up the attack")]
    public float meleeWarningTime;
    public float meleeVerticalRange;

    //General variables
    GameObject player;
    GuardAnimator ga;
    Rigidbody2D rb;
    BoxCollider2D bc;
    Vector3 horizontalOffset;
    Vector3 verticalOffset;
    bool isOnScreen = false;
    bool isAttacking = false;
    int facingDirection;

    //Patrol variables
    Transform moveLocation;
    int locationIndex;
    float idleDuration;
    float idleTimer;
    bool isIdle;
    float lookForPlayerTimer = 100f;
    float findPlayerProbability;

    //Movement variables
    bool isMeleeAttacking = false;
    bool hasMeleeAttacked = true;
    float adjustTimer;

    //Combat variables
    float meleeTimer = 100f;

    public int GetFacingDirection()
    {
        return facingDirection;
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        ga = GetComponent<GuardAnimator>();

        rb = GetComponent<Rigidbody2D>();

        bc = GetComponent<BoxCollider2D>();

        horizontalOffset = new Vector3(bc.bounds.extents.x, 0.0f);

        verticalOffset = new Vector3(0.0f, bc.bounds.extents.y);

        locationIndex = GetRandomLocationIndex();
        moveLocation = moveLocations[locationIndex];

        idleDuration = Random.Range(idleDurationRange.min, idleDurationRange.max);
    }

    void FixedUpdate()
    {
        meleeTimer += Time.deltaTime;
        lookForPlayerTimer += Time.deltaTime;

        if (player.GetComponent<PlayerController>().GetIsStealthed() && !isAttacking && isOnScreen)
        {
            gameObject.layer = LayerMask.NameToLayer("Enemy");

            isAttacking = true;
        }
        else if (player.GetComponent<PlayerHealth>().GetIsDead() && isAttacking)
        {
            gameObject.layer = LayerMask.NameToLayer("NPC");

            isAttacking = false;
        }

        if (isAttacking)
        {
            facingDirection = GetPlayerDirection();

            AttackState();
        }
        else
        {
            PatrolState();
        }
    }

    void PatrolState()
    {
        if (isIdle)
        {
            idleTimer += Time.deltaTime;
        }
        else
        {
            if (Vector3.Distance(moveLocation.position, transform.position) < .1)
            {
                isIdle = true;

                rb.velocity = new Vector2(patrolDragVelocity * GetPatrolDirection(), 0.0f);
            }
            else
            {
                rb.velocity = new Vector2(patrolVelocity * GetPatrolDirection(), 0.0f);
            }
        }

        if (idleTimer >= idleDuration)
        {
            idleTimer = 0;
            isIdle = false;

            locationIndex = GetRandomLocationIndex();
            moveLocation = moveLocations[locationIndex];

            idleDuration = GetRandomDuration();
        }
    }

    void AttackState()
    {
        float distanceToPlayer = Mathf.Abs(player.transform.position.x - transform.position.x);

        //If the player is within melee attack distance
        if (distanceToPlayer <= meleeAttackDistance)
        {
            //If the player is within my bounds
            if (distanceToPlayer < horizontalOffset.x + player.GetComponent<BoxCollider2D>().bounds.extents.x)
            {
                adjustTimer += Time.deltaTime;

                //Move away after a short delay
                if (adjustTimer >= adjustDelay)
                {
                    rb.velocity = new Vector2(adjustVelocity * -GetPlayerDirection(), 0.0f);
                }
            }
            //If the player is within adjacent distance, stop moving
            else if (distanceToPlayer < adjacentDistance)
            {
                adjustTimer = 0;

                rb.velocity = Vector2.zero;
            }
            //If currently doing melee warning, stop moving
            else if (distanceToPlayer >= adjacentDistance && isMeleeAttacking && !hasMeleeAttacked)
            {
                adjustTimer = 0;

                rb.velocity = new Vector2(meleeWarningVelocity * GetPlayerDirection(), 0.0f);
            }
            //If the player is out of adjacent distance, move towards the player
            else if (distanceToPlayer >= adjacentDistance)
            {
                adjustTimer = 0;

                rb.velocity = new Vector2(runVelocity * GetPlayerDirection(), 0.0f);
            }

            if (player.transform.position.y > meleeVerticalRange)
            {
                if (isMeleeAttacking && !hasMeleeAttacked)
                {
                    ga.ResetAttack();

                    hasMeleeAttacked = true;

                    if (meleeTimer >= meleeCooldown)
                    {
                        isMeleeAttacking = false;
                    }
                }
            }
            else
            {
                //If melee attack sequence started, activate melee warning
                if (!isMeleeAttacking)
                {
                    isMeleeAttacking = true;

                    hasMeleeAttacked = false;

                    meleeTimer = 0;

                    ga.MeleeWarning();
                }

                //If melee attack is off cooldown, begin melee attack sequence
                if (meleeTimer >= meleeCooldown && isMeleeAttacking)
                {
                    isMeleeAttacking = false;
                }

                //If melee warning is done, melee attack
                if (meleeTimer >= meleeWarningTime && isMeleeAttacking && !hasMeleeAttacked)
                {
                    hasMeleeAttacked = true;

                    ga.MeleeAttack();

                    MeleeAttack();
                }
            }
        }
        //If the player is out of melee attack distance and still melee attacking, wait until melee warning is done
        else if (distanceToPlayer > meleeAttackDistance && isMeleeAttacking && !hasMeleeAttacked)
        {
            rb.velocity = new Vector2(meleeWarningVelocity * GetPlayerDirection(), 0.0f);

            if (meleeTimer >= meleeWarningTime)
            {
                ga.MeleeAttack();
                
                hasMeleeAttacked = true;
            }
        }
        //If the player is out of melee attack distance and just finished melee attacking, stop melee attacking
        else if (distanceToPlayer > meleeAttackDistance && isMeleeAttacking && hasMeleeAttacked)
        {
            if (meleeTimer >= meleeCooldown)
            {
                isMeleeAttacking = false;
            }
        }

        //If the player is out of melee attack distance, move towards the player
        if (distanceToPlayer > meleeAttackDistance && hasMeleeAttacked)
        {
            rb.velocity = new Vector2(runVelocity * GetPlayerDirection(), 0.0f);
        }
    }

    void OnBecameVisible()
    {
        isOnScreen = true;
    }

    void OnBecameInvisible()
    {
        isOnScreen = false;
    }

    int GetPlayerDirection()
    {
        if (transform.position.x < player.transform.position.x)
        {
            return 1;
        }
        else if (transform.position.x > player.transform.position.x)
        {
            return -1;
        }

        return 1;
    }
    
    void MeleeAttack()
    {
        if (player.transform.position.y < transform.position.y + meleeVerticalRange)
        {
            player.GetComponent<PlayerHealth>().TakeDamage(meleeDamage, gameObject);
        }
    }
    
    int GetRandomLocationIndex()
    {
        int locIndex = Random.Range(0, moveLocations.Length);

        if (locIndex == locationIndex)
        {
            return GetRandomLocationIndex();
        }
        else
        {
            return locIndex;
        }
    }

    float GetRandomDuration()
    {
        float dur = Random.Range(idleDurationRange.min, idleDurationRange.max);

        return dur;
    }

    int GetPatrolDirection()
    {
        if (moveLocation.position.x > transform.position.x)
        {
            facingDirection = 1;

            return 1;
        }
        else
        {
            facingDirection = -1;

            return -1;
        }
    }
}
