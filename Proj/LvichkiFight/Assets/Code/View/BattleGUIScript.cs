using UnityEngine;
using System.Collections.Generic;

public class BattleGUIScript : MonoBehaviour
{
    [SerializeField]
    private GameObject infPrefab;
    [SerializeField]
    private GameObject cavPrefab;
    [SerializeField]
    private GameObject artPrefab;
    private RealBattleScript battle;
    private GameObject[] linf;
    private GameObject[] lcav;
    private GameObject[] lart;
    private GameObject[] rinf;
    private GameObject[] rcav;
    private GameObject[] rart;

    void InitPrefabs(GameObject[] arr, GameObject prefab, int count)
    {
        arr = new GameObject[count];
        for (int i = 0; i < count; i++)
        { arr[i] = Instantiate<GameObject>(prefab); }
    }

    void Start()
    {
        ArmyUnitScript leftArmy = new ArmyUnitScript(1, 0, 0);
        ArmyUnitScript rightArmy = new ArmyUnitScript(1, 0, 0);
        battle = new RealBattleScript(leftArmy, rightArmy);

        InitPrefabs(linf, infPrefab, battle.InfOne.GetUnitsCount());
        InitPrefabs(lcav, cavPrefab, battle.CavLeftOne.GetUnitsCount() +
            battle.CavRightOne.GetUnitsCount());
        InitPrefabs(lart, artPrefab, battle.ArtOne.GetUnitsCount());
        InitPrefabs(rinf, infPrefab, battle.InfTwo.GetUnitsCount());
        InitPrefabs(rcav, cavPrefab, battle.CavLeftTwo.GetUnitsCount() +
            battle.CavRightTwo.GetUnitsCount());
        InitPrefabs(rart, artPrefab, battle.ArtTwo.GetUnitsCount());
    }

    void Update()
    {
        BattleDeltaTimeScript.Update();
        if (BattleDeltaTimeScript.IsNextFrame())
        {
            battle.Update(BattleDeltaTimeScript.SpeedTime);
        }

        drawFight();
    }

    #region FIGHT
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
    #endregion
}
