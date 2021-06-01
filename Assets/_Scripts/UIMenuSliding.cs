using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMenuSliding : MonoBehaviour
{
    [SerializeField] private GameObject uiElementTransform;
    [SerializeField] private Vector3 beforeSlidePosition;
    [SerializeField] private Vector3 afterSlidePosition;

    [SerializeField] private bool nowShowingMenu = false;
    [SerializeField] private bool readyToSlide = true;

    [SerializeField] private GameObject thisGO;

    private void OnEnable()
    {
        if (thisGO == null)
        {
            thisGO = this.gameObject;
        }

        if (uiElementTransform == null)
        {
            Debug.Log("No UI transform assigned. Shutting code down.");
            this.gameObject.SetActive(false);
            return;
        }

        beforeSlidePosition = uiElementTransform.transform.position;
        readyToSlide = true;
    }

    public void SlideMenuNow()
    {
        if (readyToSlide)
        {
            nowShowingMenu = !nowShowingMenu;
            SlideMenuAnimation(nowShowingMenu);
        }
    }

    public void SlideMenuAnimation(bool forthWard)
    {
        if (readyToSlide)
        {
            iTween.MoveTo(uiElementTransform,
                iTween.Hash(
                    "position", (forthWard ? afterSlidePosition : beforeSlidePosition),
                    "delay", 0.3f,
                    "time", 2.1f,
                    "easeType", "easeInOutElastic",
                    "onComplete", "ReadyToSlideNow",
                    "onCompleteTarget", thisGO
                )
            );
            readyToSlide = false;
            StartCoroutine(SafetyReady());
        }
    }
    // This is a hack because iTween is acting up on scene reloads
    private IEnumerator SafetyReady()
    {
        float timeSinceAnimationStart = 0f;

        while (timeSinceAnimationStart < 2.7f)
        {
            if (timeSinceAnimationStart > 2.56f && !readyToSlide)
            {
                readyToSlide = true;
            }

            timeSinceAnimationStart += Time.deltaTime;

            yield return null;
        }

    }


    void ReadyToSlideNow()
    {
        readyToSlide = true;
    }
}