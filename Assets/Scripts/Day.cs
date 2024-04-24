using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Day : MonoBehaviour
{
    [SerializeField] Image color;
    [SerializeField] private TMP_Text dayNumber;

    [SerializeField] private Color OnColor;
    [SerializeField] private Color OffColor;
    [SerializeField] private Color currentDayColor;

    public void SetActive(int day, bool isCurrentDay)
    {
        color.color = isCurrentDay ? currentDayColor : OnColor;
        dayNumber.text = day.ToString();
    }

    public void TurnOff()
    {
        color.color = OffColor;
        dayNumber.text = "";
    }
}
