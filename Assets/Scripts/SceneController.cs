using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    //Will change our scene to the string passed in
    public void ChangeScene(string _sceneName)
    {
        SceneManager.LoadScene(_sceneName);
    }

    //Reloads the current scene we are in
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //Gets our active scenes name
    public string GetScene()
    {
        return SceneManager.GetActiveScene().name;
    }

    //Quits our game
    public void QuitGame()
    {
        Debug.Log("Game Was Quit");
        Application.Quit();
    }
}
