using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HomeworkTimePeriod : MonoBehaviour 
{
    private int numbersOfDaysInAdvance;

    private List<Assignment> assignmentList;

    [SerializeField] private TMP_Text periodName;
    [SerializeField] private Image periodColor;

    public void Initialize(int days, string name, Color color)
    {
        numbersOfDaysInAdvance = days;
        periodName.text = name;
        periodColor.color = color;
        assignmentList = new List<Assignment>();
    }

    public int GetDays() { return numbersOfDaysInAdvance; }

    public void AddNewAssignmnet(GameObject assignmnet)
    {
        assignmentList.Add(assignmnet.GetComponent<Assignment>());
        assignmnet.transform.SetParent(transform);
    }

    public Assignment GetAssignment(string assignmentName)
    {
        for (int i = 0; i < assignmentList.Count; i++)
        {
            if (assignmentList[i].assignmentName == assignmentName)
            {
                return assignmentList[i];
            }
        }

        return null;
    }
}
