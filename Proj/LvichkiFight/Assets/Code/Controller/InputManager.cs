using UnityEngine;
using System.Collections;

public interface IInputManager
{
    bool IsMoveRight();
    bool IsMoveLeft();
    bool IsMoveUp();
    bool IsMoveDown();
    float GetScroll();
}

public class InputManager : IInputManager
{
    private const int edgeScreenBound = 20;
    public bool IsMoveRight()
    {
        return Input.mousePosition.x >
            Screen.width - edgeScreenBound;
    }
    public bool IsMoveLeft()
    { return Input.mousePosition.x < edgeScreenBound; }
    public bool IsMoveUp()
    {
        return Input.mousePosition.y >
            Screen.height - edgeScreenBound;
    }
    public bool IsMoveDown()
    { return Input.mousePosition.y < edgeScreenBound; }
    public float GetScroll()
    { return Input.GetAxis("Mouse ScrollWheel"); }
}
