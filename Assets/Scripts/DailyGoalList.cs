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

    [Header("---Week View---")]
    [SerializeField] private Toggle viewToggle;
    [SerializeField] private Animator toggleAnim;

    private int daysLeftInTheWeek;

    private string key = "DAILY_GOAL_KEY";

    // deleting and editing
    private List<int> assignmentsRemovedThisSession;
    int dayOfWeek;

    private void Start()
    {
        dayOfWeek = DateTime.Now.DayOfWeek == 0 ? 6 : (int)DateTime.Now.DayOfWeek - 1;

        currentInfo = new DailyGoalInfo();
        dailyGoalList = new List<DailyGoal>();
        editChoices = new List<EditChoice>();
        assignmentsRemovedThisSession = new List<int>();
        progressBarMaskWidth = progressBarMask.rect.width;

        LoadFromJson();

        UpdateDay(currentInfo.lastLoadedDay);
        //NewWeek();
        
        LoadGoals();
        LoadEditor();
    }

    private void Awake()
    {
        DailyGoal.OnClickCompleted += HandleCompletedOne;
        DailyGoal.OnClickUndo += HandleUncompletedOne;
        EditChoice.OnChooseEdit += HandleChooseToEdit;
        DailyGoal.OnToggleGoal += HandleToggleDayProgress;
    }

    private void OnDestroy()
    {
        DailyGoal.OnClickCompleted -= HandleCompletedOne;
        DailyGoal.OnClickUndo -= HandleUncompletedOne;
        EditChoice.OnChooseEdit -= HandleChooseToEdit;
        DailyGoal.OnToggleGoal -= HandleToggleDayProgress;
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

            ToggleTodayWeekValue(newIndex, 1, dayOfWeek);
            dailyGoalList[newIndex].UpdateToggle();
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

            ToggleTodayWeekValue(newIndex, 0, dayOfWeek);
            dailyGoalList[newIndex].UpdateToggle();
        }
        currentInfo.thisDaysProgress[newIndex] = completions;
        UpdateJson();
        UpdateProgressBar();
    }

    private void HandleToggleDayProgress(int index, int day, bool value)
    {
        int newIndex = index;

        foreach (int i in assignmentsRemovedThisSession)
        {
            if (index > i)
            {
                newIndex--;
            }
        }

        Debug.Log("toggleDayProgress");
        ToggleTodayWeekValue(newIndex, value ? 1 : 0, day);

        currentInfo.thisWeeksProgress[newIndex] += value ? 1 : -1;
        if (day == dayOfWeek)
        {
            dailyGoalList[newIndex].Toggle(value, currentInfo.numberPerWeek[newIndex] - currentInfo.thisWeeksProgress[newIndex]);
            currentInfo.thisDaysProgress[newIndex] = value ? currentInfo.numberPerDay[newIndex] : 0;
        }

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
            //dailyGoalList[i].NewDay(daysLeftInTheWeek, currentInfo.numberPerWeek[i] - currentInfo.thisWeeksProgress[i]);
        }

        // update priority for new day

        currentInfo.NewDay(); // resets daily goals
        UpdateJson();
    }

    private void NewWeek()
    {
        Debug.Log("new week");

        for (int i = 0; i < currentInfo.numberOfGoals; i++)
        {
            currentInfo.thisWeeksProgress[i] = 0;
            currentInfo.thisWeeksValue[i] = 0;

            // update priority for new day
            //dailyGoalList[i].NewDay(daysLeftInTheWeek, currentInfo.numberPerWeek[i] - currentInfo.thisWeeksProgress[i]);
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
        dailyGoalList.Add(dailyGoal);
        dailyGoal.Initialize(goalNameInput.text, Convert.ToInt32(numberPerDayInput.text), 0, daysLeftInTheWeek, (int)numberPerWeek.value, dailyGoalList.Count, 0);

        GameObject newChoice = Instantiate(choicePrefab, editorContentBox);
        EditChoice editChoice = newChoice.GetComponent<EditChoice>();
        editChoice.Initialize(goalNameInput.text, currentInfo.numberOfGoals - 1);
        editChoices.Add(editChoice);

        taskMaker.SetActive(false);

        UpdateJson();
        UpdateProgressBar();
    }

    private void UpdateJson()
    {
        currentInfo.lastLoadedDay = DateTime.Now.ToString();

        string json = JsonUtility.ToJson(currentInfo, true);
        //File.WriteAllText(Application.dataPath + "/DailyInfoFile.json", json);
        PlayerPrefs.SetString(key, json);
    }

    public void LoadFromJson()
    {
        currentInfo = new DailyGoalInfo();
        string json = "";

        if (PlayerPrefs.HasKey(key))
        {
            json = PlayerPrefs.GetString(key);
        }
        else
        {
            json = JsonUtility.ToJson(currentInfo, true);
            PlayerPrefs.SetString(key, json);
        }

        Debug.Log(json);
        daysLeftInTheWeek = 7 - (DateTime.Now.DayOfWeek == 0 ? 6 : (int)DateTime.Now.DayOfWeek - 1);

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

        dailyGoalList[newIndex].Initialize(goalNameEdit.text, currentInfo.numberPerDay[newIndex], 0, daysLeftInTheWeek, currentInfo.numberPerWeek[newIndex] - currentInfo.thisWeeksProgress[newIndex], newIndex, currentInfo.thisWeeksValue[newIndex]);
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

    public void SwapGoalView()
    {
        toggleAnim.SetTrigger("Toggle");

        if (viewToggle.isOn)
        {
            todayText.text = DateTime.Now.ToString("MMMM dd, yyyy");
        }
        else
        {
            todayText.text = "Week of " + DateTime.Now.AddDays(-dayOfWeek).ToString("MM/dd");
        }

        for (int i = 0; i < currentInfo.numberOfGoals; i++)
        {
            dailyGoalList[i].SwapView(viewToggle.isOn);
        }
    }

    private void LoadGoals()
    {
        for (int i = 0; i < currentInfo.numberOfGoals; i++)
        {
            GameObject newGoal = Instantiate(dailyGoalPrefab, goalContainer);
            DailyGoal dailyGoal = newGoal.GetComponent<DailyGoal>();
            dailyGoalList.Add(dailyGoal);
            dailyGoal.Initialize(currentInfo.goalNames[i], currentInfo.numberPerDay[i], currentInfo.thisDaysProgress[i], daysLeftInTheWeek, currentInfo.numberPerWeek[i] - currentInfo.thisWeeksProgress[i], i, currentInfo.thisWeeksValue[i]);
            
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

    private void ToggleTodayWeekValue(int index, int can, int day)
    {
        int newValue = 0;
        int original = currentInfo.thisWeeksValue[index];
        for (int i = 6; i >= 0; i--)
        {
            if (day != i)
            {
                newValue += (1 << i) & original;
            }
            else
            {
              
                newValue += (can << day);// * (can == 0 ? -1 : 1);
            }
        }

        Debug.Log("toggle : " + newValue);
        currentInfo.thisWeeksValue[index] = newValue;
        dailyGoalList[index].NewWeekValue(newValue);
    }
}
