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
    DateTime lastLoadedTime;

    [Header("---Field Info---")]
    [SerializeField] private GameObject taskMaker;
    [SerializeField] private TMP_InputField goalNameInput;
    [SerializeField] private TMP_InputField numberPerDayInput;
    [SerializeField] private Slider numberPerWeek;

    [Header("---Visual List---")]
    [SerializeField] private GameObject dailyGoalPrefab;
    [SerializeField] private Transform goalContainer;
    private List<DailyGoal> dailyGoalList;

    private void Start()
    {
        currentInfo = new DailyGoalInfo();
        dailyGoalList = new List<DailyGoal>();
        LoadFromJson();
        LoadGoals();

        UpdateDay(currentInfo.lastLoadedDay);

        DateTime startOfLastLoadedYear = new DateTime(DateTime.Now.Year, 1, 1);
        Debug.Log(startOfLastLoadedYear.ToString());
        Debug.Log(startOfLastLoadedYear.DayOfYear);
    }

    private void Awake()
    {
        
    }

    private void OnDestroy()
    {
        
    }

    private void UpdateDay(string last)
    {
        DateTime lastSave = DateTime.Parse(last);
        DateTime dateTime = DateTime.Now;
        
        if (lastSave.DayOfYear < dateTime.DayOfYear && lastSave.Year == dateTime.Year)
        {
            Debug.Log("new day");
            NewDay(dateTime);
            UpdateWeek(lastSave, dateTime);
        }
        else if (lastSave.Year < dateTime.Year)
        {
            Debug.Log("new year");
            NewDay(dateTime);
            UpdateWeek(lastSave, dateTime);
        }
        Debug.Log(dateTime.DayOfYear);

        
    }

    private void UpdateWeek(DateTime last, DateTime now) // NEED TO COME BACK TO THIS
    {
        int day = 1;
        DateTime startOfLastLoadedYear = new DateTime(last.Year, 1, day);
        while ((int)startOfLastLoadedYear.DayOfWeek != 1)
        {
            day++;
            startOfLastLoadedYear = new DateTime(last.Year, 1, day);
        }

        int lastLoadedWeek = (last.DayOfYear - day) / 7;
        if (last.DayOfYear % 7 == 0) lastLoadedWeek--;

        int currentWeek = (now.DayOfYear - day) / 7;
        if (now.DayOfYear % 7 == 0) currentWeek--;

        if (last.DayOfWeek == 0)
        {
            Debug.Log("was last a sunday");
        }
        if (currentWeek > lastLoadedWeek && last.Year == now.Year)
        {
            Debug.Log("new week");
        }
        else if (currentWeek == 0 && now.DayOfYear - last.DayOfWeek == 0)
        {

        }
        else if (last.Year > now.Year && now.DayOfYear > 7)
        {
            Debug.Log("new week");
        }
    }

    private void NewDay(DateTime today)
    {
        // check if they were completed
        for (int i = 0; i < currentInfo.numberOfGoals; i++)
        {
            // update completed times per week
            if (dailyGoalList[i].IsCompleted())
            {
                currentInfo.thisWeeksProgress[i]++;
                Debug.Log("did complete a task yesterday");
            }

            // update priority for new day
            dailyGoalList[i].NewDay(7 - (int)today.DayOfWeek == 0 ? 6 : (int)today.DayOfWeek - 1, currentInfo.numberPerWeek[i] - currentInfo.thisWeeksProgress[i]);
        }

        // update priority for new day

        currentInfo.NewDay(); // resets daily goals
    }

    public void SaveToJson()
    {
        currentInfo.AddNewGoal(goalNameInput.text, (int)numberPerWeek.value, Convert.ToInt32(numberPerDayInput.text));

        currentInfo.lastLoadedDay = DateTime.Now.ToString();
        //currentInfo = new DailyGoalInfo();

        string json = JsonUtility.ToJson(currentInfo, true);
        File.WriteAllText(Application.dataPath + "/DailyInfoFile.json", json);
    }

    public void LoadFromJson()
    {
        string json = File.ReadAllText(Application.dataPath + "/DailyInfoFile.json");
        currentInfo = JsonUtility.FromJson<DailyGoalInfo>(json);

        Debug.Log(currentInfo.goalNames[currentInfo.numberOfGoals - 1]);
        Debug.Log(currentInfo.lastLoadedDay);

        
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
            dailyGoal.Initialize(currentInfo.goalNames[i], currentInfo.numberPerDay[i], currentInfo.thisDaysProgress[i], 7 - (int)today.DayOfWeek == 0 ? 6 : (int)today.DayOfWeek - 1, currentInfo.numberPerWeek[i] - currentInfo.thisWeeksProgress[i]);
            dailyGoalList.Add(dailyGoal);
            Debug.Log("added");
        }
    }
}
