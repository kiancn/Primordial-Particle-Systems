using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TractorbeamActivator : MonoBehaviour
{
    [SerializeField] private GameObject tractorBeam;
    [SerializeField] private float timeMaxActivate = 4f;
    [SerializeField] private float timeBetweenActivates = 4;
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

        if (!mouseDown)
        {
            if (tractorBeam.activeSelf)
            {
                tractorBeam.SetActive(false);
            }else if (currentTimeToNextActivate >= 0f)
            {
                currentTimeToNextActivate -= Time.deltaTime;
            }
        }
        else if (mouseDown && !tractorBeam.activeSelf && currentTimeToNextActivate <= 0)
        {
            tractorBeam.SetActive(true);
            activationTime = 0;
            return;
        }


        if (mouseDown && activationTime > timeMaxActivate && tractorBeam.activeInHierarchy)
        {
            currentTimeToNextActivate = timeBetweenActivates;

            tractorBeam.SetActive(false);
            currentTimeToNextActivate = timeMaxActivate;
            return;
        }

        if (mouseDown && activationTime < timeMaxActivate)
        {
            activationTime += Time.deltaTime;
        }
    }
}