using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame(){
        //Load the next scene in the build settings index.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }  

    public void QuitGame(){
        Application.Quit(); //Quit the application.
        Debug.Log("QUIT!");
    }
}
