using UnityEngine;
using System.Collections;

public class ProjectileController : MonoBehaviour {

    public float speed = 30;
    public int damage = 1;

    private float timeCreated;

    private const float lifeSpan = 1;


    void Awake()
    {

    }

    void OnEnable()
    {
        timeCreated = Time.time;
    }

    void OnDisable()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector2(speed * Time.deltaTime, 0));
        if (Time.time - timeCreated >= lifeSpan) {
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
