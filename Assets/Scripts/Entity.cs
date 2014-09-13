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
		Animator animator = GetComponent<Animator>();
		if(animator == null) {
			Destroy(gameObject);
		} else {
			animator.SetBool ("Dead", true);
		}
    }

	// called from death animation
	public void DestroyEntity()
	{
		if(gameObject.tag == "Player") {
			// temp respawn stuff
			Animator animator = GetComponent<Animator>();
			gameObject.SetActive(false);
			animator.SetBool ("Dead", false);
			gameObject.transform.position = Vector2.zero;
			gameObject.SetActive(true);
		} else {
			Destroy(gameObject);
		}
	}
}
