using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth;
    public int meleeDamageReduction;
    public Color hitColor;
    public float hitColorDuration;
    public Color deflectedColor;
    public float deflectedColorDuration;
    public float destroySelfDelay;

    bool isDead;
    int health;
    EnemyAnimator ea;
    EnemyController ec;

    public void TakeDamage(int damage)
    {
        if (isDead)
        {
            return;
        }
        
        damage -= meleeDamageReduction;

        if (damage <= 0)
        {
            StartCoroutine(FlashGold());
        }
        else
        {
            health -= damage;

            StartCoroutine(FlashRed());
        }

        if (health <= 0)
        {
            Die();
        }
    }

    void Start()
    {
        ea = GetComponent<EnemyAnimator>();

        ec = GetComponent<EnemyController>();
        
        SetHealthToMax();
    }

    IEnumerator FlashRed()
    {
        GetComponent<SpriteRenderer>().color = hitColor;

        yield return new WaitForSeconds(hitColorDuration);

        GetComponent<SpriteRenderer>().color = Color.white;
    }

    IEnumerator FlashGold()
    {
        GetComponent<SpriteRenderer>().color = deflectedColor;

        yield return new WaitForSeconds(deflectedColorDuration);

        GetComponent<SpriteRenderer>().color = Color.white;
    }

    void SetHealthToMax()
    {
        health = maxHealth;
    }

    void Die()
    {
        isDead = true;

        ea.Die();

        Destroy(ea);
        Destroy(ec);
        Destroy(gameObject, destroySelfDelay);
    }
}
