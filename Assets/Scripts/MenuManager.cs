using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject[] screens;

    public void ChangeScreen(GameObject screen)
    {
        foreach (GameObject scr in screens)
        {
            if (scr != screen)
            {
                scr.SetActive(false);
            }
        }

        screen.SetActive(true);
    }
}
