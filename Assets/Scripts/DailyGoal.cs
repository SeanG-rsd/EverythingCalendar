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
    private string goal;

    private int countPerDay;
    [SerializeField] private TMP_Text completedCountText;
    [SerializeField] private RectTransform progressMask;

    private int currentNumberOfCompletionsToday;

    public static Action<GameObject> OnClickCompleted = delegate { };
    public static Action<GameObject> OnClickUndo = delegate { };

    [SerializeField] private Image urgencyImage;
    [SerializeField] private Color[] urgencyChart;

    public void Initialize(string goal, int perDay, int currentCount, int daysLeftInWeek, int goalsLeftInWeek)
    {
        this.goal = goal;
        countPerDay = perDay;
        currentNumberOfCompletionsToday = currentCount;

        goalText.text = goal;
        completedCountText.text = currentNumberOfCompletionsToday.ToString() + "/" + countPerDay.ToString();

        UpdatePriority(daysLeftInWeek, goalsLeftInWeek);
    }

    public bool IsCompleted()
    {
        return currentNumberOfCompletionsToday >= countPerDay;
    }

    public void UpdatePriority(int daysLeftInWeek, int goalsLeftInWeek)
    {
        
        int numberOfFreeDays = 7 - daysLeftInWeek - goalsLeftInWeek;
        Debug.Log("update priority : " + numberOfFreeDays);

        if (goalsLeftInWeek == 0)
        {
            urgencyImage.color = urgencyChart[0];
        }
        else if (numberOfFreeDays == 0)
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
        completedCountText.text = currentNumberOfCompletionsToday.ToString() + "/" + countPerDay.ToString();

        UpdatePriority(daysLeftInWeek, goalsLeftInWeek);
    }

    public void CompleteGoal()
    {
        currentNumberOfCompletionsToday++;
        OnClickCompleted?.Invoke(gameObject);
    }

    public void UnCompleteGoal()
    {
        currentNumberOfCompletionsToday--;
        OnClickUndo?.Invoke(gameObject);
    }
}
