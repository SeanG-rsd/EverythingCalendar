using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class HomeworkManager : MonoBehaviour
{
    HomeworkInfo currentInfo;
    private List<Assignment> assignments;

    [Header("---Visual---")]
    [SerializeField] private GameObject assignmentPrefab;
    [SerializeField] private GameObject timePeriodPrefab;

    [Header("---Assignment Adder")]
    [SerializeField] private GameObject addAssignmentScreen;
    [SerializeField] private TMP_InputField assignmentName;
    [SerializeField] private TMP_Dropdown subject;
    [SerializeField] private TMP_Dropdown monthDue;
    [SerializeField] private TMP_Dropdown dayDue;
    [SerializeField] private TMP_Dropdown predictedHours;

    private float[] predeictedHourPossibilities = new float[] {0.5f, 1, 1.5f, 2, 2.5f, 3, 3.5f, 4, 4.5f, 5, 5.5f, 6 };
    private string[] monthNames = new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

    private void Awake()
    {
        currentInfo = new HomeworkInfo();
        assignments = new List<Assignment>();

        UpdateHomework();
        LoadHomework();
        LoadAssignmentMaker();
    }

    private void OnDestroy()
    {
        
    }

    private void UpdateHomework()
    {
        currentInfo.lastLoadedDay = DateTime.Now.ToString();

        string json = JsonUtility.ToJson(currentInfo, true);
        File.WriteAllText(Application.dataPath + "/HomeworkInfoFile.json", json);
    }

    private void LoadHomework()
    {
        string json = File.ReadAllText(Application.dataPath + "/HomeworkInfoFile.json");
        currentInfo = JsonUtility.FromJson<HomeworkInfo>(json);
    }

    private void LoadAssignmentMaker()
    {
        monthDue.ClearOptions();
        List<TMP_Dropdown.OptionData> list = new List<TMP_Dropdown.OptionData>() {
        new TMP_Dropdown.OptionData("Month")};
        for (int i = DateTime.Now.Month; i <= 12; i++)
        {
            list.Add(new TMP_Dropdown.OptionData(monthNames[i - 1]));
        }

        monthDue.AddOptions(list);
    }

    private void AddNewAssignment(string name, DateTime dueDate, string subject, float hours)
    {
        int daysUntilDue = (DateTime.Now - dueDate).Days + 1;

        Debug.Log(daysUntilDue);
    }

    public void MonthChanged()
    {
        int checkMonth = monthDue.value + DateTime.Now.Month - 1;

        dayDue.ClearOptions();
        int numberOfDaysInMonth = DateTime.DaysInMonth(DateTime.Now.Year, checkMonth);

        List<TMP_Dropdown.OptionData> list = new List<TMP_Dropdown.OptionData>() { new TMP_Dropdown.OptionData("Day") };

        for (int i = 1; i <= numberOfDaysInMonth; i++)
        {
            list.Add(new TMP_Dropdown.OptionData(i.ToString()));
        }

        dayDue.AddOptions(list);
    }

    public void FinishMakingAssignment()
    {
        DateTime dueDate = new DateTime(DateTime.Now.Year, monthDue.value + DateTime.Now.Month - 1, dayDue.value);

        AddNewAssignment(assignmentName.text, dueDate, currentInfo.subjects[subject.value], predeictedHourPossibilities[predictedHours.value]);

        ToggleAssignmentMaker();
    }

    public void ToggleAssignmentMaker()
    {
        addAssignmentScreen.SetActive(!addAssignmentScreen.activeSelf);
        if (addAssignmentScreen.activeSelf)
        {
            assignmentName.text = "";
            subject.value = 0;
            monthDue.value = 0;
            dayDue.value = 0;
            predictedHours.value = 0;
        }
    }
}
