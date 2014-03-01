using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerPhysics))]
public class PlayerController : MonoBehaviour {

    public float speed;
    public float acceleration;
    public float gravity;
    public float jumpSpeed;
    public float minJumpHeight;
    public float maxJumpHeight;

    private float m_currentSpeed;
    private float m_targetSpeed;
    private Vector2 m_amountToMove;

    private bool m_jumping;
    private bool m_sliding;

    private PlayerPhysics m_playerPhysics;

	// Use this for initialization
	void Start () {
        m_playerPhysics = GetComponent<PlayerPhysics>();
	}
	
	// Update is called once per frame
	void Update () {

        // handle "sticking" when stopped horizontally
        if (m_playerPhysics.movementStopped) {
            m_targetSpeed = 0;
            m_currentSpeed = 0;
        }

        //handle grounded movement
        if (m_playerPhysics.grounded) {
            m_amountToMove.y = 0;

            if (m_jumping) {
                m_jumping = false;
            }

            // Jump
            if (Input.GetButtonDown("Jump")) {
                m_amountToMove.y = maxJumpHeight;
                m_jumping = true;
            }
        }

	    //handle input
        m_targetSpeed = Input.GetAxisRaw("Horizontal") * speed;
        m_currentSpeed = IncrementTowards(m_currentSpeed, m_targetSpeed, acceleration);

        float moveDirection = Mathf.Sign(m_targetSpeed);
        if (moveDirection != 0) {
            // Flip the character sprite if going left
            transform.eulerAngles = (moveDirection < 0) ? Vector3.up * 180 : Vector3.zero;
        }

        m_amountToMove.x = m_currentSpeed;
        m_amountToMove.y -= gravity * Time.deltaTime;
        m_playerPhysics.Move(m_amountToMove * Time.deltaTime);

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
