using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UiManager : Singleton<UiManager>
{
    [Header("Pause Screen")]
    public GameObject pausePanels;
    public GameObject pausedMenu;
    public GameObject optionsMenu;
    public GameObject controlsMenu;
    public GameObject quitPanel;

    [Header("Win Screen")]
    public GameObject winPanel;
    public TMP_Text timeText;
    public TMP_Text damageTakenText;
    public TMP_Text rankLetter;
    float minutes;
    float seconds;

    [Header("Lose Screen")]
    public GameObject losePanel;

    [Header("Enemy Count")]
    public TMP_Text enemyCountText;

    public bool isPaused = false;
    bool gameWon = false;

    void Start()
    {
        pausePanels.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown("5"))
            _GM.CalculateRank();

        if (Input.GetButtonDown("Pause"))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (_PLAYER.state == PlayerState.Dead || gameWon)
            return;

        isPaused = !isPaused;

        if (isPaused)
        {
            pausePanels.SetActive(true);
            pausedMenu.SetActive(true);

            _GM.ToggleCursorLockState();
            Time.timeScale = 0;

            _AM.audioSource.volume = _GM.OSTVolume / 2;
        }
        else
        {
            optionsMenu.SetActive(false);
            controlsMenu.SetActive(false);
            quitPanel.SetActive(false);

            pausePanels.SetActive(false);

            _GM.ToggleCursorLockState();
            Time.timeScale = 1;

            _AM.audioSource.volume = _GM.OSTVolume;
        }
    }

    public void UpdateEnemyCount(int _enemyCount)
    {
        enemyCountText.text = "Enemies Remaining - " + _enemyCount.ToString();
    }

    void WinScreen()
    {
        _GM.isTiming = false;
        _PLAYER.inCutscene = true;
        _PLAYER.anim.SetFloat("movementSpeed", 0f);
        gameWon = true;

        _GM.CalculateRank();

        minutes = Mathf.FloorToInt(_GM.timer / 60);
        seconds = Mathf.FloorToInt(_GM.timer % 60);

        timeText.text = "Time -     " + "0" + minutes + '-' + seconds;
        damageTakenText.text = "Damage Taken -    " + _GM.damageRecieved;
        rankLetter.text = _GM.rank.ToString();

        StartCoroutine(ShowWinScreen(1f));
    }

    IEnumerator ShowWinScreen(float _waitTime)
    {
        yield return new WaitForSeconds(_waitTime);

        _PLAYER.freeLook.gameObject.SetActive(false);
        winPanel.SetActive(true);
        _GM.ToggleCursorLockState();
    }

    void LoseScreen()
    {
        StartCoroutine(ShowLoseScreen(1f));
    }

    IEnumerator ShowLoseScreen(float _waitTime)
    {
        yield return new WaitForSeconds(_waitTime);

        losePanel.SetActive(true);
        _GM.ToggleCursorLockState();
    }

    public void EnableEvent()
    {
        EnemyManager.AllEnemiesDead += WinScreen;
    }

    private void OnEnable()
    {
        PlayerMovement.PlayerDeath += LoseScreen;
    }

    private void OnDisable()
    {
        PlayerMovement.PlayerDeath -= LoseScreen;
        EnemyManager.AllEnemiesDead -= WinScreen;
    }
}
