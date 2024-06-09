using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyGoalList : MonoBehaviour
{
    DailyGoalInfo currentInfo;

    [Header("---Field Info---")]
    [SerializeField] private GameObject taskMaker;
    [SerializeField] private TMP_InputField goalNameInput;
    [SerializeField] private TMP_InputField numberPerDayInput;
    [SerializeField] private Slider numberPerWeek;
    [SerializeField] private TMP_Text numberPerWeekText;

    [Header("---Editor---")]
    [SerializeField] private GameObject taskEditor;
    [SerializeField] private GameObject choicePrefab;
    [SerializeField] private TMP_InputField goalNameEdit;
    [SerializeField] private TMP_InputField numberPerDayEdit;
    [SerializeField] private Slider numberPerWeekEdit;
    [SerializeField] private TMP_Text numberPerWeekEditText;
    [SerializeField] private RectTransform editorContentBox;

    [SerializeField] private GameObject chooseEdit;
    [SerializeField] private GameObject editInfo;

    private List<EditChoice> editChoices;
    private int currentEditIndex = -1;

    [Header("---Visual List---")]
    [SerializeField] private GameObject dailyGoalPrefab;
    [SerializeField] private Transform goalContainer;
    [SerializeField] private TMP_Text todayText;
    [SerializeField] private RectTransform progressBarMask;
    private float progressBarMaskWidth;
    [SerializeField] private TMP_Text progressPercentText;
    private List<DailyGoal> dailyGoalList;

    private int daysLeftInTheWeek;

    // deleting and editing
    private List<int> assignmentsRemovedThisSession;

    private void Start()
    {
        currentInfo = new DailyGoalInfo();
        dailyGoalList = new List<DailyGoal>();
        editChoices = new List<EditChoice>();
        assignmentsRemovedThisSession = new List<int>();
        progressBarMaskWidth = progressBarMask.rect.width;
        LoadFromJson();
        
        LoadGoals();
        LoadEditor();

        UpdateDay(currentInfo.lastLoadedDay);
    }

    private void Awake()
    {
        DailyGoal.OnClickCompleted += HandleCompletedOne;
        DailyGoal.OnClickUndo += HandleUncompletedOne;
        EditChoice.OnChooseEdit += HandleChooseToEdit;
    }

    private void OnDestroy()
    {
        DailyGoal.OnClickCompleted -= HandleCompletedOne;
        DailyGoal.OnClickUndo -= HandleUncompletedOne;
        EditChoice.OnChooseEdit -= HandleChooseToEdit;
    }

    private void HandleCompletedOne(int index, int completions)
    {
        int newIndex = index;

        foreach (int i in assignmentsRemovedThisSession)
        {
            if (index > i)
            {
                newIndex--;
            }
        }

        currentInfo.thisDaysProgress[newIndex] = completions;
        if (currentInfo.thisDaysProgress[newIndex] == currentInfo.numberPerDay[newIndex])
        {
            currentInfo.thisWeeksProgress[newIndex]++;
            dailyGoalList[newIndex].UpdatePriority(daysLeftInTheWeek, currentInfo.numberPerWeek[newIndex] - currentInfo.thisWeeksProgress[newIndex]);
            //Debug.Log("finished a task for the day");
        }
        UpdateJson();
        UpdateProgressBar();
    }

    private void HandleUncompletedOne(int index, int completions)
    {
        int newIndex = index;

        foreach (int i in assignmentsRemovedThisSession)
        {
            if (index > i)
            {
                newIndex--;
            }
        }

        if (currentInfo.thisDaysProgress[newIndex] == currentInfo.numberPerDay[newIndex])
        {
            currentInfo.thisWeeksProgress[newIndex]--;
            dailyGoalList[newIndex].UpdatePriority(daysLeftInTheWeek, currentInfo.numberPerWeek[newIndex] - currentInfo.thisWeeksProgress[newIndex]);
            //Debug.Log(currentInfo.numberPerWeek[index] - currentInfo.thisWeeksProgress[index]);
        }
        currentInfo.thisDaysProgress[newIndex] = completions;
        UpdateJson();
        UpdateProgressBar();
    }

    private void HandleChooseToEdit(int index)
    {
        chooseEdit.SetActive(false);
        editInfo.SetActive(true);

        int newIndex = index;

        foreach (int i in assignmentsRemovedThisSession)
        {
            if (index > i)
            {
                newIndex--;
            }
        }

        goalNameEdit.text = currentInfo.goalNames[newIndex];
        numberPerDayEdit.text = currentInfo.numberPerDay[newIndex].ToString();
        numberPerWeekEdit.value = currentInfo.numberPerWeek[newIndex];
        currentEditIndex = newIndex;
        ChangeNumberPerWeekEdit();
    }

    private void UpdateProgressBar()
    {
        int goalsToday = 0;
        int progress = 0;

        for (int i = 0; i < currentInfo.numberOfGoals; i++)
        {
            if (currentInfo.thisWeeksProgress[i] < currentInfo.numberPerWeek[i])
            {
                goalsToday += currentInfo.numberPerDay[i];
                progress += currentInfo.thisDaysProgress[i];
            }
        }
        // daily progress bar
        float progressToday = progress / (float)goalsToday;
        progressPercentText.text = ((int)(progressToday * 100)).ToString() + "%";
        progressBarMask.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, progressBarMaskWidth * progressToday);
    }
    private void UpdateDay(string last)
    {
        DateTime lastSave = DateTime.Parse(last);
        DateTime dateTime = DateTime.Now;

        todayText.text = dateTime.ToString("MMM dd, yyyy");

        daysLeftInTheWeek = 7 - (dateTime.DayOfWeek == 0 ? 1 : (int)dateTime.DayOfWeek - 1);
        Debug.Log(daysLeftInTheWeek);

        if (lastSave.DayOfYear < dateTime.DayOfYear && lastSave.Year == dateTime.Year)
        {
            Debug.Log("new day");
            NewDay();
            UpdateWeek(lastSave, dateTime);
        }
        else if (lastSave.Year < dateTime.Year)
        {
            Debug.Log("new year");
            NewDay();
            UpdateWeek(lastSave, dateTime);
        }

        UpdateProgressBar();

    }

    private void UpdateWeek(DateTime last, DateTime now)
    {
        int day = 1;
        DateTime startOfLastLoadedYear = new DateTime(last.Year, 1, day);
        while ((int)startOfLastLoadedYear.DayOfWeek != 1)
        {
            day++;
            startOfLastLoadedYear = new DateTime(last.Year, 1, day);
        }

        int lastLoadedWeek = (last.DayOfYear - day) / 7;
        if (last.DayOfYear % 7 == 6) lastLoadedWeek++;

        int currentWeek = (now.DayOfYear - day) / 7;
        if (now.DayOfYear % 7 == 6) currentWeek++;

        if (last.DayOfWeek == 0)
        {
            Debug.Log("was last a sunday, NEW WEEK");
            NewWeek();
        }
        if (currentWeek > lastLoadedWeek && last.Year == now.Year)
        {
            Debug.Log("new week");
            NewWeek();
        }
        else if (currentWeek == 0 && last.Year < now.Year)
        {
            Debug.Log("new week");
            NewWeek();
        }
    }

    private void NewDay()
    {
        // check if they were completed
        for (int i = 0; i < currentInfo.numberOfGoals; i++)
        {
            // update priority for new day
            Debug.Log(i);
            dailyGoalList[i].NewDay(daysLeftInTheWeek, currentInfo.numberPerWeek[i] - currentInfo.thisWeeksProgress[i]);
        }

        // update priority for new day

        currentInfo.NewDay(); // resets daily goals
        UpdateJson();
    }

    private void NewWeek()
    {
        for (int i = 0; i < currentInfo.numberOfGoals; i++)
        {
            currentInfo.thisWeeksProgress[i] = 0;

            // update priority for new day
            dailyGoalList[i].NewDay(daysLeftInTheWeek, currentInfo.numberPerWeek[i] - currentInfo.thisWeeksProgress[i]);
        }

        // update priority for new day

        currentInfo.NewDay(); // resets daily goals
        UpdateJson();
    }

    public void AddNewGoal()
    {
        currentInfo.AddNewGoal(goalNameInput.text, (int)numberPerWeek.value, Convert.ToInt32(numberPerDayInput.text));

        GameObject newGoal = Instantiate(dailyGoalPrefab, goalContainer);
        DailyGoal dailyGoal = newGoal.GetComponent<DailyGoal>();
        dailyGoal.Initialize(goalNameInput.text, Convert.ToInt32(numberPerDayInput.text), 0, daysLeftInTheWeek, (int)numberPerWeek.value, dailyGoalList.Count);
        dailyGoalList.Add(dailyGoal);

        taskMaker.SetActive(false);

        UpdateJson();
    }

    private void UpdateJson()
    {
        currentInfo.lastLoadedDay = DateTime.Now.ToString();

        string json = JsonUtility.ToJson(currentInfo, true);
        File.WriteAllText(Application.dataPath + "/DailyInfoFile.json", json);
    }

    public void LoadFromJson()
    {
        string json = File.ReadAllText(Application.dataPath + "/DailyInfoFile.json");
        currentInfo = JsonUtility.FromJson<DailyGoalInfo>(json);
    }

    public void ToggleTaskMaker()
    {
        taskMaker.SetActive(!taskMaker.activeSelf);
    }

    public void ToggleTaskEditor()
    {
        chooseEdit.SetActive(!taskMaker.activeSelf);
        editInfo.SetActive(taskMaker.activeSelf);
        taskEditor.SetActive(!taskEditor.activeSelf);
    }

    public void ChangeNumberPerWeek()
    {
        numberPerWeekText.text = "^ Number Per Week : " + numberPerWeek.value;
    }

    public void ChangeNumberPerWeekEdit()
    {
        numberPerWeekEditText.text = "^ Number Per Week : " + numberPerWeekEdit.value;
    }

    public void FinishedEditingGoal()
    {
        int newIndex = currentEditIndex;

        foreach (int i in assignmentsRemovedThisSession)
        {
            if (currentEditIndex > i)
            {
                newIndex--;
            }
        }

        currentInfo.goalNames[newIndex] = goalNameEdit.text;
        currentInfo.numberPerDay[newIndex] = Convert.ToInt32(numberPerDayEdit.text);
        currentInfo.thisDaysProgress[newIndex] = 0;
        currentInfo.numberPerWeek[newIndex] = (int)numberPerWeekEdit.value;

        chooseEdit.SetActive(true);
        editInfo.SetActive(false);

        // update all other lists

        dailyGoalList[newIndex].Initialize(goalNameEdit.text, currentInfo.numberPerDay[newIndex], 0, daysLeftInTheWeek, currentInfo.numberPerWeek[newIndex] - currentInfo.thisWeeksProgress[newIndex], newIndex);
        editChoices[newIndex].Initialize(goalNameEdit.text, newIndex);

        UpdateJson();
        UpdateProgressBar();
    }

    public void RemoveGoal()
    {
        int newIndex = currentEditIndex;

        foreach (int i in assignmentsRemovedThisSession)
        {
            if (currentEditIndex > i)
            {
                newIndex--;
            }
        }

        Destroy(dailyGoalList[newIndex].gameObject);
        Destroy(editChoices[newIndex].gameObject);

        assignmentsRemovedThisSession.Add(newIndex);

        dailyGoalList.RemoveAt(newIndex);
        editChoices.RemoveAt(newIndex);

        currentInfo.RemoveGoal(newIndex);

        chooseEdit.SetActive(true);
        editInfo.SetActive(false);

        UpdateJson();
        UpdateProgressBar();
    }

    private void LoadGoals()
    {
        for (int i = 0; i < currentInfo.numberOfGoals; i++)
        {
            GameObject newGoal = Instantiate(dailyGoalPrefab, goalContainer);
            DailyGoal dailyGoal = newGoal.GetComponent<DailyGoal>();
            dailyGoal.Initialize(currentInfo.goalNames[i], currentInfo.numberPerDay[i], currentInfo.thisDaysProgress[i], daysLeftInTheWeek, currentInfo.numberPerWeek[i] - currentInfo.thisWeeksProgress[i], i);
            dailyGoalList.Add(dailyGoal);
        }
    }

    private void LoadEditor()
    {
        for (int i = 0; i < currentInfo.numberOfGoals; i++)
        {
            GameObject newChoice = Instantiate(choicePrefab, editorContentBox);
            EditChoice editChoice = newChoice.GetComponent<EditChoice>();
            editChoice.Initialize(currentInfo.goalNames[i], i);
            editChoices.Add(editChoice);
        }
    }
}
