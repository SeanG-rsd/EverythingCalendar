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
    private int index;

    public static Action<int, string> OnViewAssignment = delegate { };

    public void Initialize(string name, Color subjectColor, DateTime dueDate)
    {
        //subjectImage.color = subjectColor;
        nameText.text = name;
        assignmentName = name;
    }

    public void SetIndex(int index)
    {
        this.index = index;
    }

    public void OnClickAssignment()
    {
        OnViewAssignment?.Invoke(index, assignmentName);
    }
}
