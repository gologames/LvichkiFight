using UnityEngine;
using System.Collections;

public class TestStartFight : MonoBehaviour
{
    public void OnGUI()
    {
        if (GUI.Button(new Rect(0,0,100,50), "Run"))
        {
            StartFight.LoadFightScene(1, 2, 0, 1,
                42, 20, 4, 30, 25, 8);
        }
    }
}
