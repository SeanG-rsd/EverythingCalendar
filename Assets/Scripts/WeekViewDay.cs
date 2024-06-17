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

    public void Toggle(bool value, bool isViableDay)
    {
        toggleObj.GetComponent<Toggle>().isOn = value;
        toggleObj.SetActive(isViableDay);
    }

    public void OnToggle()
    {
        Debug.Log("non click");
        OnToggleDay?.Invoke(toggleObj.GetComponent<Toggle>().isOn, this);
    }
}
