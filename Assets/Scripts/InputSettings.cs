using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSettings
{
    public static bool Left()
    {
        return Input.GetKeyDown(KeyCode.A);
    }

    public static bool Right()
    {
        return Input.GetKeyDown(KeyCode.D);
    }

    public static bool Jump()
    {
        return (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W));
    }

    public static bool JumpRelease()
    {
        return Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.W);
    }

    public static bool Throw()
    {
        return Input.GetKeyDown(KeyCode.E);
    }

    public static bool Menu()
    {
        return Input.GetKeyDown(KeyCode.Escape);
    }
}
