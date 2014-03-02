using UnityEngine;
using System.Collections;

[RequireComponent(typeof(EntityPhysics))]
[RequireComponent(typeof(Entity))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour {

    public float speed;
    public float acceleration;
    public float gravity;
    public float jumpSpeed;
    public float minJumpHeight;
    public float maxJumpHeight;
    public float slideDuration;
    public Transform[] projectiles;

    private float m_currentSpeed;
    private float m_targetSpeed;
    private Vector2 m_amountToMove;

    private float m_slideStartTime;

    private bool m_jumping;
    private bool m_sliding;

    private EntityPhysics m_physics;
    private Animator m_animator;

	// Use this for initialization
	void Start () {
        m_physics = GetComponent<EntityPhysics>();
        m_animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        // handle "sticking" when stopped horizontally
        if (m_physics.movementStopped) {
            m_targetSpeed = 0;
            m_currentSpeed = 0;
        }

        //handle grounded movement
        if (m_physics.grounded) {
            m_amountToMove.y = 0;

            if (m_jumping) {
                m_jumping = false;
            }

            if (Input.GetAxisRaw("Vertical") < 0 && Input.GetButtonDown("Jump")) {
                m_sliding = true;
                m_animator.SetBool("Sliding", true);
                m_slideStartTime = Time.time;
            }

            // Jump
            if (Input.GetButtonDown("Jump") && !m_sliding) {
                m_amountToMove.y = maxJumpHeight;
                m_jumping = true;
            }
        }

	    //handle input
        if (!m_sliding) {
            m_targetSpeed = Input.GetAxisRaw("Horizontal") * speed;
            m_currentSpeed = IncrementTowards(m_currentSpeed, m_targetSpeed, acceleration);
        }
        else {
            if (Time.time - m_slideStartTime >= slideDuration) {
                // stop sliding
                m_sliding = false;
                m_animator.SetBool("Sliding", false);
            }
            else {
                // move in correct direction
                if (m_targetSpeed == 0) {
                    // right if facing right and left if facing left
                    m_currentSpeed = (transform.eulerAngles.y == 0) ? speed : -speed;
                }
            }
        }
     
        float facing = Mathf.Sign(m_targetSpeed);
        if (facing != 0 && m_targetSpeed != 0) {
            // Flip the character sprite if going left
            transform.eulerAngles = (facing < 0) ? Vector3.up * 180 : Vector3.zero;
        }

        m_amountToMove.x = m_currentSpeed;
        m_amountToMove.y -= gravity * Time.deltaTime;
        m_physics.Move(m_amountToMove * Time.deltaTime);

        if (Input.GetButtonDown("Fire") && GameObject.FindGameObjectsWithTag("PlayerProjectile").Length < 3) {
            Instantiate(projectiles[0], transform.position, transform.rotation);
        }
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
