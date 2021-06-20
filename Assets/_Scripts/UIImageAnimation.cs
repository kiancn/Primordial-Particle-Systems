using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Image))]
public class UIImageAnimation : MonoBehaviour
{
    public Sprite[] sprites;
    public int framesPerSprite = 6;
    public bool loop = true;
    public bool destroyOnEnd = false;
    public bool disableOnEnd = false;

    private int index = 0;
    private Image image;
    private int frame = 0;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    private void OnEnable()
    {
        index = 0;
        image.sprite = sprites[0];
    }

    void Update()
    {
        if (!loop && index == sprites.Length) return;
        frame++;
        if (frame < framesPerSprite) return;
        image.sprite = sprites[index];
        frame = 0;
        index++;
        if (index >= sprites.Length)
        {
            if (loop) index = 0;
            if (destroyOnEnd)
            {
                Destroy(gameObject);
            }

            if (disableOnEnd)
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void PlayAnimation()
    {
        gameObject.SetActive(true);
    }
}