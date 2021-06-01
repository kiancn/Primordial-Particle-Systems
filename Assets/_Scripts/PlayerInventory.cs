using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInventory : MonoBehaviour
{
  
  
  public int state0Cells;
  public int state1Cells;
  public int state2Cells;
  public int state3Cells;
  public int state4Cells;
  public int state5Cells;

  // Events intended for GUI
 [SerializeField] private UnityEvent<int> updateState0Cells;
 [SerializeField] private UnityEvent<int> updateState1Cells;
 [SerializeField] private UnityEvent<int> updateState2Cells;
 [SerializeField] private UnityEvent<int> updateState3Cells;
 [SerializeField] private UnityEvent<int> updateState4Cells;
 [SerializeField] private UnityEvent<int> updateState5Cells;
  
  public void AddResource(int type)
  {
    switch (type)
    {
      case 0: state0Cells++; updateState0Cells.Invoke(state0Cells); break;
      case 1: state1Cells++; updateState1Cells.Invoke(state1Cells);break;
      case 2: state2Cells++; updateState2Cells.Invoke(state2Cells);break;
      case 3: state3Cells++; updateState3Cells.Invoke(state3Cells);break;
      case 4: state4Cells++; updateState4Cells.Invoke(state4Cells);break;
      case 5: state5Cells++; updateState5Cells.Invoke(state5Cells);break;
    }
  }
  
  
}
