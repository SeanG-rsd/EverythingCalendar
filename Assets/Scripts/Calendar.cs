using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Calendar : MonoBehaviour
{
    [SerializeField] private Transform[] weeks;

    private List<Day> days;

    private DateTime lastLoadedMonth;

    [SerializeField] private TMP_Text monthYearTitle;

    private string[] monthNames = new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"};

    private void Start()
    {
        DateTime today = DateTime.Now;

        InitializeCalendar();

        SetCalendar(today.Year, today.Month);
    }

    private void InitializeCalendar()
    {
        days = new List<Day>();

        foreach (Transform week in weeks)
        {
            for (int i = 0; i < week.childCount; i++)
            {
                days.Add(week.GetChild(i).GetComponent<Day>());
            }
        }
    }

    private void SetCalendar(int year, int month)
    {
        DateTime startingDayOfMonth = new (year, month, 1);
        lastLoadedMonth = startingDayOfMonth;
        DateTime today = DateTime.Now;

        monthYearTitle.text = monthNames[lastLoadedMonth.Month - 1] + ", " + lastLoadedMonth.Year.ToString();

        int startingDay = (int)startingDayOfMonth.DayOfWeek;
        int daysInMonth = DateTime.DaysInMonth(year, month);

        for (int i = 0; i < days.Count; i++)
        {
            if (i >= startingDay && i < daysInMonth + startingDay)
            {
                days[i].SetActive(i - startingDay + 1, today.Year == year && today.Month == month && today.Day == i);
            }
            else days[i].TurnOff();
        }
    }

    public void ChangeMonth(int value)
    {
        int newMonth = lastLoadedMonth.Month + value;
        int newYear = lastLoadedMonth.Year;
        if (newMonth < 1)
        {
            newYear--;
            newMonth = 12;
        }
        else if (newMonth > 12)
        {
            newYear++;
            newMonth = 1;
        }

        SetCalendar(newYear, newMonth);
    }
}
