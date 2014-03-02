using UnityEngine;
using System.Collections;

[RequireComponent(typeof(EntityPhysics))]
[RequireComponent(typeof(Entity))]
public class BasicEnemy : MonoBehaviour {

    public float speed;
    public float acceleration;
    public float gravity;

    private float m_currentSpeed;
    private float m_targetSpeed;
    private Vector2 m_amountToMove;

    private EntityPhysics m_enemyPhysics;

	// Use this for initialization
	void Start () {
        m_enemyPhysics = GetComponent<EntityPhysics>();

        m_targetSpeed = 0;
        m_currentSpeed = 0;
	}
	
	// Update is called once per frame
	void Update () {
        // handle "sticking" when stopped horizontally
        if (m_enemyPhysics.movementStopped) {
            m_targetSpeed = 0;
            m_currentSpeed = 0;
        }

        if (m_enemyPhysics.grounded) {
            m_amountToMove.y = 0;
        }

        m_amountToMove.x = m_currentSpeed;
        m_amountToMove.y -= gravity * Time.deltaTime;
        m_enemyPhysics.Move(m_amountToMove * Time.deltaTime);

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
