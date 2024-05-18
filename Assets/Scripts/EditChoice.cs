using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EditChoice : MonoBehaviour
{
    [SerializeField] private TMP_Text goalName;
    private int index;

    public static Action<int> OnChooseEdit = delegate { };

    public void Initialize(string goal, int i)
    {
        index = i;
        goalName.text = goal;
    }

    public void OnChooseThis()
    {
        OnChooseEdit?.Invoke(index);
    }
}
