using UnityEngine;
using System.Collections.Generic;
using Spine.Unity;
using Com.LuisPedroFonseca.ProCamera2D;

public class BattleGUIScript : MonoBehaviour
{
    [SerializeField]
    private GameObject infPrefab;
    [SerializeField]
    private GameObject cavPrefab;
    [SerializeField]
    private GameObject artPrefab;
    private RealBattleScript battle;
    private SkeletonAnimation[] linf;
    private SkeletonAnimation[] llcav;
    private SkeletonAnimation[] lrcav;
    private SkeletonAnimation[] lart;
    private SkeletonAnimation[] rinf;
    private SkeletonAnimation[] rlcav;
    private SkeletonAnimation[] rrcav;
    private SkeletonAnimation[] rart;
    private bool cameraTargetFlag = true;

    void InitPrefabs(ref SkeletonAnimation[] arr, GameObject prefab, int count)
    {
        arr = new SkeletonAnimation[count];
        for (int i = 0; i < count; i++)
        { arr[i] = Instantiate<GameObject>(prefab).GetComponent<SkeletonAnimation>(); }
    }


    #region CAMERA_TARGET
    void addCameraTargets()
    {
        SkeletonAnimation[][] units = new SkeletonAnimation[8][];
        units[0] = linf; units[1] = llcav; units[2] = lrcav; units[3] = lart;
        units[4] = rinf; units[5] = rlcav; units[6] = rrcav; units[7] = rart;

        float minX = float.MaxValue, maxX = float.MinValue;
        float minY = float.MaxValue, maxY = float.MinValue;
        SkeletonAnimation minXunit = null, maxXunit = null;
        SkeletonAnimation minYunit = null, maxYunit = null;

        for (int i = 0; i < units.Length; i++)
        {
            for (int j = 0; j < units[i].Length; j++)
            {
                if (units[i][j].gameObject == null) continue;
                if (units[i][j].transform.position.x < minX)
                {
                    minX = units[i][j].transform.position.x;
                    minXunit = units[i][j];
                }
                if (units[i][j].transform.position.x > maxX)
                {
                    maxX = units[i][j].transform.position.x;
                    maxXunit = units[i][j];
                }
                if (units[i][j].transform.position.y < minY)
                {
                    minY = units[i][j].transform.position.y;
                    minYunit = units[i][j];
                }
                if (units[i][j].transform.position.y > maxY)
                {
                    maxY = units[i][j].transform.position.y;
                    maxYunit = units[i][j];
                }
            }
        }

        ProCamera2D.Instance.AddCameraTarget(minXunit.transform);
        ProCamera2D.Instance.AddCameraTarget(maxXunit.transform);
        ProCamera2D.Instance.AddCameraTarget(minYunit.transform);
        ProCamera2D.Instance.AddCameraTarget(maxYunit.transform);
    }
    #endregion

    void Start()
    {
        ArmyUnitScript leftArmy = new ArmyUnitScript(100000, 0, 0);
        ArmyUnitScript rightArmy = new ArmyUnitScript(100000, 0, 0);
        battle = new RealBattleScript(leftArmy, rightArmy);

        InitPrefabs(ref linf, infPrefab, battle.InfOne.GetUnitsCount());
        InitPrefabs(ref llcav, cavPrefab, battle.CavLeftOne.GetUnitsCount());
        InitPrefabs(ref lrcav, cavPrefab, battle.CavRightOne.GetUnitsCount());
        InitPrefabs(ref lart, artPrefab, battle.ArtOne.GetUnitsCount());
        InitPrefabs(ref rinf, infPrefab, battle.InfTwo.GetUnitsCount());
        InitPrefabs(ref rlcav, cavPrefab, battle.CavLeftTwo.GetUnitsCount());
        InitPrefabs(ref rrcav, cavPrefab, battle.CavRightTwo.GetUnitsCount());
        InitPrefabs(ref rart, artPrefab, battle.ArtTwo.GetUnitsCount());
    }

    void Update()
    {
        BattleDeltaTimeScript.Update();
        if (BattleDeltaTimeScript.IsNextFrame())
        {
            battle.Update(BattleDeltaTimeScript.SpeedTime);
        }

        destroyDead();
        drawFight();
    }

    #region DESTROY_DEAD
    void _destroyDead(RealBattleScript.BattleGroup group, SkeletonAnimation[] arr)
    {
        for (int i = group.GetUnitsCount(); i < arr.Length; i++)
        { if (arr[i] != null) Destroy(arr[i]); else break; }
    }
    void destroyDead()
    {
        _destroyDead(battle.InfOne, linf);
        _destroyDead(battle.CavLeftOne, llcav);
        _destroyDead(battle.CavRightOne, lrcav);
        _destroyDead(battle.ArtOne, lart);
        _destroyDead(battle.InfTwo, rinf);
        _destroyDead(battle.CavLeftTwo, rlcav);
        _destroyDead(battle.CavRightTwo, rrcav);
        _destroyDead(battle.ArtTwo, rart);
    }
    #endregion

