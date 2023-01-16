using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiAim : MonoBehaviour
{
    /*    void Update()
        {
            transform.position = Input.mousePosition;
        }*/
    public Texture2D cursorTexture;
    public Vector2 hotSpot = Vector2.zero;
    private void Awake()
    {
        Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.Auto);
    }
}
