using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenePause : MonoBehaviour
{
    [SerializeField] private KeyCode pauseButton = KeyCode.P;

    public bool pausedNow = false;

    public bool externallyLocked = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(pauseButton))
        {
            PauseSwitch();
        }
    }

    public void PauseSwitch()
    {
        if (externallyLocked) return;

        pausedNow = !pausedNow;

        if (pausedNow)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }
}