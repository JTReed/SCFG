using UnityEngine;
using System.Collections;

[RequireComponent(typeof(EntityPhysics))]
[RequireComponent(typeof(Entity))]
public class BasicEnemy : MonoBehaviour {

    public float speed;
    public float acceleration;
    public float gravity;

    private float currentSpeed;
    private float targetSpeed;
    private Vector2 amountToMove;

    private EntityPhysics enemyPhysics;

	// Use this for initialization
	void Start () {
        enemyPhysics = GetComponent<EntityPhysics>();

        targetSpeed = 0;
        currentSpeed = 0;
	}
	
	// Update is called once per frame
	void Update () {
        // handle "sticking" when stopped horizontally
        if (enemyPhysics.movementStopped) {
            targetSpeed = 0;
            currentSpeed = 0;
        }

        if (enemyPhysics.grounded) {
            amountToMove.y = 0;
        }

        amountToMove.x = currentSpeed;
        amountToMove.y -= gravity * Time.deltaTime;
        enemyPhysics.Move(amountToMove * Time.deltaTime);

	}

    // Increase curr toward target
    private float IncrementTowards(float curr, float target, float accel)
    {
        if (curr == target) {
            return curr;
        }

        float direction = Mathf.Sign(target - curr); // The sign of scceleration
        curr += accel * Time.deltaTime * direction;
        return (Mathf.Sign(target - curr) == direction) ? curr : target; // if curr has passed target, return target
    }

}
