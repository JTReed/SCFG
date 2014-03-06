using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class EntityPhysics : MonoBehaviour {

    public LayerMask collisionMask;

    [HideInInspector]
    public bool grounded;
    [HideInInspector]
    public bool movementStopped;

    private BoxCollider2D m_collider;
    private Vector2 m_colliderSize;
    private Vector2 m_colliderCenter;
    private float m_colliderScale;

    private int m_raycastDivisions = 3;
    private float m_collisionSkin = .005f;

    private Ray2D m_ray;
    private RaycastHit2D m_hit;


	// Use this for initialization
	void Start () {
	    m_collider = GetComponent<BoxCollider2D>();
        m_colliderScale = transform.localScale.x; // Assume both x and y have same scale value

        ScaleCollider(m_collider.size, m_collider.center);
	}

    public void Move(Vector2 amountToMove)
    {
        float deltaX = amountToMove.x;
        float deltaY = amountToMove.y;
        Vector2 playerPos = transform.position;

        grounded = false; // reset each frame
        // collision UP/DOWN
        for (int i = 0; i < m_raycastDivisions; i++) {
            float direction = Mathf.Sign(deltaY); // jumping or falling?
            // player position + collider offset - half collider to reach left side of collider
            // Add an offset for each ray, creating m_raycastDivisions number of rays
            float x = (playerPos.x + m_colliderCenter.x - m_colliderSize.x / 2) + (m_colliderSize.x / (m_raycastDivisions - 1)) * i;
            // direction points toward top or bottom - default top
            float y = playerPos.y + m_colliderCenter.y + m_colliderSize.y / 2 * direction;

            // create ray and check collisions
            m_ray = new Ray2D(new Vector2(x, y), new Vector2(0, direction));
            Debug.DrawRay(m_ray.origin, m_ray.direction);

            //m_hit = Physics2D.Raycast(m_ray.origin, m_ray.direction, Mathf.Abs(deltaY) + m_collisionSkin, collisionMask);
            if(Physics2D.Raycast(m_ray.origin, m_ray.direction, Mathf.Abs(deltaY) + m_collisionSkin, collisionMask)) {
                m_hit = Physics2D.Raycast(m_ray.origin, m_ray.direction, Mathf.Abs(deltaY) + m_collisionSkin, collisionMask);
                // get distance between object and collider
                float distance = Mathf.Abs(m_ray.origin.y - m_hit.point.y);
                
                if (distance > m_collisionSkin) {
                    // not collided
                    // "warp" to skin distance away from collider
                    deltaY = (distance * direction) - (m_collisionSkin * direction);
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
            for (int i = 0; i < m_raycastDivisions; i++) {
                float direction = Mathf.Sign(deltaX); // left or right?
                float x = playerPos.x + m_colliderCenter.x + m_colliderSize.x / 2 * direction;
                float y = (playerPos.y + m_colliderCenter.y - m_colliderSize.y / 2) + (m_colliderSize.y / m_raycastDivisions) * i;

                m_ray = new Ray2D(new Vector2(x, y), new Vector2(direction, 0));
                Debug.DrawRay(m_ray.origin, m_ray.direction);

                if (Physics2D.Raycast(m_ray.origin, m_ray.direction, Mathf.Abs(deltaX) + m_collisionSkin, collisionMask)) {
                    m_hit = Physics2D.Raycast(m_ray.origin, m_ray.direction, Mathf.Abs(deltaX) + m_collisionSkin, collisionMask);

                    float distance = Mathf.Abs(m_ray.origin.x - m_hit.point.x);

                    if (distance > m_collisionSkin) {
                        deltaX = (distance * direction) - (m_collisionSkin * direction);
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
            Vector2 origin = new Vector2(playerPos.x + m_colliderCenter.x + (m_colliderSize.x / 2 * Mathf.Sign(deltaX)), 
                playerPos.y + m_colliderCenter.y + (m_colliderSize.y / 2 * Mathf.Sign(deltaY)));
            m_ray = new Ray2D(origin, playerDirection.normalized);
            Debug.DrawRay(m_ray.origin, m_ray.direction);
            if(Physics2D.Raycast(m_ray.origin, m_ray.direction, Mathf.Sqrt(deltaX * deltaX + deltaY * deltaY), collisionMask)) {
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
        m_collider.size = sz;
        m_collider.center = ct;

        m_colliderSize = sz * m_colliderScale;
        m_colliderCenter = ct * m_colliderScale;
    }

    public void SetSlideCollider(bool reset)
    {
        float tmp = m_colliderSize.x;
        m_colliderSize.x = m_colliderSize.y;
        m_colliderSize.y = tmp;

        m_colliderCenter.y = (reset) ? 0 : -m_colliderSize.y / 4.0f;

        m_collider.size = m_colliderSize / m_colliderScale;
        m_collider.center = m_colliderCenter / m_colliderScale;
    }
}
