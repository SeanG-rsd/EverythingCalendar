using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Assignment : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image subjectImage;

    public string assignmentName;
    private int listIndex;

    public int setTimePeriod;

    public static Action<int, Assignment> OnViewAssignment = delegate { };

    public void Initialize(string name, string subject, DateTime dueDate, int listIndex, float hours)
    {
        //subjectImage.color = subjectColor;
        nameText.text = name;
        assignmentName = name;
        this.listIndex = listIndex;
    }

    public void OnClickAssignment()
    {
        OnViewAssignment?.Invoke(listIndex, this);
    }

    public void SetTimePeriod(int period)
    {
        setTimePeriod = period;
    }
}
