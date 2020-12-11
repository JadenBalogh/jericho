using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableWall : MonoBehaviour {

    public float topVirtualWidth;

    BoxCollider2D bc;
    GameObject player;
    float topPosition;
    float leftPosition;
    float rightPosition;
    
	void Start()
    {
        bc = GetComponent<BoxCollider2D>();

        player = GameObject.FindGameObjectWithTag("Player");

		topPosition = transform.position.y + bc.bounds.extents.y + bc.offset.y;

        leftPosition = transform.position.x - bc.bounds.extents.x;

        rightPosition = transform.position.x + bc.bounds.extents.x;
    }

    void Update()
    {
        float playerRightPosition = player.transform.position.x + player.GetComponent<PlayerController>().GetHorizontalOffset().x;
        float playerLeftPosition = player.transform.position.x - player.GetComponent<PlayerController>().GetHorizontalOffset().x;

        if (playerRightPosition > leftPosition && playerLeftPosition < rightPosition)
        {
            player.GetComponent<PlayerController>().SetActiveWall(gameObject);

            player.GetComponent<PlayerController>().SetIsWithinBounds(true);
        }
        else if (player.GetComponent<PlayerController>().GetActiveWall() == gameObject)
        {
            player.GetComponent<PlayerController>().SetActiveWall(null);

            player.GetComponent<PlayerController>().SetIsWithinBounds(false);
        }

        float playerBottomPosition = player.transform.position.y + player.GetComponent<PlayerController>().GetBottomOffset().y;

        if (playerBottomPosition > topPosition && !player.GetComponent<PlayerController>().GetCanClimbDown() && !player.GetComponent<PlayerController>().GetCanClimbUp() && player.GetComponent<PlayerController>().GetActiveWall() == gameObject)
        {
            bc.isTrigger = false;

            player.GetComponent<PlayerController>().SetIsClimbing(false);
        }
        else if (playerBottomPosition <= topPosition - topVirtualWidth && player.GetComponent<PlayerController>().GetCanClimbDown() && player.GetComponent<PlayerController>().GetActiveWall() == gameObject)
        {
            player.GetComponent<PlayerController>().SetCanClimbDown(false);

            player.GetComponent<PlayerController>().SetCanJump(false);
        }
        else if (playerBottomPosition <= topPosition - topVirtualWidth)
        {
            bc.isTrigger = true;
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerController>().SetCanClimbUp(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerController>().SetCanClimbUp(false);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.CompareTag("Player"))
        {
            collision.collider.gameObject.GetComponent<PlayerController>().SetCanClimbDown(true);

            collision.collider.gameObject.GetComponent<PlayerController>().SetCanJump(true);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.gameObject.CompareTag("Player"))
        {
            collision.collider.gameObject.GetComponent<PlayerController>().SetCanClimbDown(false);

            collision.collider.gameObject.GetComponent<PlayerController>().SetCanJump(false);
        }
    }
}
