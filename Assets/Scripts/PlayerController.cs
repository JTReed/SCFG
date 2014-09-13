using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(EntityPhysics))]
[RequireComponent(typeof(Entity))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour {

    // basic movement vars
    public float speed = 10;
    public float acceleration = 100;
    public float gravity = 80;
    public float jumpSpeed = 30;

    private float currentSpeed;
    private float targetSpeed;
    private Vector2 amountToMove;

    // sliding vars
    public float slideDuration = 0.5f;

    private float slideStartTime;
    private float slideSpeedMultiplier = 2.0f;

    // firing vars
    public float chargeThreshold = 3.0f;
    private float lastShotTime;
    /*private ObjectPool bulletStage1;
  	public GameObject bulletStage2;
    public GameObject bulletStage3;*/
	public BulletPool bulletPool; 

	//damage vars
	public float invincibleTime;
	private float timeDamaged;

    // state booleans
    private bool jumping;
    private bool sliding;
    private bool charging;
	private bool damaged;
	private bool invincible;

	private Entity entity;
    private EntityPhysics physics;
    private Animator animator;
	private SpriteRenderer sprite;

	// Use this for initialization
	void Start () {
        physics = GetComponent<EntityPhysics>();
		entity = GetComponent<Entity>();
        animator = GetComponent<Animator>();
        bulletPool = GetComponent<BulletPool>(); 
		sprite = GetComponent<SpriteRenderer>();
	}

    // Update is called once per frame
    void Update()
    {
        //MOVEMENT
        // handle "sticking" when stopped horizontally
        if (physics.movementStopped) {
            targetSpeed = 0;
            currentSpeed = 0;
        }
		if(!damaged) {
	        //handle grounded movement
	        if (physics.grounded) {
	            amountToMove.y = 0;

	            if (jumping) {
	                jumping = false;
					animator.SetBool("Jumping", false);
					animator.SetBool ("Falling", false);
				}

	            if (!sliding) {
	                if ((Input.GetAxisRaw("Vertical") < 0 && Input.GetButtonDown("Jump")) || Input.GetButtonDown("Slide")) {
	                    sliding = true;
	                    animator.SetBool("Sliding", true);
	                    //physics.SetSlideCollider(false);
	                    slideStartTime = Time.time;
	                }
	            }

	            // Jump
	            if (Input.GetButtonDown("Jump") && !sliding) {
	                amountToMove.y = jumpSpeed;
	                jumping = true;
					animator.SetBool("Jumping", true);
	            }
	        }

		    //handle directional input
	        if (!sliding) {
	            targetSpeed = (Input.GetAxisRaw("Horizontal") != 0) ? Mathf.Sign(Input.GetAxisRaw("Horizontal")) * speed : 0;
	            currentSpeed = IncrementTowards(currentSpeed, targetSpeed, acceleration);
	        }
	        else {
	            if (Time.time - slideStartTime >= slideDuration) {
	                // stop sliding
	                sliding = false;
	                animator.SetBool("Sliding", false);
	                //physics.SetSlideCollider(true);
	            }
	            else {
	                // move in correct direction
	                /*if (targetSpeed == 0) {
	                    // right if facing right and left if facing left
	                    currentSpeed = (transform.eulerAngles.y == 0) ? speed : -speed;
	                }*/
					currentSpeed = ((transform.eulerAngles.y == 0) ? speed : -speed) * slideSpeedMultiplier;
	            }
	        }
     
	        // need to keep track of which direction the character is facing
	        float facing = Mathf.Sign(targetSpeed);
	        if (facing != 0 && targetSpeed != 0) {
	            // Flip the character sprite if going left
	            transform.eulerAngles = (facing < 0) ? Vector3.up * 180 : Vector3.zero;
	        }

			if(invincible) {
				Debug.Log ("invincibility check");
				//blink character to show invincibility after damage
				if(Time.time - timeDamaged <= invincibleTime) {
					if((Time.frameCount % 60) % 5 == 0) {
						sprite.enabled = (sprite.enabled) ? false : true;
					}
				} else {
					Debug.Log ("invincibility done");
					sprite.enabled = true;
					invincible = false;
				}
			}
		} else {
			// Damaged
			if(!physics.grounded) {
				amountToMove.y = (amountToMove.y >= 5) ? 5 : amountToMove.y;
			}
			currentSpeed = (transform.eulerAngles == Vector3.zero) ? -8.0f : 8.0f;

			if(Time.time - timeDamaged >= (invincibleTime / 5.0f)) {
				damaged = false;
				animator.SetBool("Hit", false);
				currentSpeed = 0;
			}
		}

		animator.SetFloat("Speed", Mathf.Abs(currentSpeed));

        if (Input.GetButtonUp("Jump")) {
            amountToMove.y = (amountToMove.y >= 2) ? 5 : amountToMove.y;
        }
        amountToMove.x = currentSpeed;
        amountToMove.y -= gravity * Time.deltaTime;

		//if(jumping) Debug.Log ("jump speed: " + jumpSpeed + ", negative speed: " + amountToMove.y );
		if(jumping && amountToMove.y < -5.0) {
			animator.SetBool("Falling", true);
		}

        physics.Move(amountToMove * Time.deltaTime);

        // FIRING
        if (Input.GetButtonDown("Fire") && GameObject.FindGameObjectsWithTag("PlayerProjectile").Length < 3 && !sliding) {
            // can fire anytime except when sliding
            charging = true;
            bulletPool.CreateObject(1);
            lastShotTime = Time.time;
			animator.SetBool("Shooting", true);
        }

        if (charging) {
            float chargeTime = Time.time - lastShotTime;
            Debug.Log("chargetime: " + chargeTime + " , chargethresh: " + chargeThreshold);
            if(Input.GetButton("Fire")) {
                if (chargeTime >= chargeThreshold) {
                    Debug.Log("FULLY CHARGED");
                    animator.SetInteger("ChargeLevel", 2);
                }
                else if (chargeTime >= (chargeThreshold / 3.0f)) {
                    Debug.Log("HALF CHARGED");
                    animator.SetInteger("ChargeLevel", 1);
                }
            } else {
                charging = false;

                if (chargeTime >= chargeThreshold) {
					bulletPool.CreateObject(2);
                    animator.SetInteger("ChargeLevel", 0);
                }
                else if (chargeTime >= chargeThreshold / 3.0f) {
					bulletPool.CreateObject(3);
                    animator.SetInteger("ChargeLevel", 0);
                }
				lastShotTime = Time.time;
            }
        } else if(animator.GetBool("Shooting") && (Time.time - lastShotTime) >= 1.0f) {
			animator.SetBool("Shooting", false);
		}
	}

	void OnTriggerEnter2D(Collider2D collider)
	{
		if( collider.tag == "Enemy" && !invincible){
			Debug.Log("collided with enemy");
			damaged = true;
			invincible = true;
			animator.SetBool("Hit", true);
			entity.TakeDamage(1);
			timeDamaged = Time.time;
		}

		if( collider.tag == "Death") {
			entity.Die();
		}
	}

    // Increase curr toward target
    private float IncrementTowards(float curr, float target, float accel)
    {
        if (curr == target) {
            return curr;
        }

        float direction = Mathf.Sign(target - curr); // The sign of scceleration
        curr += 500 * Time.deltaTime * direction;
        return (Mathf.Sign(target - curr) == direction) ? curr : target; // if curr has passed target, return target
    }
}
