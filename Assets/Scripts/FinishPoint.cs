using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishPoint : MonoBehaviour
{
    public string scenename;

    private void OnTriggerEnter(Collider collision){
        if (collision.CompareTag("Player")){ //Check Collision with Objects that have a Player Tag
            SceneManager.LoadScene(scenename); //Load a scene based off name.
        }
    }
}
