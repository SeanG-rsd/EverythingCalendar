using System;
using System.Diagnostics;

[Serializable]

public class HomeworkInfo
{
    public int numberOfAssignments;
    public string lastLoadedDay;
    public string[] homeworkNames;
    public string[] dueDates;
    public string[] subject;
    public float[] predictedHours;
    public string[] notes;
    public string[] subjects;
    public int numberOfSubjects;

    private int maxNumberOfAssignments = 100;

    public HomeworkInfo()
    {
        numberOfAssignments = 0;
        homeworkNames = new string[maxNumberOfAssignments];
        dueDates = new string[maxNumberOfAssignments];
        subject = new string[maxNumberOfAssignments];
        predictedHours = new float[maxNumberOfAssignments];
        subjects = new string[maxNumberOfAssignments];
        notes = new string[maxNumberOfAssignments];
    }

    public void AddNewAssignment(string name, string dueDate, string subject, float predictedHours)
    {
        homeworkNames[numberOfAssignments] = name;
        dueDates[numberOfAssignments] = dueDate;
        this.subject[numberOfAssignments] = subject;
        this.predictedHours[numberOfAssignments] = predictedHours;
        notes[numberOfAssignments] = "";
        numberOfAssignments++;
    }

    public void RemoveAssignment(int index)
    {
        for (int i = index + 1; i < numberOfAssignments; i++)
        {
            homeworkNames[i - 1] = homeworkNames[i];
            dueDates[i - 1] = dueDates[i];
            subject[i - 1] = subjects[i];
            predictedHours[i - 1] = predictedHours[i];
            notes[i - 1] = notes[i];
        }

        numberOfAssignments--;
        homeworkNames[numberOfAssignments] = "";
        notes[numberOfAssignments] = "";
        dueDates[numberOfAssignments] = "";
        subject[numberOfAssignments] = "";
        predictedHours[numberOfAssignments] = 0;
    }
}
