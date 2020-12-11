using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {

    public RectTransform[] menus;

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void SetMenu(string menuName)
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

    public void CloseMenus()
    {
        SetMenu(string.Empty);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
