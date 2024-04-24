using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToDoTask : MonoBehaviour
{
    private bool hasBeenCompleted;
    [SerializeField] private TMP_Text taskInfo;
    [SerializeField] private Image completedImage;

    [SerializeField] private Color completedColor;
    [SerializeField] private Color uncompletedColor;

    public static Action<GameObject, bool> OnToggleCompleted = delegate { };
    public static Action<GameObject, bool> OnDeleteTask = delegate { };

    public void Initialize(bool completed, string task)
    {
        hasBeenCompleted = completed;
        taskInfo.text = task;
        completedImage.color = hasBeenCompleted ? completedColor : uncompletedColor;
    }
    public void ClickCompleted()
    {
        hasBeenCompleted = !hasBeenCompleted;
        UpdateStatus();
    }

    private void UpdateStatus()
    {
        completedImage.color = hasBeenCompleted ? completedColor : uncompletedColor;
        OnToggleCompleted?.Invoke(gameObject, hasBeenCompleted);
    }

    public string GetTask()
    {
        return taskInfo.text;
    }

    public void OnClickDelete()
    {
        OnDeleteTask?.Invoke(gameObject, hasBeenCompleted);
    }
}
