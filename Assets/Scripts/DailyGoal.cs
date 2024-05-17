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

    [SerializeField] private Image urgencyImage;
    [SerializeField] private Color[] urgencyChart;

    private int index;

    public void Initialize(string goal, int perDay, int currentCount, int daysLeftInWeek, int goalsLeftInWeek, int index)
    {
        this.index = index;
        countPerDay = perDay;
        currentNumberOfCompletionsToday = currentCount;

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
        //Debug.Log("update priority : " + numberOfFreeDays);

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

    private void UpdateVisual()
    {
        completedCountText.text = currentNumberOfCompletionsToday.ToString() + "/" + countPerDay.ToString();
    }
}
