using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour {

    public RectTransform[] menus;

    GameObject player;
    GameObject HUDCanvas;
    bool isGamePaused;

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void SetMenu(string menuName = "")
    {
        foreach (RectTransform menu in menus)
        {
            if (menu.name == menuName)
            {
                menu.gameObject.SetActive(true);
            }
            else
            {
                menu.gameObject.SetActive(false);
            }
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void PauseGame()
    {
        isGamePaused = true;
        Time.timeScale = 0;
        player.GetComponent<PlayerController>().enabled = false;
        player.GetComponent<PlayerAnimator>().enabled = false;
        SetMenu("Pause Menu");
        SetHUDCanvasActive(false);
    }

    public void ResumeGame()
    {
        isGamePaused = false;
        Time.timeScale = 1;
        player.GetComponent<PlayerController>().enabled = true;
        player.GetComponent<PlayerAnimator>().enabled = true;
        SetHUDCanvasActive(true);
        SetMenu();
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);

        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGamePaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    void SetHUDCanvasActive(bool isActive)
    {
        if (HUDCanvas == null)
        {
            HUDCanvas = GameObject.FindGameObjectWithTag("HUDCanvas");
        }

        HUDCanvas.gameObject.SetActive(isActive);
    }
}
