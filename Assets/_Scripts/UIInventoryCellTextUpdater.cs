using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventoryCellTextUpdater : MonoBehaviour
{
    [SerializeField] private Text inventoryCell1Text;
    [SerializeField] private Text inventoryCell2Text;
    [SerializeField] private Text inventoryCell3Text;
    [SerializeField] private Text inventoryCell4Text;
    [SerializeField] private Text inventoryCell5Text;
    [SerializeField] private Text inventoryCell6Text;

    public void UpdateCell1Text(int newValue)
    {
        inventoryCell1Text.text = newValue.ToString();
        
    }

    public void UpdateCell2Text(int newValue)
    {
        inventoryCell2Text.text = newValue.ToString();
    }

    public void UpdateCell3Text(int newValue)
    {
        inventoryCell3Text.text = newValue.ToString();
    }

    public void UpdateCell4Text(int newValue)
    {
        inventoryCell4Text.text = newValue.ToString();
    }

    public void UpdateCell5Text(int newValue)
    {
        inventoryCell5Text.text = newValue.ToString();
    }

    public void UpdateCell6Text(int newValue)
    {
        inventoryCell6Text.text = newValue.ToString();
    }
}