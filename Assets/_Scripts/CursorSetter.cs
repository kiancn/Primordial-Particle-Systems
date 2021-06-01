using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorSetter : MonoBehaviour
{
    public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot;
    void OnEnable()
    {
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }

    void OnDestroy()
    {
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }
    
}
