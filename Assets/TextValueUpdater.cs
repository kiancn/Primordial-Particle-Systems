using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextValueUpdater : MonoBehaviour
{
    [SerializeField] private Text valueText;

    public void OnUpdateValue(int newValue)
    {
        valueText.text = newValue.ToString();
    }
    
    public void OnUpdatedValueSingle(Single newValue)
    {
        valueText.text = newValue.ToString();
    }
}
