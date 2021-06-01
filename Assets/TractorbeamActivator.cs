using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TractorbeamActivator : MonoBehaviour
{
    [SerializeField] private GameObject tractorBeam;
    [SerializeField] private float timeMaxActivate = 4f;
    [SerializeField] private float activationTime = 0f;
    [SerializeField] private float currentTimeToNextActivate = 0f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        bool mouseDown = Input.GetMouseButton(1);

        if (!mouseDown && tractorBeam.activeSelf)
        {
            tractorBeam.SetActive(false);
        }else if (mouseDown && !tractorBeam.activeSelf)
        {
            tractorBeam.SetActive(true);
        }
        //
        // if (!mouseDown && currentTimeToNextActivate >= 0f)
        // {
        //     if(tractorBeam.activeInHierarchy){tractorBeam.SetActive(false);}
        //     currentTimeToNextActivate -= Time.deltaTime / timeMaxActivate;
        // }
        //
        // if (mouseDown && currentTimeToNextActivate >= 0f)
        // {
        //     tractorBeam.SetActive(true);
        //     activationTime = 0f;
        // }
        //
        // if (mouseDown)
        // {
        //     activationTime += Time.deltaTime;
        //    currentTimeToNextActivate = Time.deltaTime;
        //    if (activationTime > timeMaxActivate)
        //     {
        //         tractorBeam.SetActive(false);
        //     }
        // }
    }
}