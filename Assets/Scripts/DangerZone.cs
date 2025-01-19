using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DangerZone : MonoBehaviour
{
    // The target that the DangerZone is attracted to
    public Transform target;

    // Interpolation factor for movement towards the target
    public float t;

    // Speed of the DangerZone's movement
    public float speed;

    // Force applied to the rigidbody to move towards the target
    public float force;

    // Reference to the Rigidbody component attached to this GameObject
    public Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        // Initialization can go here if needed
    }

    // FixedUpdate is called once per physics update
    void FixedUpdate()
    {
        // Calculate the direction towards the target
        Vector3 directionToTarget = target.position - transform.position;

        // Normalize the direction to get a unit vector
        directionToTarget = directionToTarget.normalized;

        // Scale the normalized vector by the force to get the final force vector
        Vector3 forceVector = directionToTarget * force;

        // Apply the force to the Rigidbody
        rb.AddForce(forceVector);
    }

    // OnCollisionEnter is called when a collision occurs
    void OnCollisionEnter(Collision col)
    {
        // Check if the collided object has the "Player" tag
        if (col.transform.CompareTag("Player"))
        {
            // Reload the current scene if the Player collides with the DangerZone
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

    }
}
