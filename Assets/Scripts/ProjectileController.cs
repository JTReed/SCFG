using UnityEngine;
using System.Collections;

public class ProjectileController : MonoBehaviour {

    public float speed;
    public int damage;

    private float m_timeCreated;

    private const float m_lifeSpan = 1;

	// Use this for initialization
	void Start () {
        m_timeCreated = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(new Vector2(speed * Time.deltaTime, 0));
        if (Time.time - m_timeCreated >= m_lifeSpan) {
            Destroy(this.gameObject);
        }
	}

    void OnTriggerEnter2D(Collider2D enemy)
    {
        if (enemy.tag == "Enemy") {
            Destroy(this.gameObject);
            enemy.gameObject.GetComponent<Entity>().TakeDamage(damage);
        }
    }
}
