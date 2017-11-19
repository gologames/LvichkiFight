using UnityEngine;
using System.Collections;

public class RealBattleInfoScript
{
    public const float FieldLeft = 0f;
    public const float FieldTop = 0f;
    public const float FieldWidth = 1920f;
    public const float FieldHeight = 1080f;

    public const float CellGrid_Side = 93.75f;
    public const int CellGrid_CountW = 21;
    public const int CellGrid_CountH = 12;

    #region START_POINTS
    public const float InfOne_X = 800f;
    public const float InfOne_Y = 540f;
    public const float InfTwo_X = 1120f;
    public const float InfTwo_Y = 540f;

    public const float CavLeftOne_X = 800f;
    public const float CavLeftOne_Y = 650f;
    public const float CavLeftTwo_X = 1120f;
    public const float CavLeftTwo_Y = 650f;

    public const float CavRightOne_X = 800f;
    public const float CavRightOne_Y = 430f;
    public const float CavRightTwo_X = 1120f;
    public const float CavRightTwo_Y = 430f;

    public const float ArtOne_X = 600f;
    public const float ArtOne_Y = 540f;
    public const float ArtTwo_X = 1320f;
    public const float ArtTwo_Y = 540f;
    #endregion

    //public const float Field_V_FreePer = 0.03837209302325581f;
    //public const float Field_V_FreeArtPer = 0.09651162790697673f;
    //public const float Field_V_FreeArtFreePer = 0.029069767441860465f;
    //public const float Field_V_FreeArtFreeInfPer = 0.17151162790697674f;
    //public const float Field_V_OnePartPer = 0.3f;
    //public const float Field_V_FightPartPer = 0.4f;

    //public const float Field_H_FreePer = 0.1f;
    //public const float Field_H_FreeCavPer = 0.1f;
    //public const float Field_H_FreeCavFreePer = 0.1f;
    //public const float Field_H_FreeCavFreeInfPer = 0.4f;

    public const float Inf_UnitSide = 8f;
    public const float Cav_UnitSide = 10f;
    public const float Art_UnitSide = 11f;
    public const float UnitMarchSpace = 2f;
    public const float Ball_UnitSide = 4f;

    //public const float Inf_Ratio_WidthToHeight = 25.0f / 12.0f;
    public const float Inf_Ratio_WidthToHeight = 12.0f / 25.0f;
    //public const float Cav_Ratio_WidthToHeight = 1.0f / 3.0f;
    public const float Cav_Ratio_WidthToHeight = 6.0f / 18.0f;
    //public const float Art_Ratio_WidthToHeight = 4.0f / 1.0f;
    public const float Art_Ratio_WidthToHeight = 25.0f / 4.0f;

    public const int Inf_MaxCount = 300;
    public const int Inf_MaxColumns = 25;
    public const int Inf_MaxRows = 12;
    public const int Cav_MaxCount = 216;
    public const int Cav_MaxColumns = 6;
    public const int Cav_MaxRows = 18;
    public const int Art_MaxCount = 100;
    public const int Art_MaxColumns = 25;
    public const int Art_MaxRows = 4;

    //660 - max
    public const float InfSpeed_PerSec = 15.0f;
    public const float CavSpeed_PerSec = 25.0f;
    public const float ArtSpeed_PerSec = 10.0f;
    public const float BallSpeed_PerSec = 100.0f;

    public const float CavMaxRotateAngle = 15.0f;
    public const float ArtShotDist = 180.0f;
    public const float ArtBallDamagePerUnit = 0.1f;

    public static float GetSpeed(RealBattleScript.RealBattleTroops type)
    {
        switch (type)
        {
            case RealBattleScript.RealBattleTroops.Inf:
                return InfSpeed_PerSec;
            case RealBattleScript.RealBattleTroops.CavLeft:
            case RealBattleScript.RealBattleTroops.CavRight:
                return CavSpeed_PerSec;
            case RealBattleScript.RealBattleTroops.Art:
                return ArtSpeed_PerSec;
            default:
                Debug.Log("error in GetSpeed in RealBattleInfoScript");
                return 0;
        }
    }
    public static float GetUnitSide(RealBattleScript.RealBattleTroops type)
    {
        switch (type)
        {
            case RealBattleScript.RealBattleTroops.Inf:
                return Inf_UnitSide;
            case RealBattleScript.RealBattleTroops.CavLeft:
            case RealBattleScript.RealBattleTroops.CavRight:
                return Cav_UnitSide;
            case RealBattleScript.RealBattleTroops.Art:
                return Art_UnitSide;
            default:
                Debug.Log("error in GetUnitSide in RealBattleInfoScript");
                return 0;
        }
    }
}