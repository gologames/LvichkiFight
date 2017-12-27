using UnityEngine;
using System.Collections;

public class TestStartFight : MonoBehaviour
{
    public void OnGUI()
    {
        if (GUI.Button(new Rect(0,0,100,50), "Run"))
        {
            StartFight.LoadFightScene(0, 0, 1,
                110, 96, 20, 100, 46, 100);
        }
    }
}
