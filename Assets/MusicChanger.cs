using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicChanger : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<AudioClip> clips;

    public void ChangeMusicClip(int clipsIndex)
    {
        if (clips == null || clipsIndex < 0 || clipsIndex > clips.Count) { return; }

        audioSource.clip = clips[clipsIndex];
        audioSource.Play();
            
    }
}