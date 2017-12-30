using UnityEngine;
using System.Collections;

public class TestStartFight : MonoBehaviour
{
    public void OnGUI()
    {
        if (GUI.Button(new Rect(0,0,100,50), "Run"))
        {
            StartFight.LoadFightScene(0, 2, 0, 1,
                40, 20, 5, 21, 10, 36);
        }
    }
}
