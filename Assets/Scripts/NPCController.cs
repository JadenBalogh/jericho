using UnityEngine;

public class NPCController : MonoBehaviour {
    
    public Transform[] moveLocations;
    public FloatRange idleDurationRange;
    public float patrolVelocity;
    public float patrolDragVelocity;

    Rigidbody2D rb;

    Transform moveLocation;
    int locationIndex;
    int facingDirection;
    float idleDuration;
    float idleTimer;
    bool isIdle;

    public int GetFacingDirection()
    {
        return facingDirection;
    }

    void Start ()
    {
        rb = GetComponent<Rigidbody2D>();

        locationIndex = GetRandomLocationIndex();
        moveLocation = moveLocations[locationIndex];

        idleDuration = Random.Range(idleDurationRange.min, idleDurationRange.max);
	}
	
	void FixedUpdate ()
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
