using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour {

    public float topVirtualWidth;

    BoxCollider2D bc;
    GameObject player;
    float topPosition;

    void Start ()
    {
        bc = GetComponent<BoxCollider2D>();

        player = GameObject.FindGameObjectWithTag("Player");

        topPosition = transform.position.y + bc.bounds.extents.y + bc.offset.y;
    }
	
	void Update ()
    {
        float playerBottomPosition = player.transform.position.y + player.GetComponent<PlayerController>().GetBottomOffset().y;

        if (playerBottomPosition > topPosition)
        {
            bc.isTrigger = false;
        }
        else if (playerBottomPosition <= topPosition - topVirtualWidth)
        {
            bc.isTrigger = true;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.CompareTag("Player"))
        {
            collision.collider.gameObject.GetComponent<PlayerController>().SetCanJump(true);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.gameObject.CompareTag("Player"))
        {
            collision.collider.gameObject.GetComponent<PlayerController>().SetCanJump(false);
        }
    }
}
