using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    public void Initialize(string goal, int perDay, int currentCount, int urgencyLevel)
    {
        this.goal = goal;
        countPerDay = perDay;
        currentNumberOfCompletionsToday = currentCount;
    }
}
