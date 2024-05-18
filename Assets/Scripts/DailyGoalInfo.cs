using System;

[Serializable]

public class DailyGoalInfo
{
    public string[] goalNames;
    public int numberOfGoals;
    public int[] numberPerWeek;
    public int[] thisWeeksProgress;

    public int[] numberPerDay;
    public int[] thisDaysProgress;
    public string lastLoadedDay;

    private int listSize = 100;

    public DailyGoalInfo()
    {
        numberOfGoals = 0;
        goalNames = new string[listSize];
        numberPerWeek = new int[listSize];
        thisWeeksProgress = new int[listSize];
        thisDaysProgress = new int[listSize];
        numberPerDay = new int[listSize];
    }

    public void AddNewGoal(string goalName, int perWeek, int perDay)
    {
        goalNames[numberOfGoals] = goalName;
        numberPerWeek[numberOfGoals] = perWeek;
        thisWeeksProgress[numberOfGoals] = 0;
        numberPerDay[numberOfGoals] = perDay;
        thisDaysProgress[numberOfGoals] = 0;
        numberOfGoals++;
    }

    public void NewDay()
    {
        thisDaysProgress = new int[listSize];
    }

    public void RemoveGoal(int index)
    {
        for (int i = index + 1; i < numberOfGoals; i++)
        {
            goalNames[i - 1] = goalNames[i];
            numberPerWeek[i - 1] = numberPerWeek[i];
            numberPerDay[i - 1] = numberPerDay[i];
            thisDaysProgress[i - 1] = thisDaysProgress[i];
            thisWeeksProgress[i - 1] = thisWeeksProgress[i];
        }

        numberOfGoals--;
        goalNames[numberOfGoals] = "";
        numberPerWeek[numberOfGoals] = 0;
        numberPerDay[numberOfGoals] = 0;
        thisWeeksProgress[numberOfGoals] = 0;
        thisDaysProgress[numberOfGoals] = 0;
        
    }
}
