using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeekViewDay : MonoBehaviour
{
    [SerializeField] private GameObject toggleObj;

    public static Action<bool, WeekViewDay> OnToggleDay = delegate { };

    bool expectChange = false;

    public void Toggle(bool value)
    {
        //expectChange = value;
        toggleObj.GetComponent<Toggle>().isOn = value;
    }

    public void OnToggle()
    {
        if (!expectChange)
        {
            OnToggleDay?.Invoke(toggleObj.GetComponent<Toggle>().isOn, this);
        }
        else { expectChange = false; }
    }
}
