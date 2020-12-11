using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public float rightBoundary;
    public float leftBoundary;

    Vector3 velocity = Vector3.zero;
    GameObject player;
    Rigidbody rb;

	void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        rb = GetComponent<Rigidbody>();
	}
	
	void Update ()
    {
        float offset = GetComponent<Camera>().orthographicSize * GetComponent<Camera>().aspect;
        
        if (player.transform.position.x < rightBoundary - offset && player.transform.position.x > leftBoundary + offset)
        {
            Vector3 targetPosition = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);

            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, 0.1f);
        }
        else if (player.transform.position.x > rightBoundary - offset)
        {
            Vector3 targetPosition = new Vector3(rightBoundary - offset, player.transform.position.y, transform.position.z);

            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, 0.1f);
        }
        else if (player.transform.position.x < leftBoundary + offset)
        {
            Vector3 targetPosition = new Vector3(leftBoundary + offset, player.transform.position.y, transform.position.z);

            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, 0.1f);
        }
    }
}
