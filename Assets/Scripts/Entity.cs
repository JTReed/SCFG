using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour {

    public int health;

    public void TakeDamage(int dmg)
    {
        health -= dmg;
        Debug.Log(transform.name + " is at " + health + " health");
        if (health <= 0) {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log(transform.name + " is dead");
        Destroy(gameObject);
    }
}
