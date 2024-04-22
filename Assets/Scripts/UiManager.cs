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




    public int maxHealth = 100;
    public int health = 100;
    public Text healthText;
    public Scrollbar healthScrollbar;


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

        UpdateHealthUI();
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



    public void RecoverHealth(int _recAmount)
    {
        health += _recAmount;
        if (health > maxHealth)
            health = maxHealth;

        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = "HP: " + health.ToString();

        if (healthScrollbar != null)
        {
            // Calculate the percentage of health remaining
            float healthPercentage = (float)health / maxHealth;
            // Set the size of the scrollbar based on the percentage
            healthScrollbar.size = healthPercentage;
        }
    }
}
