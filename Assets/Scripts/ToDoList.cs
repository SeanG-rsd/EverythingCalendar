using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToDoList : MonoBehaviour
{
    [SerializeField] private GameObject todoTaskPrefab;
    [SerializeField] private Transform taskContainer;

    private List<GameObject> completedList;
    private List<GameObject> unCompletedList;

    private string completedTasksKey = "COMPLETED_TO_DO_LIST";
    private string unCompletedTasksKey = "UNCOMPLETED_TO_DO_LIST";
    private string seperator = "\n";

    private bool showingUnCompleted;

    [Header("---Make Task---")]
    [SerializeField] private GameObject makeTaskScreen;
    [SerializeField] private TMP_InputField taskName;


    private void Awake()
    {
        

        completedList = new List<GameObject>();
        unCompletedList = new List<GameObject>();

        ToDoTask.OnToggleCompleted += CompletedTask;
        ToDoTask.OnDeleteTask += HandleDeleteTask;

        //PlayerPrefs.SetString(completedTasksKey, "");
        //PlayerPrefs.SetString(unCompletedTasksKey, "");

        LoadSavedTasks();

        SwapView(true);
    }

    private void OnDestroy()
    {
        ToDoTask.OnToggleCompleted -= CompletedTask;
        ToDoTask.OnDeleteTask -= HandleDeleteTask;
    }


    // seperated by (@)
    private void LoadSavedTasks()
    {
        try
        {
            string completed = PlayerPrefs.GetString(completedTasksKey);
            //Debug.Log(completed);
            string[] tasks = completed.Split(seperator);

            
            foreach (string task in tasks)
            {
                if (task != string.Empty)
                {
                    GameObject newTask = Instantiate(todoTaskPrefab, taskContainer);
                    newTask.GetComponent<ToDoTask>().Initialize(true, task);
                    completedList.Add(newTask);
                }
            }
        }
        catch
        {
            PlayerPrefs.SetString(completedTasksKey, "");
        }

        try
        {
            string completed = PlayerPrefs.GetString(unCompletedTasksKey);
            //Debug.Log(completed);
            string[] tasks = completed.Split(seperator);

            foreach (string task in tasks)
            {
                if (task != string.Empty)
                {
                    GameObject newTask = Instantiate(todoTaskPrefab, taskContainer);
                    newTask.GetComponent<ToDoTask>().Initialize(false, task);
                    unCompletedList.Add(newTask);
                }
            }
        }
        catch
        {
            PlayerPrefs.SetString(unCompletedTasksKey, "");
        }
    }

    private void SaveTasks()
    {
        string uncompleted = "";

        foreach (GameObject gameObject in unCompletedList)
        {
            uncompleted += gameObject.GetComponent<ToDoTask>().GetTask() + seperator;   
        }

        string completed = "";

        foreach (GameObject gameObject in completedList)
        {
            completed += gameObject.GetComponent<ToDoTask>().GetTask() + seperator;
        }

        PlayerPrefs.SetString(completedTasksKey, completed);
        PlayerPrefs.SetString(unCompletedTasksKey, uncompleted);

        //Debug.Log(completed + ", " + uncompleted);
    }

    public void SwapView(bool showingUncompleted)
    {
        showingUnCompleted = showingUncompleted;
        foreach (GameObject task in unCompletedList)
        {
            task.SetActive(showingUncompleted);
        }

        foreach(GameObject task in completedList)
        {
            task.SetActive(!showingUncompleted);
        }
    }

    public void MakeNewTask()
    {
        makeTaskScreen.SetActive(true);
    }

    public void FinishedMakingNewTask()
    {
        makeTaskScreen.SetActive(false);
        string task = taskName.text;

        if (task != string.Empty)
        {
            GameObject newTask = Instantiate(todoTaskPrefab, taskContainer);
            newTask.GetComponent<ToDoTask>().Initialize(false, task);
            unCompletedList.Add(newTask);
            newTask.SetActive(showingUnCompleted);
            SaveTasks();
        }

        taskName.text = "";
    }

    public void StopMakingNewTask()
    {
        makeTaskScreen.SetActive(false);
        taskName.text = "";
    }

    private void CompletedTask(GameObject task, bool isCurrentlyCompleted)
    {
        if (isCurrentlyCompleted)
        {
            completedList.Add(task);
            unCompletedList.Remove(task);
        }
        else
        {
            unCompletedList.Add(task);
            completedList.Remove(task);
        }

        SwapView(isCurrentlyCompleted);
    }

    private void HandleDeleteTask(GameObject task, bool isCompleted)
    {
        if (isCompleted)
        {
            completedList.Remove(task);
        }
        else
        {
            unCompletedList.Remove(task);
        }

        Destroy(task);

        SaveTasks();
    }
}