    void drawFight()
    {
        //if (oneAll == 0)
        //{
        //    GUIGUI.StateM.ClickPostRealBattle(false);
        //}
        //if (twoAll == 0)
        //{
        //    GUIGUI.StateM.ClickPostRealBattle(true);
        //}

        for (int i = 0; i < battle.InfOne.GetUnitsCount(); i++)
        {
            float side = RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.Inf);
            float left = battle.InfOne.GetUnit(i).X - side / 2.0f - RealBattleInfoScript.FieldLeft;
            float top = battle.InfOne.GetUnit(i).Y - side / 2.0f - RealBattleInfoScript.FieldTop;
            linf[i].transform.position = new Vector3(left, top, 0);
        }

        for (int i = 0; i < battle.InfTwo.GetUnitsCount(); i++)
        {
            float side = RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.Inf);
            float left = battle.InfTwo.GetUnit(i).X - side / 2.0f - RealBattleInfoScript.FieldLeft;
            float top = battle.InfTwo.GetUnit(i).Y - side / 2.0f - RealBattleInfoScript.FieldTop;
            rinf[i].transform.position = new Vector3(left, top, 0);
            rinf[i].skeleton.SetColor(Color.blue);
        }

        if (cameraTargetFlag)
        {
            addCameraTargets();
            cameraTargetFlag = false;
        }


        //#region FIELD
        //GUIBox unitBox = new GUIBox(0, 0, 0, 0, "");

        //#region INF
        //for (int i = 0; i < battle.InfOne.GetUnitsCount(); i++)
        //{
        //    unitBox.Left = battle.InfOne.GetUnit(i).X - RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.Inf) / 2.0f;
        //    unitBox.Top = battle.InfOne.GetUnit(i).Y - RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.Inf) / 2.0f;
        //    unitBox.Width = unitBox.Height = RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.Inf);
        //    GUIGUI.CreateBox(unitBox, Unit_Inf_One_Style);
        //}

        //for (int i = 0; i < battle.InfTwo.GetUnitsCount(); i++)
        //{
        //    unitBox.Left = battle.InfTwo.GetUnit(i).X - RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.Inf) / 2.0f;
        //    unitBox.Top = battle.InfTwo.GetUnit(i).Y - RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.Inf) / 2.0f;
        //    unitBox.Width = unitBox.Height = RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.Inf);
        //    GUIGUI.CreateBox(unitBox, Unit_Inf_Two_Style);
        //}
        //#endregion

        //#region CAV
        //for (int i = 0; i < battle.CavLeftOne.GetUnitsCount(); i++)
        //{
        //    unitBox.Left = battle.CavLeftOne.GetUnit(i).X - RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.CavLeft) / 2.0f;
        //    unitBox.Top = battle.CavLeftOne.GetUnit(i).Y - RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.CavLeft) / 2.0f;
        //    unitBox.Width = unitBox.Height = RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.Inf);
        //    float angle = battle.CavLeftOne.GetUnit(i).Angle;
        //    GUIGUI.CreateRotateBox(unitBox, angle, Unit_Cav_One_Style);
        //}
        //for (int i = 0; i < battle.CavRightOne.GetUnitsCount(); i++)
        //{
        //    unitBox.Left = battle.CavRightOne.GetUnit(i).X - RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.CavRight) / 2.0f;
        //    unitBox.Top = battle.CavRightOne.GetUnit(i).Y - RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.CavRight) / 2.0f;
        //    unitBox.Width = unitBox.Height = RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.Inf);
        //    float angle = battle.CavRightOne.GetUnit(i).Angle;
        //    GUIGUI.CreateRotateBox(unitBox, angle, Unit_Cav_One_Style);
        //}

        //for (int i = 0; i < battle.CavLeftTwo.GetUnitsCount(); i++)
        //{
        //    unitBox.Left = battle.CavLeftTwo.GetUnit(i).X - RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.CavLeft) / 2.0f;
        //    unitBox.Top = battle.CavLeftTwo.GetUnit(i).Y - RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.CavLeft) / 2.0f;
        //    unitBox.Width = unitBox.Height = RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.Inf);
        //    float angle = battle.CavLeftTwo.GetUnit(i).Angle;
        //    GUIGUI.CreateRotateBox(unitBox, angle, Unit_Cav_Two_Style);
        //}
        //for (int i = 0; i < battle.CavRightTwo.GetUnitsCount(); i++)
        //{
        //    unitBox.Left = battle.CavRightTwo.GetUnit(i).X - RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.CavRight) / 2.0f;
        //    unitBox.Top = battle.CavRightTwo.GetUnit(i).Y - RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.CavRight) / 2.0f;
        //    unitBox.Width = unitBox.Height = RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.Inf);
        //    float angle = battle.CavRightTwo.GetUnit(i).Angle;
        //    GUIGUI.CreateRotateBox(unitBox, angle, Unit_Cav_Two_Style);
        //}
        //#endregion

        //#region ART
        //for (int i = 0; i < battle.ArtOne.GetUnitsCount(); i++)
        //{
        //    unitBox.Left = battle.ArtOne.GetUnit(i).X - RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.Inf) / 2.0f;
        //    unitBox.Top = battle.ArtOne.GetUnit(i).Y - RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.Inf) / 2.0f;
        //    unitBox.Width = unitBox.Height = RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.Inf);
        //    GUIGUI.CreateBox(unitBox, Unit_Art_One_Style);
        //}

        //for (int i = 0; i < battle.ArtTwo.GetUnitsCount(); i++)
        //{
        //    unitBox.Left = battle.ArtTwo.GetUnit(i).X - RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.Inf) / 2.0f;
        //    unitBox.Top = battle.ArtTwo.GetUnit(i).Y - RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.Inf) / 2.0f;
        //    unitBox.Width = unitBox.Height = RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.Inf);
        //    GUIGUI.CreateBox(unitBox, Unit_Art_Two_Style);
        //}
        //#endregion

        //#region BALL
        //for (int i = 0; i < battle.BallOne.Units.Count; i++)
        //{
        //    unitBox.Left = battle.BallOne.Units[i].Pos.x - RealBattleInfoScript.Ball_UnitSide / 2.0f;
        //    unitBox.Top = battle.BallOne.Units[i].Pos.y - RealBattleInfoScript.Ball_UnitSide / 2.0f;
        //    unitBox.Width = unitBox.Height = RealBattleInfoScript.Ball_UnitSide;
        //    GUIGUI.CreateBox(unitBox, Unit_Art_One_Style);
        //}

        //for (int i = 0; i < battle.BallTwo.Units.Count; i++)
        //{
        //    unitBox.Left = battle.BallTwo.Units[i].Pos.x - RealBattleInfoScript.Ball_UnitSide / 2.0f;
        //    unitBox.Top = battle.BallTwo.Units[i].Pos.y - RealBattleInfoScript.Ball_UnitSide / 2.0f;
        //    unitBox.Width = unitBox.Height = RealBattleInfoScript.Ball_UnitSide;
        //    GUIGUI.CreateBox(unitBox, Unit_Art_Two_Style);
        //}
        //#endregion

        //#endregion
    }
    void drawPost()
    {
        #region VICTORY
        //ArmyUnitScript oneArmy = state.OneArmy;
        //ArmyUnitScript twoArmy = state.TwoArmy;
        ////int myID = Engine.World.GetMyCountryID();
        //int oneID = oneArmy.GetCountryID();
        //int twoID = twoArmy.GetCountryID();
        //bool isOneWin = state.GetIsOneWin();
        //bool isMyOne = myID == oneID;

        //string text = isMyOne == isOneWin ? Text.GetPopup(TextPopup.BattleInfo_Victory) : Text.GetPopup(TextPopup.BattleInfo_Defeat);
        //if (GUIGUI.CreateButton(GUIGUI.GUIel.Battle_Pre_StartBattle, text, Choice_But_Style))
        //{
        //    #region CHANGE_UNTS_COUNT
        //    oneArmy.SubInfantry(oneArmy.GetInfantry() - battle.InfOne.GetCountReal());
        //    oneArmy.SubCavalry(oneArmy.GetCavalry() - battle.CavLeftOne.GetCountReal() - battle.CavRightOne.GetCountReal());
        //    oneArmy.SubArtillery(oneArmy.GetArtillery() - battle.ArtOne.GetCountReal());

        //    twoArmy.SubInfantry(twoArmy.GetInfantry() - battle.InfTwo.GetCountReal());
        //    twoArmy.SubCavalry(twoArmy.GetCavalry() - battle.CavLeftTwo.GetCountReal() - battle.CavRightTwo.GetCountReal());
        //    twoArmy.SubArtillery(twoArmy.GetArtillery() - battle.ArtTwo.GetCountReal());
        //    #endregion
        //}
        #endregion
    }
}
