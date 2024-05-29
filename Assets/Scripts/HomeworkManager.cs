using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System.Runtime.CompilerServices;

public class HomeworkManager : MonoBehaviour
{
    HomeworkInfo currentInfo;
    private List<HomeworkTimePeriod> timePeriods;

    [Header("---Visual---")]
    [SerializeField] private GameObject assignmentPrefab;
    [SerializeField] private GameObject timePeriodPrefab;
    [SerializeField] private Transform timePeriodContainer;

    private int[] timePeriodWindows = new int[] { 0, 1, 2, 3, 7, 14 };
    [SerializeField] private Color[] timePeriodColors;
    [SerializeField] private string[] timePeriodNames;

    [Header("---Assignment Adder")]
    [SerializeField] private GameObject addAssignmentScreen;
    [SerializeField] private TMP_InputField assignmentName;
    [SerializeField] private TMP_Dropdown subject;
    [SerializeField] private TMP_Dropdown monthDue;
    [SerializeField] private TMP_Dropdown dayDue;
    [SerializeField] private TMP_Dropdown predictedHours;

    [Header("---Assignment Viewer")]
    [SerializeField] private TMP_InputField assignmentNotesInput;
    [SerializeField] private GameObject assignmentViewerScreen;

    private float[] predeictedHourPossibilities = new float[] {0.5f, 1, 1.5f, 2, 2.5f, 3, 3.5f, 4, 4.5f, 5, 5.5f, 6 };
    private string[] monthNames = new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };


    private void Awake()
    {
        currentInfo = new HomeworkInfo();
        timePeriods = new List<HomeworkTimePeriod>();

        LoadHomework();
        UpdateHomework();

        LoadAssignmentMaker();

        LoadVisual();
    }

    private void OnDestroy()
    {

    }

    private void HandleEditAssignment(int timePeriod, string assignmentName)
    {
        assignmentViewerScreen.SetActive(true);

        Assignment assignment = timePeriods[timePeriod].GetAssignment(assignmentName);
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

    private void LoadVisual()
    {
        for (int i = currentInfo.numberOfAssignments - 1; i >= 0; i--)
        {
            if (DateTime.Now.DayOfYear <= DateTime.Parse(currentInfo.dueDates[i]).DayOfYear)
            {
                Debug.Log("new ");
                AddNewAssignment(currentInfo.homeworkNames[i], DateTime.Parse(currentInfo.dueDates[i]), currentInfo.subject[i], currentInfo.predictedHours[i]);
            }
            else // remove assignment
            {
                currentInfo.RemoveAssignment(i);
            }
        }
    }

    private void AddNewAssignment(string name, DateTime dueDate, string subject, float hours)
    {
        int daysUntilDue = dueDate.DayOfYear - DateTime.Now.DayOfYear;
        Debug.Log(daysUntilDue);

        GameObject assignment = Instantiate(assignmentPrefab);
        assignment.GetComponent<Assignment>().Initialize(name, new Color(0, 0, 0), dueDate);

        int timePeriod = -1;
        int index = -1;

        for (int i = timePeriodWindows.Length - 1; i >= 0; i--)
        {
            if (daysUntilDue >= timePeriodWindows[i])
            {
                timePeriod = timePeriodWindows[i];
                index = i;
                break;
            }
        }

        Debug.Log(timePeriod);
        UpdateHomework();

        for (int i = timePeriods.Count - 1; i >= 0; i--)
        {
            if (timePeriods[i].GetDays() == timePeriod)
            {
                timePeriods[i].AddNewAssignmnet(assignment);
                assignment.GetComponent<Assignment>().SetIndex(i);
                return;
            }
            else if (timePeriod > timePeriods[i].GetDays()) // expected time period does not exist must add a new one
            {
                Debug.Log("new time period : " + i);
                GameObject newTimePeriod = Instantiate(timePeriodPrefab, timePeriodContainer);
                newTimePeriod.GetComponent<HomeworkTimePeriod>().Initialize(timePeriod, timePeriodNames[index], timePeriodColors[index]);
                timePeriods.Insert(i + 1, newTimePeriod.GetComponent<HomeworkTimePeriod>());
                newTimePeriod.transform.SetSiblingIndex(i + 1);
                newTimePeriod.GetComponent<HomeworkTimePeriod>().AddNewAssignmnet(assignment);
                assignment.GetComponent<Assignment>().SetIndex(i + 1);
                return;
            }
        }

        GameObject tp = Instantiate(timePeriodPrefab, timePeriodContainer);
        tp.GetComponent<HomeworkTimePeriod>().Initialize(timePeriod, timePeriodNames[index], timePeriodColors[index]);
        timePeriods.Insert(0, tp.GetComponent<HomeworkTimePeriod>());
        tp.transform.SetSiblingIndex(0);
        assignment.GetComponent<Assignment>().SetIndex(0);
        tp.GetComponent<HomeworkTimePeriod>().AddNewAssignmnet(assignment);
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
        currentInfo.AddNewAssignment(assignmentName.text, dueDate.ToString(), currentInfo.subjects[subject.value], predeictedHourPossibilities[predictedHours.value]);

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

    public void EditAssignmentNotes()
    {

    }

    public void RemoveCurrentAssignment()
    {

    }
}
