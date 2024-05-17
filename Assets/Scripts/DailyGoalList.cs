using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

    [Header("---Visual List---")]
    [SerializeField] private GameObject dailyGoalPrefab;
    [SerializeField] private Transform goalContainer;
    [SerializeField] private TMP_Text todayText;
    [SerializeField] private RectTransform progressBarMask;
    private float progressBarMaskWidth;
    [SerializeField] private TMP_Text progressPercentText;
    private List<DailyGoal> dailyGoalList;

    private int daysLeftInTheWeek;

    private void Start()
    {
        currentInfo = new DailyGoalInfo();
        dailyGoalList = new List<DailyGoal>();
        progressBarMaskWidth = progressBarMask.rect.width;
        LoadFromJson();
        LoadGoals();

        UpdateDay(currentInfo.lastLoadedDay);
    }

    private void Awake()
    {
        DailyGoal.OnClickCompleted += HandleCompletedOne;
        DailyGoal.OnClickUndo += HandleUncompletedOne;
    }

    private void OnDestroy()
    {
        DailyGoal.OnClickCompleted -= HandleCompletedOne;
        DailyGoal.OnClickUndo -= HandleUncompletedOne;
    }

    private void HandleCompletedOne(int index, int completions)
    {
        currentInfo.thisDaysProgress[index] = completions;
        if (currentInfo.thisDaysProgress[index] == currentInfo.numberPerDay[index])
        {
            currentInfo.thisWeeksProgress[index]++;
            dailyGoalList[index].UpdatePriority(daysLeftInTheWeek, currentInfo.numberPerWeek[index] - currentInfo.thisWeeksProgress[index]);
            //Debug.Log("finished a task for the day");
        }
        UpdateJson();
        UpdateProgressBar();
    }

    private void HandleUncompletedOne(int index, int completions)
    {
        if (currentInfo.thisDaysProgress[index] == currentInfo.numberPerDay[index])
        {
            currentInfo.thisWeeksProgress[index]--;
            dailyGoalList[index].UpdatePriority(daysLeftInTheWeek, currentInfo.numberPerWeek[index] - currentInfo.thisWeeksProgress[index]);
            //Debug.Log(currentInfo.numberPerWeek[index] - currentInfo.thisWeeksProgress[index]);
        }
        currentInfo.thisDaysProgress[index] = completions;
        UpdateJson();
        UpdateProgressBar();
    }

    private void UpdateProgressBar()
    {
        int goalsToday = 0;
        int progress = 0;

        for (int i = 0; i < currentInfo.numberOfGoals; i++)
        {
            goalsToday += currentInfo.numberPerDay[i];
            progress += currentInfo.thisDaysProgress[i];
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

        daysLeftInTheWeek = 7 - (int)dateTime.DayOfWeek == 0 ? 6 : (int)dateTime.DayOfWeek - 1;

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

    public void CloseTaskMaker()
    {
        taskMaker.SetActive(false);
    }

    public void OpenTaskMaker()
    {
        taskMaker.SetActive(true);
    }

    private void LoadGoals()
    {
        DateTime today = DateTime.Now;
        for (int i = 0; i < currentInfo.numberOfGoals; i++)
        {
            GameObject newGoal = Instantiate(dailyGoalPrefab, goalContainer);
            DailyGoal dailyGoal = newGoal.GetComponent<DailyGoal>();
            dailyGoal.Initialize(currentInfo.goalNames[i], currentInfo.numberPerDay[i], currentInfo.thisDaysProgress[i], 7 - (int)today.DayOfWeek == 0 ? 6 : (int)today.DayOfWeek - 1, currentInfo.numberPerWeek[i] - currentInfo.thisWeeksProgress[i], i);
            dailyGoalList.Add(dailyGoal);
        }
    }
}
