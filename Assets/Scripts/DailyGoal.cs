using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class DailyGoal : MonoBehaviour
{
    [SerializeField] private TMP_Text goalText;

    private int countPerDay;
    [SerializeField] private TMP_Text completedCountText;
    [SerializeField] private RectTransform progressMask;

    private int currentNumberOfCompletionsToday;

    public static Action<int, int> OnClickCompleted = delegate { };
    public static Action<int, int> OnClickUndo = delegate { };
    public static Action<int, int, bool> OnToggleGoal = delegate { };

    [SerializeField] private Image urgencyImage;
    [SerializeField] private Color[] urgencyChart;

    private int index;
    private int weekValue;

    [SerializeField] private RectTransform progressBarMask;
    private float maskWidth;

    [SerializeField] private GameObject dayView;


    [SerializeField] private GameObject weekView;
    [SerializeField] private List<WeekViewDay> daysOfWeek;

    private int dayOfWeek;


    private void Awake()
    {
        WeekViewDay.OnToggleDay += HandleToggleView;
    }

    private void OnDestroy()
    {
        WeekViewDay.OnToggleDay -= HandleToggleView;
    }

    public void Initialize(string goal, int perDay, int currentCount, int daysLeftInWeek, int goalsLeftInWeek, int index, int weekValue)
    {
        dayOfWeek = DateTime.Now.DayOfWeek == 0 ? 6 : (int)DateTime.Now.DayOfWeek - 1;

        this.index = index;
        countPerDay = perDay;
        this.weekValue = weekValue;
        currentNumberOfCompletionsToday = currentCount;
        maskWidth = progressBarMask.rect.width;

        goalText.text = goal;
        UpdateVisual();

        UpdatePriority(daysLeftInWeek, goalsLeftInWeek);
    }

    public bool IsCompleted()
    {
        return currentNumberOfCompletionsToday == countPerDay;
    }

    public void UpdatePriority(int daysLeftInWeek, int goalsLeftInWeek)
    {      
        int numberOfFreeDays = daysLeftInWeek - goalsLeftInWeek;
        //Debug.Log("update priority for " + goalText.text + ": " + daysLeftInWeek);

        if (goalsLeftInWeek == 0 || IsCompleted())
        {
            urgencyImage.color = urgencyChart[0];
        }
        else if (numberOfFreeDays <= 0)
        {
            urgencyImage.color = urgencyChart[1];
        }
        else if (numberOfFreeDays == 1)
        {
            urgencyImage.color = urgencyChart[2];
        }
        else if (numberOfFreeDays >= 2)
        {
            urgencyImage.color = urgencyChart[3];
        }
    }

    public void NewDay(int daysLeftInWeek, int goalsLeftInWeek)
    {
        currentNumberOfCompletionsToday = 0;
        UpdateVisual();

        UpdatePriority(daysLeftInWeek, goalsLeftInWeek);
    }

    public void CompleteGoal()
    {
        if (!IsCompleted())
        {
            currentNumberOfCompletionsToday = Mathf.Clamp(currentNumberOfCompletionsToday + 1, 0, countPerDay);
            UpdateVisual();
            OnClickCompleted?.Invoke(index, currentNumberOfCompletionsToday);
        }
    }

    public void UnCompleteGoal()
    {
        if (currentNumberOfCompletionsToday > 0)
        {
            currentNumberOfCompletionsToday = Mathf.Clamp(currentNumberOfCompletionsToday - 1, 0, countPerDay);
            UpdateVisual();
            OnClickUndo?.Invoke(index, currentNumberOfCompletionsToday);
        }
    }

    public void NewWeekValue(int value)
    {
        weekValue = value;
    }

    private void UpdateVisual()
    {
        // day
        completedCountText.text = currentNumberOfCompletionsToday.ToString() + "/" + countPerDay.ToString();
        progressBarMask.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (currentNumberOfCompletionsToday / (float)countPerDay) * maskWidth);

        int value = weekValue;
        Debug.Log("update");
        // week
        for(int i = 0; i < 7; i++)
        {
            if (((value >> i) & 1) == 1)
            {
                daysOfWeek[i].Toggle(true);
            }
            else
            {
                daysOfWeek[i].Toggle(false);
            }
        }
    }

    public void SwapView(bool isDayView)
    {
        dayView.SetActive(isDayView);
        weekView.SetActive(!isDayView);

        //UpdateVisual();
    }

    private void HandleToggleView(bool state, WeekViewDay day)
    {
        if (daysOfWeek.Contains(day))
        {
            int dayIndex = daysOfWeek.IndexOf(day);
            OnToggleGoal?.Invoke(index, dayIndex, state);
        }
    }
}
