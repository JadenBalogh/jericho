using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [Header("Movement")]
    public GameObject dustKickObject;
    public LayerMask groundMask;
    public float groundDetectDistance;
    public LayerMask climbMask;
    public float climbVelocity;
    public float runVelocity;
    public float runDragVelocity;
    public Color stealthColor;
    public float stealthClimbVelocity;
    public float stealthVelocity;
    public float stealthDragVelocity;
    public float rollDodgeCooldown;
    public float rollDodgeDuration;
    public float rollDodgeDistance;
    public float gravityScale;
    public float jumpVelocity;
    public int maxAirJumps;

    [Header("Combat")]
    public LayerMask enemyMask;
    public float universalCooldown;
    public int meleeDamage;
    public float meleeCooldown;
    public float meleeRange;
    public float stealthDamageMultiplier;
    public int parryAmount;
    /*
    public GameObject rangedPrefab;
    public int rangedDamage;
    public float rangedCooldown;
    public float rangedVelocity;
    public float rangedSpawnOffset;
    */

    [Header("Spells")]
    public int healSpellHealing;
    public float healSpellCooldown;
    public float healSpellFreezeDuration;

    [Header("Interaction")]
    public GameObject interactIndicatorPrefab;
    public float interactIndicatorOffset;
    public LayerMask interactMask;
    public float interactMaxDistance;

    //General variables
    Rigidbody2D rb;
    BoxCollider2D bc;
    PlayerAnimator pa;
    PlayerHealth ph;
    Vector3 bottomOffset;
    Vector3 horizontalOffset;
    int facingDirection;
    bool isControllerEnabled = true;

    //Movement variables
    GameObject activeWall;
    bool isMoving = false;
    bool isGrounded = true;
    bool wasGroundedLastFrame = false;
    bool isStealthed = false;
    bool isClimbing = false;
    bool canClimbDown = false;
    bool canClimbUp = false;
    bool isWithinBounds = false;
    bool isRolling = false;
    bool canJump = false;
    int jumpCount;
    int lastJumpCount;
    Vector2 rollDodgeStartPos;
    Vector2 rollDodgeEndPos;
    float rollDodgeTimer = 100f;
    float rollDodgeRate;
    float rollDodgeProgress;

    //Combat variables
    bool isParrying = false;
    float universalTimer = 100f;
    float meleeTimer = 100f;
    //float rangedTimer = 100f;

    //Spells variables
    float healSpellTimer = 100f;
    
    public Vector3 GetBottomOffset()
    {
        return bottomOffset;
    }

    public Vector3 GetHorizontalOffset()
    {
        return horizontalOffset;
    }

    public GameObject GetActiveWall()
    {
        return activeWall;
    }

    public bool GetIsControllerEnabled()
    {
        return isControllerEnabled;
    }

    public bool GetCanClimbDown()
    {
        return canClimbDown;
    }

    public bool GetCanClimbUp()
    {
        return canClimbUp;
    }

    public int GetFacingDirectionWithMouse()
    {
        if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x >= transform.position.x)
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }

    public int GetFacingDirection()
    {
        return facingDirection;
    }

    public bool GetIsClimbing()
    {
        return isClimbing;
    }

    public bool GetIsMoving()
    {
        return isMoving;
    }
    
    public bool GetIsGrounded()
    {
        return isGrounded;
    }

    public bool GetIsStealthed()
    {
        return isStealthed;
    }

    public bool GetIsParrying()
    {
        return isParrying;
    }

    public void SetActiveWall(GameObject activeWall)
    {
        this.activeWall = activeWall;
    }

    public void SetIsControllerEnabled(bool isControllerEnabled)
    {
        this.isControllerEnabled = isControllerEnabled;
    }

    public void SetIsClimbing(bool isClimbing)
    {
        if (isClimbing)
        {
            rb.gravityScale = 0.0f;
        }
        else
        {
            rb.gravityScale = gravityScale;
        }

        this.isClimbing = isClimbing;
    }

    public void SetCanClimbDown(bool canClimbDown)
    {
        this.canClimbDown = canClimbDown;
    }

    public void SetCanClimbUp(bool canClimbUp)
    {
        this.canClimbUp = canClimbUp;
    }

    public void SetIsWithinBounds(bool isWithinBounds)
    {
        this.isWithinBounds = isWithinBounds;
    }

    public void SetIsGrounded(bool isGrounded)
    {
        this.isGrounded = isGrounded;
    }

    public void SetIsStealthed(bool isStealthed)
    {
        this.isStealthed = isStealthed;

        //Stealth color
        if (isStealthed)
        {
            GetComponent<SpriteRenderer>().color = stealthColor;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
    
    public void SetCanJump(bool canJump)
    {
        this.canJump = canJump;
    }

    public void SetJumpCount(int jumpCount)
    {
        this.jumpCount = jumpCount;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        bc = GetComponent<BoxCollider2D>();

        pa = GetComponent<PlayerAnimator>();

        ph = GetComponent<PlayerHealth>();
        
        bottomOffset = new Vector3(0.0f, -bc.bounds.extents.y + bc.offset.y, 0.0f);
        
        horizontalOffset = new Vector3(bc.bounds.extents.x, 0.0f, 0.0f);

        rb.gravityScale = gravityScale;

        rollDodgeRate = 1.0f / rollDodgeDuration;
    }
	
	void Update()
    {
        rollDodgeTimer += Time.deltaTime;
        universalTimer += Time.deltaTime;
        meleeTimer += Time.deltaTime;
        //rangedTimer += Time.deltaTime;

        if (!isControllerEnabled)
        {
            return;
        }

        #region Movement

        wasGroundedLastFrame = isGrounded;

        Vector2 downDirection = new Vector2(0, -1);

        /*Check if grounded
        RaycastHit2D groundRay = Physics2D.Raycast(transform.position, downDirection, 10, groundMask);

        float groundDistance = Vector2.Distance(transform.position + bottomOffset, groundRay.point);

        if (groundDistance < groundDetectDistance)
        {
            if (!(jumpCount == 1 && lastJumpCount == 0))
            {
                isGrounded = true;

                jumpCount = 0;
            }
        }
        else
        {
            isGrounded = false;
        }
        */

        RaycastHit2D groundRay = Physics2D.Raycast(transform.position + horizontalOffset, downDirection, 10, groundMask);

        float groundDistance = Vector2.Distance(transform.position + horizontalOffset + bottomOffset, groundRay.point);

        if (groundDistance < groundDetectDistance)
        {
            if (!(jumpCount == 1 && lastJumpCount == 0))
            {
                isGrounded = true;

                jumpCount = 0;
            }
        }
        else
        {
            isGrounded = false;
        }

        if (!groundRay)
        {
            groundRay = Physics2D.Raycast(transform.position - horizontalOffset, downDirection, 10, groundMask);

            groundDistance = Vector2.Distance(transform.position - horizontalOffset + bottomOffset, groundRay.point);

            if (groundDistance < groundDetectDistance)
            {
                if (!(jumpCount == 1 && lastJumpCount == 0))
                {
                    isGrounded = true;

                    jumpCount = 0;
                }
            }
            else
            {
                isGrounded = false;
            }
        }

        /*Check if can climb down
        groundRay = Physics2D.Raycast(transform.position, downDirection, 10, climbMask);

        groundDistance = Vector2.Distance(transform.position + bottomOffset, groundRay.point);
        
        if (groundDistance < groundDetectDistance && canClimbDown)
        {
            if (!(jumpCount == 1 && lastJumpCount == 0))
            {
                isGrounded = true;
                
                jumpCount = 0;
            }
        }
        */

        groundRay = Physics2D.Raycast(transform.position + horizontalOffset, downDirection, 10, climbMask);

        groundDistance = Vector2.Distance(transform.position + horizontalOffset + bottomOffset, groundRay.point);

        if (groundDistance < groundDetectDistance && canJump && !canClimbUp)
        {
            if (!(jumpCount == 1 && lastJumpCount == 0))
            {
                isGrounded = true;

                jumpCount = 0;
            }
        }

        if (!groundRay)
        {
            groundRay = Physics2D.Raycast(transform.position - horizontalOffset, downDirection, 10, climbMask);

            groundDistance = Vector2.Distance(transform.position - horizontalOffset + bottomOffset, groundRay.point);

            if (groundDistance < groundDetectDistance && canJump && !canClimbUp)
            {
                if (!(jumpCount == 1 && lastJumpCount == 0))
                {
                    isGrounded = true;

                    jumpCount = 0;
                }
            }
        }

        //Dust Kick Particles
        if (isGrounded)
        {
            ParticleSystem.EmissionModule em = dustKickObject.GetComponent<ParticleSystem>().emission;
            em.enabled = true;
        }
        else
        {
            ParticleSystem.EmissionModule em = dustKickObject.GetComponent<ParticleSystem>().emission;
            em.enabled = false;
        }

        //Climb Up
        if (Input.GetKeyDown(KeyCode.W) && isGrounded && canClimbUp && isWithinBounds && !isParrying && !isRolling)
        {
            isMoving = false;
            
            rb.gravityScale = 0;

            isClimbing = true;
        }

        //Climb Down
        if (Input.GetKeyDown(KeyCode.S) && canClimbDown && !isParrying && !isRolling)
        {
            if (groundRay)
            {
                isMoving = false;

                groundRay.collider.gameObject.GetComponent<BoxCollider2D>().isTrigger = true;

                canClimbDown = false;

                canClimbUp = true;

                rb.gravityScale = 0;

                isClimbing = true;
            }
        }

        //Climb Movement
        if (Input.GetButton("Vertical") && isClimbing)
        {
            if (!isStealthed)
            {
                rb.velocity = new Vector2(0.0f, climbVelocity * Input.GetAxis("Vertical"));
            }
            else
            {
                rb.velocity = new Vector2(0.0f, stealthClimbVelocity * Input.GetAxis("Vertical"));
            }

            if (isGrounded && !wasGroundedLastFrame)
            {
                rb.gravityScale = gravityScale;

                isClimbing = false;
            }
        }
        else if (Input.GetButtonUp("Vertical") && isClimbing)
        {
            rb.velocity = Vector2.zero;
        }

        //Move left and right
        if (Input.GetButton("Horizontal") && !isClimbing && !isRolling && !isParrying)
        {
            isMoving = true;

            if (Input.GetKeyDown(KeyCode.D))
            {
                SetFacingDirection(1);
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                SetFacingDirection(-1);
            }

            if (!isStealthed)
            {
                rb.velocity = new Vector2(runVelocity * Input.GetAxis("Horizontal"), rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(stealthVelocity * Input.GetAxis("Horizontal"), rb.velocity.y);
            }
        }
        //Drag after stopped moving
        else if (Input.GetButtonUp("Horizontal") && !isClimbing && !isRolling && !isParrying)
        {
            isMoving = false;

            if (!isStealthed)
            {
                rb.velocity = new Vector2(runDragVelocity * Input.GetAxis("Horizontal"), rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(stealthDragVelocity * Input.GetAxis("Horizontal"), rb.velocity.y);
            }
        }
        
        //Roll dodge
        if (Input.GetKeyDown(KeyCode.LeftShift) && rollDodgeTimer >= rollDodgeCooldown && isGrounded && !isParrying)
        {
            pa.RollDodge();

            isRolling = true;

            isMoving = false;

            rb.velocity = Vector2.zero;

            rollDodgeTimer = 0;

            float rollDodgeFacingDirection = Input.GetAxis("Horizontal") != 0 ? Input.GetAxis("Horizontal") : GetFacingDirectionWithMouse(); 

            if (rollDodgeFacingDirection > 0)
            {
                rollDodgeFacingDirection = 1;
            }
            else if (rollDodgeFacingDirection < 0)
            {
                rollDodgeFacingDirection = -1;
            }

            rollDodgeStartPos = transform.position;
            rollDodgeEndPos = new Vector2(transform.position.x + (rollDodgeFacingDirection * rollDodgeDistance), transform.position.y);

            rollDodgeProgress = 0;
        }

        if (rollDodgeProgress < 1.0f)
        {
            rollDodgeProgress += Time.deltaTime * rollDodgeRate;

            transform.position = Vector3.Lerp(rollDodgeStartPos, rollDodgeEndPos, rollDodgeProgress);

            if (rollDodgeProgress >= 1.0f)
            {
                isRolling = false;
            }
        }

        //Stealth toggle
        if (Input.GetKeyDown(KeyCode.LeftControl) && isGrounded && !isClimbing && !isParrying && !isRolling)
        {
            ToggleStealth();
        }

        lastJumpCount = jumpCount;

        //Jumping while grounded
        if (Input.GetButtonDown("Jump") && isGrounded && jumpCount == 0 && !isClimbing && !isParrying && !isRolling)
        {
            jumpCount++;

            pa.Jump();

            if (isStealthed)
            {
                ToggleStealth();
            }

            rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
        }
        //Jumping while not grounded
        else if (Input.GetButtonDown("Jump") && !isGrounded && jumpCount <= maxAirJumps && jumpCount > 0 && !isClimbing && !isParrying && !isRolling)
        {
            jumpCount++;

            pa.DoubleJump();

            rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
        }
        #endregion
        
        #region Combat
        
        //Melee
        if (Input.GetButton("Fire1") && meleeTimer >= meleeCooldown && universalTimer >= universalCooldown && !isClimbing && !isParrying && !isRolling)
        {
            pa.MeleeAttack();

            Vector2 meleeDirection = new Vector2(GetFacingDirectionWithMouse(), 0);

            RaycastHit2D meleeRay = Physics2D.Raycast(transform.position, meleeDirection, Mathf.Infinity, enemyMask);
            
            if (meleeRay)
            {
                float meleeDistance = Vector2.Distance(transform.position + horizontalOffset, meleeRay.point);

                if (meleeDistance <= meleeRange)
                {
                    float sneakAttackMultiplier = isStealthed ? stealthDamageMultiplier : 1;

                    if (isStealthed)
                    {
                        ToggleStealth();
                    }

                    GameObject meleeTarget = meleeRay.collider.gameObject;

                    int totalMeleeDamage = (int)(meleeDamage * sneakAttackMultiplier);

                    if (meleeTarget.CompareTag("Enemy"))
                    {
                        meleeTarget.GetComponent<EnemyHealth>().TakeDamage(totalMeleeDamage);
                    }
                    else if (meleeTarget.CompareTag("Guard"))
                    {
                        meleeTarget.GetComponent<GuardHealth>().TakeDamage(totalMeleeDamage);
                    }
                }
            }

            meleeTimer = 0;

            universalTimer = 0;
        }

        /*Ranged
        if (Input.GetButton("Fire2") && rangedTimer >= rangedCooldown && universalTimer >= universalCooldown && !isClimbing)
        {
            Vector3 rangedDirection = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
            
            Vector3 rangedOffset = new Vector3(rangedSpawnOffset * GetFacingDirectionWithMouse(), bc.offset.y);
            
            Vector3 rangedSpawnPoint = transform.position + horizontalOffset + rangedOffset;

            rangedDirection = Vector3.Normalize(rangedDirection - rangedSpawnPoint);

            int sneakDamageMultiplier = isStealthed ? 2 : 1;

            isStealthed = false;

            pa.RangedAttack();

            GameObject rangedObject = Instantiate(rangedPrefab, rangedSpawnPoint, Quaternion.identity) as GameObject;
            rangedObject.GetComponent<Projectile>().projectileSettings.direction = rangedDirection;
            rangedObject.GetComponent<Projectile>().projectileSettings.targetMask = enemyMask;
            rangedObject.GetComponent<Projectile>().projectileSettings.damage = rangedDamage * sneakDamageMultiplier;
            rangedObject.GetComponent<Projectile>().projectileSettings.velocity = rangedVelocity;

            rangedTimer = 0;

            universalTimer = 0;
        }
        */

        //Parry
        if (Input.GetButton("Fire2") && universalTimer >= universalCooldown && !isClimbing && !isRolling)
        {
            isParrying = true;

            if (isGrounded) rb.velocity = new Vector3(0f, rb.velocity.y);
            
            isMoving = false;
        }
        else if (Input.GetButtonUp("Fire2"))
        {
            isParrying = false;

            universalTimer = 0;
        }

        #endregion
        
        #region Spells

        healSpellTimer += Time.deltaTime;

        //Heal Spell
        if (Input.GetKeyDown(KeyCode.R) && healSpellTimer >= healSpellCooldown)
        {
            pa.HealSpell();

            StartCoroutine(HealSpell());

            healSpellTimer = 0;

            if (isGrounded) rb.velocity = new Vector3(0f, rb.velocity.y);

            isMoving = false;
        }
        #endregion

        #region Interaction

        Vector2 leftMaxDistance = transform.position - new Vector3(interactMaxDistance, 0);
        Vector2 rightMaxDistance = transform.position + new Vector3(interactMaxDistance, 0);

        RaycastHit2D interactRay = Physics2D.Linecast(leftMaxDistance, rightMaxDistance, interactMask);

        if (interactRay)
        {
            GameObject interactTarget = interactRay.collider.gameObject;

            if (GameObject.Find("InteractIndicator") == null)
            {
                Vector3 interactIndicatorPosition = new Vector3(interactTarget.transform.position.x, interactTarget.transform.position.y + interactIndicatorOffset);
                GameObject interactIndicator = Instantiate(interactIndicatorPrefab, interactIndicatorPosition, Quaternion.identity) as GameObject;
                interactIndicator.name = "InteractIndicator";
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                interactTarget.GetComponent<QuestNPCController>().Interact();
            }
        }
        else
        {
            if (GameObject.Find("InteractIndicator") != null)
            {
                GameObject.Destroy(GameObject.Find("InteractIndicator"));
            }
        }

        #endregion
    }

    IEnumerator HealSpell()
    {
        ph.TakeHealing(healSpellHealing);

        SetIsControllerEnabled(false);

        yield return new WaitForSeconds(healSpellFreezeDuration);

        SetIsControllerEnabled(true);
    }

    void SetFacingDirection(int direction)
    {
        if (direction == 1)
        {
            facingDirection = 1;

            horizontalOffset = new Vector3(Mathf.Abs(horizontalOffset.x), 0.0f, 0.0f);
        }
        else
        {
            facingDirection = -1;

            horizontalOffset = new Vector3(-Mathf.Abs(horizontalOffset.x), 0.0f, 0.0f);
        }
    }

    void ToggleStealth()
    {
        isStealthed = !isStealthed;
        
        //Stealth color
        if (isStealthed)
        {
            GetComponent<SpriteRenderer>().color = stealthColor;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
}
