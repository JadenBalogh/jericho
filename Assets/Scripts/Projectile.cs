using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSettings {
    
    public Vector2 direction;
    public LayerMask targetMask;
    public int damage;
    public float velocity;
}

public class Projectile : MonoBehaviour {

    public ProjectileSettings projectileSettings = new ProjectileSettings();

    Rigidbody2D rb;

	void Start ()
    {
        rb = GetComponent<Rigidbody2D>();
	}
	
	void Update ()
    {
        rb.velocity = projectileSettings.direction * projectileSettings.velocity;
	}

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (Mathf.Pow(2, collider.gameObject.layer) == projectileSettings.targetMask.value)
        {
            if (collider.gameObject.GetComponent<PlayerHealth>())
            {
                collider.GetComponent<PlayerHealth>().TakeDamage(projectileSettings.damage, gameObject);
            }
            else if (collider.gameObject.GetComponent<EnemyHealth>())
            {
                collider.GetComponent<EnemyHealth>().TakeDamage(projectileSettings.damage);
            }
            else if (collider.gameObject.GetComponent<GuardHealth>())
            {
                collider.GetComponent<GuardHealth>().TakeDamage(projectileSettings.damage);
            }

            Destroy(gameObject);
        }
    }
}
