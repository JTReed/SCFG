using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour {

    public Transform currentStage;

    private Transform target;
    private float trackSpeed = 100;

	public void SetTarget(Transform t) 
    {
        target = t;
    }

    // updates AFTER everything else
    void LateUpdate()
    {
        if (target != null) {
            float x = IncrementTowards(transform.position.x, target.position.x, trackSpeed);
            //float y = IncrementTowards(transform.position.y, target.position.y, trackSpeed);
            transform.position = new Vector3(x, 2, transform.position.z);
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
