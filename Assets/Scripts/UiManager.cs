using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : Singleton<UiManager>
{

    public GameObject blackBackground;
    public GameObject creamGradient;
    public GameObject logoCream;
    public GameObject logoNormal;

    public GameObject GamesText;

    public GameObject startingScreen;


    public GameObject mainMenu;

    public GameObject gameScreen;


    public GameObject quitAreYouSure;

    public int maxHealth = 100;
    public int health = 100;
    public Text healthText;
    public Scrollbar healthScrollbar;

    public int healthUi;

    private bool escPress = false;


    void Start()
    {
        startingScreen.SetActive(true);

        StartCoroutine(WaitForAnimToEnd(4.5f));

        blackBackground.SetActive(true);
        creamGradient.SetActive(true);
        GamesText.SetActive(true);

        logoCream.SetActive(true);
        logoNormal.SetActive(true);


        mainMenu.SetActive(false);

        gameScreen.SetActive(false);

        quitAreYouSure.SetActive(false);





    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ESC Pressed");
            if (startingScreen.activeSelf && !mainMenu.activeSelf && !gameScreen.activeSelf)
            {
                QuitGame();
            }
            else if (mainMenu.activeSelf && !startingScreen.activeSelf && !gameScreen.activeSelf)
            {
                quitAreYouSure.SetActive(true);
            }
            else if (gameScreen.activeSelf && !mainMenu.activeSelf && !startingScreen.activeSelf)
            {
                InGamePause();
            }
        }
    }

    IEnumerator WaitForAnimToEnd(float waitTime)
    {
        // Wait for the specified duration
        yield return new WaitForSeconds(waitTime);

        // Do something after waiting
        Debug.Log("Waited for " + waitTime + " seconds. Now do something.");

        startingScreen.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void UpdateHealthUI()
    {
        Debug.Log("health " + healthUi);
    }

    //Pressing Esc Different Functions For Different States



    //Main Menu Buttons
    public void StartGame()
    {

    }

    public void OptionsMainMenu()
    {

    }

    public void Credits()
    {

    }

    //In Game Buttons

    public void InGamePause()
    {

    }








    //Quit Buttons

    public void QuitYes()
    {
        QuitGame();
    }

    public void QuitNo()
    {
        quitAreYouSure.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit");
    } 
}
