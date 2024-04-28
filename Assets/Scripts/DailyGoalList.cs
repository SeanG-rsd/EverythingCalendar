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

    private void Start()
    {
        currentInfo = new DailyGoalInfo();
        LoadFromJson();
        LoadGoals();
    }

    private void UpdateDay(string last)
    {
        DateTime lastSave = DateTime.Parse(last);
        DateTime dateTime = DateTime.Now;
        
        if (lastSave.DayOfYear < dateTime.DayOfYear)
        {
            Debug.Log("new day");
        }
        else if (lastSave.Year < dateTime.Year)
        {
            Debug.Log("new year");
        }
        Debug.Log(dateTime.DayOfYear);
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

        UpdateDay(currentInfo.lastLoadedDay);
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
        for (int i = 0; i < currentInfo.numberOfGoals; i++)
        {
            GameObject newGoal = Instantiate(dailyGoalPrefab, goalContainer);
            newGoal.GetComponent<DailyGoal>().Initialize(currentInfo.goalNames[i], currentInfo.numberPerDay[i], currentInfo.thisDaysProgress[i], 0);
        }
    }
}
