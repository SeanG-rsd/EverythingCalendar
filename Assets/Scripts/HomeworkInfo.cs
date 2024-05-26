using System;

[Serializable]

public class HomeworkInfo
{
    public int numberOfAssignments;
    public string lastLoadedDay;
    public string[] homeworkNames;
    public string[] dueDates;
    public string[] subject;
    public float[] predictedHours;
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
    }

    public void AddNewAssignment(string name, string dueDate, string subject, float predictedHours)
    {
        homeworkNames[numberOfAssignments] = name;
        dueDates[numberOfAssignments] = dueDate;
        this.subject[numberOfAssignments] = subject;
        this.predictedHours[numberOfAssignments] = predictedHours;
        numberOfAssignments++;
    }

    public void AddNewSubject(string name)
    {
        subjects[numberOfSubjects] = name;
        numberOfSubjects++;
    }

    public void RemoveSubject(string subject)
    {

    }
}
