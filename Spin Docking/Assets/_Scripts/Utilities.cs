using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities
{
    public static void ToggleCursor()
    {
        if (Cursor.visible)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
    public static void LockCursor()
    {
        if (Input.anyKeyDown)
        {
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            if (Cursor.visible)
            {
                Cursor.visible = false;
            }
        }
    }
}
