using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class EntityPhysics : MonoBehaviour {

    public LayerMask collisionMask;

    [HideInInspector]
    public bool grounded;
    [HideInInspector]
    public bool movementStopped;

    private BoxCollider2D collider;
    private Vector2 colliderSize;
    private Vector2 colliderCenter;
    private float colliderScale;

    private int raycastDivisions = 3;
    private float collisionSkin = .005f;

    private Ray2D ray;
    private RaycastHit2D hit;


	// Use this for initialization
	void Start () 
	{
	    collider = GetComponent<BoxCollider2D>();
        colliderScale = transform.localScale.x; // Assume both x and y have same scale value

        ScaleCollider(collider.size, collider.center);
	}

    public void Move(Vector2 amountToMove)
    {
        float deltaX = amountToMove.x;
        float deltaY = amountToMove.y;
        Vector2 playerPos = transform.position;

        grounded = false; // reset each frame
        // collision UP/DOWN
        for (int i = 0; i < raycastDivisions; i++) {
            float direction = Mathf.Sign(deltaY); // jumping or falling?
            // player position + collider offset - half collider to reach left side of collider
            // Add an offset for each ray, creating raycastDivisions number of rays
            float x = (playerPos.x + colliderCenter.x - colliderSize.x / 2) + (colliderSize.x / (raycastDivisions - 1)) * i;
            // direction points toward top or bottom - default top
            float y = playerPos.y + colliderCenter.y + colliderSize.y / 2 * direction;

            // create ray and check collisions
            ray = new Ray2D(new Vector2(x, y), new Vector2(0, direction));
            Debug.DrawRay(ray.origin, ray.direction);

            //hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Abs(deltaY) + collisionSkin, collisionMask);
            if(Physics2D.Raycast(ray.origin, ray.direction, Mathf.Abs(deltaY) + collisionSkin, collisionMask)) {
                hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Abs(deltaY) + collisionSkin, collisionMask);
                // get distance between object and collider
                float distance = Mathf.Abs(ray.origin.y - hit.point.y);
                
                if (distance > collisionSkin) {
                    // not collided
                    // "warp" to skin distance away from collider
                    deltaY = (distance * direction) - (collisionSkin * direction);
                }
                else {
                    // collided
                    deltaY = 0;
                }
                grounded = true;
                break;
            }
        }
        
        // collision LEFT/RIGHT
        movementStopped = false;
        if(deltaX != 0) {
            for (int i = 0; i < raycastDivisions; i++) {
                float direction = Mathf.Sign(deltaX); // left or right?
                float x = playerPos.x + colliderCenter.x + colliderSize.x / 2 * direction;
                float y = (playerPos.y + colliderCenter.y - colliderSize.y / 2) + (colliderSize.y / raycastDivisions) * i;

                ray = new Ray2D(new Vector2(x, y), new Vector2(direction, 0));
                Debug.DrawRay(ray.origin, ray.direction);

                if (Physics2D.Raycast(ray.origin, ray.direction, Mathf.Abs(deltaX) + collisionSkin, collisionMask)) {
                    hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Abs(deltaX) + collisionSkin, collisionMask);

                    float distance = Mathf.Abs(ray.origin.x - hit.point.x);

                    if (distance > collisionSkin) {
                        deltaX = (distance * direction) - (collisionSkin * direction);
                    }
                    else {
                        deltaX = 0;
                    }
                    movementStopped = true;
                    break;
                }
            }     
        }
        // collision DIR OF MOVEMENT when in air
        if (!grounded && !movementStopped) {
            Vector2 playerDirection = new Vector2(deltaX, deltaY);
            Vector2 origin = new Vector2(playerPos.x + colliderCenter.x + (colliderSize.x / 2 * Mathf.Sign(deltaX)), 
                playerPos.y + colliderCenter.y + (colliderSize.y / 2 * Mathf.Sign(deltaY)));
            ray = new Ray2D(origin, playerDirection.normalized);
            Debug.DrawRay(ray.origin, ray.direction);
            if(Physics2D.Raycast(ray.origin, ray.direction, Mathf.Sqrt(deltaX * deltaX + deltaY * deltaY), collisionMask)) {
                grounded = true;
                deltaY = 0;
            }

        }


        Vector2 moveVector = new Vector2(deltaX, deltaY);
        transform.Translate(moveVector, Space.World);
    }

    // Modify calculations to take rascaled sprites into account
    private void ScaleCollider(Vector2 sz, Vector2 ct)
    {
        collider.size = sz;
        collider.center = ct;

        colliderSize = sz * colliderScale;
        colliderCenter = ct * colliderScale;
    }

    public void SetSlideCollider(bool reset)
    {
        float tmp = colliderSize.x;
        colliderSize.x = colliderSize.y;
        colliderSize.y = tmp;

        colliderCenter.y = (reset) ? 0 : -colliderSize.y / 4.0f;

        collider.size = colliderSize / colliderScale;
        collider.center = colliderCenter / colliderScale;
    }


}
