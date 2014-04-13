using UnityEngine;
using System.Collections;

public class ProjectileController : MonoBehaviour {

    public float speed = 30;
    public int damage = 1;

    private float m_timeCreated;

    private const float m_lifeSpan = 1;


    void Awake()
    {

    }

    void OnEnable()
    {
        m_timeCreated = Time.time;
    }

    void OnDisable()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector2(speed * Time.deltaTime, 0));
        if (Time.time - m_timeCreated >= m_lifeSpan) {
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D enemy)
    {
        if (enemy.tag == "Enemy") {
            enemy.gameObject.GetComponent<Entity>().TakeDamage(damage);
            gameObject.SetActive(false);
        }
    }
}
