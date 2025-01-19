using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameRespawn : MonoBehaviour
{
    // The threshold represents the y-axis position below which the respawn should occur
    public float threshold;

    // FixedUpdate is called at a fixed time interval, suitable for physics-related calculations
    void FixedUpdate()
    {
        // Check if the current object's y-axis position is below the specified threshold
        if (transform.position.y < threshold)
        {
            // If the condition is true, respawn the game by reloading the current scene
            // This assumes that each level or section of the game is a separate scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void OnCollisionEnter(Collision col)
    {

        if (col.transform.CompareTag("Bullet"))
        {
            // Reload the current scene if the Player collides with the DangerZone
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
