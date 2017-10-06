using UnityEngine;
using System.Collections;
using System;

public static class BattleDeltaTimeScript
{
    public const float SpeedTime = 1.0f / 60.0f;
    public const float InfSpeedTime = 0.1f;
    public const float CavSpeedTime = 0.1f;
    public const float ArtSpeedTime = 3.0f;

    private static float deltaTime = 0.0f;

    public static void Update()
    {
        deltaTime += Time.deltaTime;
    }

    public static bool IsNextFrame()
    {
        if (SpeedTime < deltaTime)
        {
            deltaTime %= SpeedTime;
            return true;
        }

        return false;
    }

    public static bool IsNextFrame(RealBattleScript.RealBattleTroops type, ref float infDelta)
    {
        float speedTime = 0;
        switch (type)
        {
            case RealBattleScript.RealBattleTroops.Inf:
                speedTime = InfSpeedTime;
                break;
            case RealBattleScript.RealBattleTroops.CavLeft:
            case RealBattleScript.RealBattleTroops.CavRight:
                speedTime = CavSpeedTime;
                break;
            case RealBattleScript.RealBattleTroops.Art:
                speedTime = ArtSpeedTime;
                break;
            default:
                Debug.Log("error in IsNextFrame in BattleDeltaTimeScript");
                break;
        }

        if (speedTime < infDelta)
        {
            infDelta %= speedTime;
            return true;
        }

        return false;
    }
}