using UnityEngine;

public class EnemyController : MonoBehaviour {

    [Header("Patrol State")]
    public Transform[] moveLocations;
    public float patrolVelocity;
    public float patrolDragVelocity;
    public FloatRange idleDurationRange;
    public float detectVerticalMaxDistance;
    public float longRangeDetectDistance;
    [Range(0, 100)]
    public float longRangeDetectProbability;
    [Range(0, 100)]
    public float longRangeVisionDetectProbability;
    public float midRangeDetectDistance;
    [Range(0, 100)]
    public float midRangeDetectProbability;
    [Range(0, 100)]
    public float midRangeVisionDetectProbability;
    public float closeRangeDetectDistance;
    [Range(0, 100)]
    public float closeRangeDetectProbability;
    [Range(0, 100)]
    public float closeRangeVisionDetectProbability;

    [Space]
    [Header("Attack State")]
    public bool isMelee;
    public bool isRanged;

    [Header("Movement")]
    public float runVelocity;
    public float meleeWarningVelocity;
    public float adjustVelocity;
    public float adjustDelay;
    public float tryEngageRunDistance;
    public float meleeEngageDistance;
    public float meleeAttackDistance;
    public float adjacentDistance;

    [Header("Combat")]
    public LayerMask playerMask;
    public int meleeDamage;
    public float meleeCooldown;
    [Tooltip("A portion of the Melee Cooldown time spent loading up the attack")]
    public float meleeWarningTime;
    public float meleeVerticalRange;
    public GameObject rangedPrefab;
    public int rangedDamage;
    public float rangedCooldown;
    public float rangedStandTime;
    public float rangedVelocity;
    public float rangedSpawnOffset;
    
    //General variables
    GameObject player;
    EnemyAnimator ea;
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
    bool isFacingPlayer;
    float detectPlayerTimer = 100f;
    float playerDetectProbability;

    //Movement variables
    bool isMeleeAttacking = false;
    bool hasMeleeAttacked = true;
    bool isTryingToEngage = false;
    bool canEngage = false;
    float tryEngageRunPosition;
    int tryEngageRunDirection;
    float adjustTimer;

    //Combat variables
    bool hasRangeAttacked = false;
    float meleeTimer = 100f;
    float rangedTimer = 100f;

    public int GetFacingDirection()
    {
        return facingDirection;
    }

    void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        ea = GetComponent<EnemyAnimator>();

        rb = GetComponent<Rigidbody2D>();

        bc = GetComponent<BoxCollider2D>();

        horizontalOffset = new Vector3(bc.bounds.extents.x, 0.0f);

        verticalOffset = new Vector3(0.0f, bc.bounds.extents.y);

        locationIndex = GetRandomLocationIndex();
        moveLocation = moveLocations[locationIndex];

