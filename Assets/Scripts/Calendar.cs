using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Calendar : MonoBehaviour
{
    [SerializeField] private Transform[] weeks;
    private List<Day> days;

    private DateTime lastLoadedMonth;

    [SerializeField] private TMP_Text monthYearTitle;

    [Header("--Events---")]
    [SerializeField] private GameObject dayViewer;
    [SerializeField] private GameObject eventMaker;

    List<DayEvent> currentEvents;
    [SerializeField] private Transform eventContainer;

    [SerializeField] private TMP_InputField eventNameInput;
    [SerializeField] private Toggle isAllDayToggle;
    [SerializeField] private TMP_InputField eventLengthInput;
    [SerializeField] private TMP_InputField eventStartTimeInput;

    [SerializeField] private TMP_InputField eventNotesInput;

    private List<int> eventsRemovedThisSession;
    private Day lastDayEdited;
    private int lastEventEdited = -1;

    private void Start()
    {
        eventsRemovedThisSession = new List<int>();
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

        monthYearTitle.text = startingDayOfMonth.ToString("MMMM, yyyy");

        int startingDay = startingDayOfMonth.DayOfWeek == 0 ? 6 : (int)startingDayOfMonth.DayOfWeek - 1;
        int daysInMonth = DateTime.DaysInMonth(year, month);

        for (int i = 1; i <= days.Count; i++)
        {
            if (i > startingDay && i <= daysInMonth + startingDay)
            {
                days[i - 1].SetActive(i - startingDay, today.Year == year && today.Month == month && today.Day == i - startingDay);
            }
            else days[i - 1].TurnOff();
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

    public void ToggleEventMaker()
    {
        eventMaker.SetActive(!eventMaker.activeSelf);
    }

    private void HandleViewDay(int index)
    {

    }
}
