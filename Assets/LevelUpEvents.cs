using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelUpEvents : MonoBehaviour
{
    // Level up events, representing the activity that happens that a level complete.

    [SerializeField] private List<UnityEvent> onLevelEndEvents;

    [SerializeField] private PlayerStats playerStats;

    public void OnEndLevelEvent()
    {
        if (playerStats.PlayerLevel <= onLevelEndEvents.Count) { onLevelEndEvents[playerStats.PlayerLevel].Invoke(); }
        else {Debug.Log("No more levels to reach!"); }
    }
}