        idleDuration = Random.Range(idleDurationRange.min, idleDurationRange.max);
    }
    
	void FixedUpdate ()
    {
        meleeTimer += Time.deltaTime;
        rangedTimer += Time.deltaTime;
        detectPlayerTimer += Time.deltaTime;
        
        //If the player is dead or off screen, enter patrol state
        if (player.GetComponent<PlayerHealth>().GetIsDead() || !isOnScreen)
        {
            PatrolState();
            
            return;
        }

        //If the player is stealthed and we are not currently in combat, look for player
        if (player.GetComponent<PlayerController>().GetIsStealthed() && !isAttacking)
        {
            bool canSeePlayer = LookForPlayer();

            //If we do not find the player, enter patrol state
            if (!canSeePlayer)
            {
                PatrolState();

                return;
            }
            //If we find the player, disable player stealth, then attack
            else
            {
                player.GetComponent<PlayerController>().SetIsStealthed(false);
            }
        }

        isAttacking = true;

        if (player.GetComponent<PlayerController>().GetIsStealthed())
        {
            player.GetComponent<PlayerController>().SetIsStealthed(false);
        }

        facingDirection = GetPlayerDirection();

        if (isMelee && isRanged)
        {
            ComboAttackState();
        }
        else if (isMelee)
        {
            MeleeAttackState();
        }
        else if (isRanged)
        {
            RangedAttackState();
        }
	}

    void PatrolState()
    {
        float distanceToPlayer = Mathf.Abs(player.transform.position.x - transform.position.x);

        if (player.transform.position.y < detectVerticalMaxDistance &&
            distanceToPlayer <= longRangeDetectDistance &&
           (player.transform.position.x >= transform.position.x && GetFacingDirection() == 1 ||
           player.transform.position.x < transform.position.x && GetFacingDirection() == -1))
        {
            isFacingPlayer = true;
        }
        else
        {
            isFacingPlayer = false;
        }

        if (player.transform.position.y >= detectVerticalMaxDistance && distanceToPlayer <= longRangeDetectDistance)
        {
            if (isFacingPlayer)
            {
                playerDetectProbability = longRangeVisionDetectProbability;
            }
            else
            {
                playerDetectProbability = longRangeDetectProbability;
            }
        }
        else if (distanceToPlayer <= closeRangeDetectDistance)
        {
            if (isFacingPlayer)
            {
                playerDetectProbability = closeRangeVisionDetectProbability;
            }
            else
            {
                playerDetectProbability = closeRangeDetectProbability;
            }
        }
        else if (distanceToPlayer <= midRangeDetectDistance)
        {
            if (isFacingPlayer)
            {
                playerDetectProbability = midRangeVisionDetectProbability;
            }
            else
            {
                playerDetectProbability = midRangeDetectProbability;
            }
        }
        else if (distanceToPlayer <= longRangeDetectDistance)
        {
            if (isFacingPlayer)
            {
                playerDetectProbability = longRangeVisionDetectProbability;
            }
            else
            {
                playerDetectProbability = longRangeDetectProbability;
            }
        }
        else
        {
            playerDetectProbability = 0;
        }
        
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

    void ComboAttackState()
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
                    hasMeleeAttacked = true;

                    if (meleeTimer >= meleeCooldown)
                    {
                        isMeleeAttacking = false;
                    }
                }

                if (rangedTimer >= rangedCooldown)
                {
                    rangedTimer = 0;

                    RangedAttack();
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

                    ea.MeleeWarning();
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

                    ea.MeleeAttack();

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
                ea.MeleeAttack();

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
        //If the player is within melee engage distance, move towards the player
        else if (distanceToPlayer <= meleeEngageDistance && !isMeleeAttacking)
        {
            rb.velocity = new Vector2(runVelocity * GetPlayerDirection(), 0.0f);

            hasRangeAttacked = false;

            isTryingToEngage = false;

            canEngage = false;
        }
        //If the player is out of melee engage distance
        else if (distanceToPlayer > meleeEngageDistance && !isMeleeAttacking)
        {
            //If has not range attacked, stop moving and attack
            if (!hasRangeAttacked)
            {
                rb.velocity = Vector2.zero;

                if (rangedTimer >= rangedCooldown)
                {
                    RangedAttack();

                    hasRangeAttacked = true;

                    Invoke("EnableEngage", rangedCooldown);
                }
            }
            //If has range attacked and has started attempting to engage, move towards the player in an attempt to reach melee engage distance
            else if (isTryingToEngage)
            {
                if (transform.position.x < tryEngageRunPosition && tryEngageRunDirection == 1)
                {
                    rb.velocity = new Vector2(runVelocity, 0.0f);
                }
                else if (transform.position.x > tryEngageRunPosition && tryEngageRunDirection == -1)
                {
                    rb.velocity = new Vector2(-runVelocity, 0.0f);
                }
                else
                {
                    rb.velocity = Vector2.zero;

                    hasRangeAttacked = false;

                    isTryingToEngage = false;
                }
            }
            //If has range attacked, attempt to engage the player by moving towards them
            else if (canEngage)
            {
                isTryingToEngage = true;

                canEngage = false;

                tryEngageRunPosition = transform.position.x + (tryEngageRunDistance * GetPlayerDirection());

                if (transform.position.x < tryEngageRunPosition)
                {
                    tryEngageRunDirection = 1;
                }
                else if (transform.position.x > tryEngageRunPosition)
                {
                    tryEngageRunDirection = -1;
                }
            }
        }
    }

    void MeleeAttackState()
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
                    ea.ResetAttack();

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

                    ea.MeleeWarning();
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

                    ea.MeleeAttack();

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
                ea.MeleeAttack();

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

    void RangedAttackState()
    {

    }

    void OnBecameVisible()
    {
        isOnScreen = true;

        isAttacking = false;
    }

    void OnBecameInvisible()
    {
        isOnScreen = false;

        isAttacking = false;
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

    bool LookForPlayer()
    {
        #region Per Frame Checks
        /*
        If you roll a 6-sided dice three times, what are the overall odds of getting a 6?
        There is a 1/6 chance we will get a 6 on the first roll.
        Consider in terms of the odds of NOT getting a 6. There is a 5/6 chance we WON'T get a 6 on the first roll.
        On the second roll, there is a 1/6 chance we WILL get a 6.
        However, we only consider this if the first roll was NOT a 6.
        At this point, the odds are as follows:
        (1/6 chance at getting 6 on the first roll) + ((1/6 chance at getting 6 on the second roll) * (5/6 chance at NOT getting 6 on the first roll))
        = 11/36
        Now, for the last roll we consider that to have not already rolled a 6, we went through a 5/6 chance on the first roll and second roll.
        5/6 * 5/6 = 25/36 chance that we get to the third roll without getting a 6.
        Therefore, the odds for the last roll to be a 6 are:
        (1/6 chance at getting 6 on the first roll) + ((1/6 chance at getting 6 on the second roll) * (5/6 chance at NOT getting 6 on the first roll))
        + ((1/6 chance at getting 6 on the second roll) * (25/36 chance at NOT getting 6 on the first two rolls))
        = 91/216

        The general formula for rolling at least one 6 in n rolls is 1 - (5/6)^n.

        Here, we want to have a roll each frame where the odds of each check totaled over one second equal the findPlayerProbability.
        Let's assume playerFindProbability is 20% and the framerate is 60.

        0.20 = 1 - n^60
        n^60 = 0.80

        check = (1 - (1 - findPlayerProbability)^(fixedDeltaTime)) * 100


        float findPlayerProbability = baseDetectProbability + rangeDetectProbability;

        if (isFacingPlayer)
        {
            findPlayerProbability += visionDetectProbability;
        }

        float check = (1 - Mathf.Pow((1 - findPlayerProbability), (Time.fixedDeltaTime))) * 100;

        Debug.Log("Result: " + result + ", Check: " + check + ", Chance: " + (findPlayerProbability * 100) + "%");

        */
        #endregion

        if (detectPlayerTimer >= 1.0f)
        {
            detectPlayerTimer = 0;

            float result = Random.Range(0.0f, 100.0f);
            
            if (result <= playerDetectProbability)
            {
                return true;
            }
        }
        
        return false;
    }

    void EnableEngage()
    {
        canEngage = true;
    }
    
    void MeleeAttack()
    {
        if (player.transform.position.y < transform.position.y + meleeVerticalRange)
        {
            player.GetComponent<PlayerHealth>().TakeDamage(meleeDamage, gameObject);
        }
    }

    void RangedAttack()
    {
        Vector3 rangedOffset = new Vector3(rangedSpawnOffset * facingDirection, bc.offset.y);

        Vector3 rangedSpawnPoint = transform.position + horizontalOffset + rangedOffset;

        Vector3 rangedDirection = Vector3.Normalize(player.transform.position - rangedSpawnPoint);

        GameObject rangedObject = Instantiate(rangedPrefab, rangedSpawnPoint, Quaternion.identity) as GameObject;
        rangedObject.GetComponent<Projectile>().projectileSettings.direction = rangedDirection;
        rangedObject.GetComponent<Projectile>().projectileSettings.targetMask = playerMask;
        rangedObject.GetComponent<Projectile>().projectileSettings.damage = rangedDamage;
        rangedObject.GetComponent<Projectile>().projectileSettings.velocity = rangedVelocity;
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
