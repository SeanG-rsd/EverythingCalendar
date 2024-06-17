using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject[] screens;

    private string key = "MENU_KEY";

    private void Start()
    {
        if (PlayerPrefs.HasKey(key))
        {
            ChangeScreen(screens[PlayerPrefs.GetInt(key)]);
        }
        else
        {
            ChangeScreen(screens[0]);
        }
    }

    public void ChangeScreen(GameObject screen)
    {
        for (int i = 0; i < screens.Length; i++)
        {
            GameObject scr = screens[i];
            if (scr != screen)
            {
                scr.SetActive(false);
            }
            else if (scr == screen)
            {
                PlayerPrefs.SetInt(key, i);
            }
        }

        screen.SetActive(true);
    }
}
