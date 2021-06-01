using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExternalLink : MonoBehaviour
{
    [SerializeField] private string url;

    public void OpenURL()
    {
        if (url != null)
        {
            Application.OpenURL(url);
        }
    }
}
