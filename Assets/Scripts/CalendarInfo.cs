using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalendarInfo : MonoBehaviour
{
    public string[] eventName;
    public float[] eventStartTime;
    public bool[] isAllDay;
    public float[] eventLength;
    public string[] eventDay;
    public string[] eventNotes;

    public int numberOfEvents;
    private int maxNumberOfEvents = 1000;

    public CalendarInfo()
    {
        eventName = new string[maxNumberOfEvents];
        eventStartTime = new float[maxNumberOfEvents];
        eventLength = new float[maxNumberOfEvents];
        isAllDay = new bool[maxNumberOfEvents];
        eventDay = new string[maxNumberOfEvents];
        eventNotes = new string[maxNumberOfEvents];
        numberOfEvents = 0;
    }

    public void AddEvent(string name, float start, bool allDay, float length, string day, int index)
    {
        for (int i = numberOfEvents - 1; i >= index; i--)
        {
            MoveEvent(i, i + 1);
        }

        eventName[index] = name;
        eventStartTime[index] = start;
        eventLength[index] = length;
        isAllDay[index] = allDay;
        eventDay[index] = day;
        eventNotes[index] = "";
        numberOfEvents++;
    }

    public void RemoveEvent(int index)
    {
        for (int i = index + 1; i < numberOfEvents; i++)
        {
            MoveEvent(i, i - 1);
        }

        numberOfEvents--;
        eventName[numberOfEvents] = "";
        eventStartTime[numberOfEvents] = 0;
        eventLength[numberOfEvents] = 0;
        isAllDay[numberOfEvents] = false;
        eventDay[numberOfEvents] = "";
        eventNotes[numberOfEvents] = "";
    }

    public void MoveEvent(int indexFrom, int indexTo)
    {
        eventName[indexTo] = eventName[indexFrom];
        eventStartTime[indexTo] = eventStartTime[indexFrom];
        eventLength[indexTo] = eventLength[indexFrom];
        isAllDay[indexTo] = isAllDay[indexFrom];
        eventDay[indexTo] = eventDay[indexFrom];
        eventNotes[indexTo] = eventNotes[indexFrom];
    }
    
}
