using UnityEngine;
using System.Collections;

[RequireComponent(typeof(EntityPhysics))]
[RequireComponent(typeof(Entity))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour {

    public float speed = 10;
    public float acceleration = 100;
    public float gravity = 80;
    public float jumpSpeed = 5;
    public float minJumpHeight = 5;
    public float maxJumpHeight = 30;
    public float slideDuration = 0.5f;
    public float chargeThreshold = 3.0f;
    public Transform[] projectiles;

    private float m_currentSpeed;
    private float m_targetSpeed;
    private Vector2 m_amountToMove;

    private float m_slideStartTime;
    private float m_chargeStartTime;

    private bool m_jumping;
    private bool m_sliding;
    private bool m_charging;

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

            if (Input.GetAxisRaw("Vertical") < 0 && Input.GetButtonDown("Jump") && !m_sliding) {
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

        if (Input.GetButtonDown("Fire") && GameObject.FindGameObjectsWithTag("PlayerProjectile").Length < 3 && !m_sliding) {
            // can fire anytime except when sliding
            m_charging = true;
            Instantiate(projectiles[0], transform.position, transform.rotation);
            m_chargeStartTime = Time.time;
        }

        if (m_charging) {
            float chargeTime = Time.time - m_chargeStartTime;
            Debug.Log("chargetime: " + chargeTime + " , chargethresh: " + chargeThreshold);
            if(Input.GetButton("Fire")) {
                if (chargeTime >= chargeThreshold) {
                    Debug.Log("FULLY CHARGED");
                    m_animator.SetInteger("ChargeLevel", 2);
                }
                else if (chargeTime >= (chargeThreshold / 3.0f)) {
                    Debug.Log("HALF CHARGED");
                    m_animator.SetInteger("ChargeLevel", 1);
                }
            } else {
                m_charging = false;
                //Debug.Log("release charge");

                if (chargeTime >= chargeThreshold) {
                    //Debug.Log("PEW");
                    Instantiate(projectiles[1], transform.position, transform.rotation);
                    m_animator.SetInteger("ChargeLevel", 0);
                }
                else if (chargeTime >= chargeThreshold / 3.0f) {
                    //Debug.Log("pew");
                    Instantiate(projectiles[0], transform.position, transform.rotation);
                    m_animator.SetInteger("ChargeLevel", 0);
                }
            }
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
