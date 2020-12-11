using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

    public int maxHealth;
    public Color hitColor;
    public float hitColorDuration;
    public Color deflectedColor;
    public float deflectedColorDuration;
    public float respawnTime;

    HUDController hudController;
    Transform spawnPosition;
    PlayerController pc;
    PlayerAnimator pa;
    int health;
    bool dead;

    public bool GetIsDead()
    {
        return dead;
    }

    public void TakeDamage(int damage, GameObject enemy)
    {
        pc.SetIsStealthed(false);

        if (pc.GetIsParrying() &&
           (enemy.transform.position.x < transform.position.x && pc.GetFacingDirectionWithMouse() == -1 || 
           enemy.transform.position.x >= transform.position.x && pc.GetFacingDirectionWithMouse() == 1))
        {
            pa.ParryHit();

            damage -= pc.parryAmount;
        }

        if (damage <= 0)
        {
            StartCoroutine(FlashGold());
        }
        else
        {
            health -= damage;

            StartCoroutine(FlashRed());
        }

        hudController.SetHealth(health, maxHealth);

        if (health <= 0 && !dead)
        {
            Die();
        }
    }

    public void TakeHealing(int healing)
    {
        health = health + healing > maxHealth ? maxHealth : health + healing;

        hudController.SetHealth(health, maxHealth);
    }

    void Start()
    {
        hudController = GameObject.FindGameObjectWithTag("HUDCanvas").GetComponent<HUDController>();

        spawnPosition = GameObject.FindGameObjectWithTag("SpawnPoint").transform;

        pc = GetComponent<PlayerController>();

        pa = GetComponent<PlayerAnimator>();

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

        hudController.SetHealth(health, maxHealth);
    }

    void Die()
    {
        dead = true;

        pc.SetIsControllerEnabled(false);

        Invoke("Respawn", respawnTime);
    }

    void Respawn()
    {
        dead = false;

        pc.SetIsControllerEnabled(true);

        SetHealthToMax();

        transform.position = spawnPosition.position;
    }
}
