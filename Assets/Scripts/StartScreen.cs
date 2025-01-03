using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScreen : GameBehaviour
{
    public GameObject startingScreen;
    public GameObject mainMenu;
    public GameObject mainFirstButton;

    public UiButtons uiButtons;

    void Start()
    {
        Time.timeScale = 1;
        startingScreen.SetActive(true);

        StartCoroutine(WaitForAnimToEnd(4.5f));
    }

    IEnumerator WaitForAnimToEnd(float waitTime)
    {
        // Wait for the specified duration
        yield return new WaitForSeconds(waitTime);

        // Do something after waiting
        Debug.Log("Waited for " + waitTime + " seconds. Now do something.");

        

        startingScreen.SetActive(false);
        uiButtons.SetFirstButton(mainFirstButton);
        mainMenu.SetActive(true);
    }
}
