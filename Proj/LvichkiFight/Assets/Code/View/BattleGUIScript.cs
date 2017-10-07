using UnityEngine;
using System.Collections;

public class BattleGUIScript : MonoBehaviour
{
    private BattleStateMachineScript state;
    private RealBattleScript battle;

    #region STYLES

    #region CHOICE
    public GUIStyle Choice_Battle_Style;
    public GUIStyle Choice_Background_Style;
    public GUIStyle Choice_But_Style;
    #endregion

    #region FIGHT
    public GUIStyle Fight_Back_Style;

    #region UNITS
    public GUIStyle Unit_Inf_One_Style;
    public GUIStyle Unit_Inf_Two_Style;
    public GUIStyle Unit_Cav_One_Style;
    public GUIStyle Unit_Cav_Two_Style;
    public GUIStyle Unit_Art_One_Style;
    public GUIStyle Unit_Art_Two_Style;
    #endregion

    #endregion

    public GUIStyle Text_Style;
    #endregion

    void Start()
    {
        state = new BattleStateMachineScript();
    }

    void Update()
    {
        if (state.Battle_State == BattleState.Fight)
        {
            BattleDeltaTimeScript.Update();
            if (BattleDeltaTimeScript.IsNextFrame())
            {
                battle.Update(BattleDeltaTimeScript.SpeedTime);
            }
        }

        switch (state.Battle_State)
        {
            case BattleState.Choice:
                drawChoice();
                break;
            case BattleState.Pre:
                drawFight();
                drawPre();
                break;
            case BattleState.Fight:
                drawFight();
                break;
            case BattleState.Post:
                drawFight();
                drawPost();
                break;
            default:
                Debug.Log("error in OnGUI in BattleGUIScript");
                break;
        }
    }

    #region CHOICE
    void drawChoice()
    {
        ArmyUnitScript oneArmy = state.OneArmy;
        ArmyUnitScript twoArmy = state.TwoArmy;
        int oneID = oneArmy.GetCountryID();
        int twoID = twoArmy.GetCountryID();

        battle = new RealBattleScript(oneArmy, twoArmy);
        state.ClickPreRealBattle();
    }
    #endregion

    #region FIGHT
    void drawPre()
    {
        state.ClickFightRealBattle();
    }
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

        #region FIELD
        GUIBox unitBox = new GUIBox(0, 0, 0, 0, "");

        #region INF
        for (int i = 0; i < battle.InfOne.GetUnitsCount(); i++)
        {
            unitBox.Left = battle.InfOne.GetUnit(i).X - RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.Inf) / 2.0f;
            unitBox.Top = battle.InfOne.GetUnit(i).Y - RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.Inf) / 2.0f;
            unitBox.Width = unitBox.Height = RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.Inf);
            GUIGUI.CreateBox(unitBox, Unit_Inf_One_Style);
        }

        for (int i = 0; i < Battle.InfTwo.GetUnitsCount(); i++)
        {
            unitBox.Left = Battle.InfTwo.GetUnit(i).X - RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.Inf) / 2.0f;
            unitBox.Top = Battle.InfTwo.GetUnit(i).Y - RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.Inf) / 2.0f;
            unitBox.Width = unitBox.Height = RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.Inf);
            GUIGUI.CreateBox(unitBox, Unit_Inf_Two_Style);
        }
        #endregion

        #region CAV
        for (int i = 0; i < Battle.CavLeftOne.GetUnitsCount(); i++)
        {
            unitBox.Left = Battle.CavLeftOne.GetUnit(i).X - RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.CavLeft) / 2.0f;
            unitBox.Top = Battle.CavLeftOne.GetUnit(i).Y - RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.CavLeft) / 2.0f;
            unitBox.Width = unitBox.Height = RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.Inf);
            float angle = Battle.CavLeftOne.GetUnit(i).Angle;
            GUIGUI.CreateRotateBox(unitBox, angle, Unit_Cav_One_Style);
        }
        for (int i = 0; i < Battle.CavRightOne.GetUnitsCount(); i++)
        {
            unitBox.Left = Battle.CavRightOne.GetUnit(i).X - RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.CavRight) / 2.0f;
            unitBox.Top = Battle.CavRightOne.GetUnit(i).Y - RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.CavRight) / 2.0f;
            unitBox.Width = unitBox.Height = RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.Inf);
            float angle = Battle.CavRightOne.GetUnit(i).Angle;
            GUIGUI.CreateRotateBox(unitBox, angle, Unit_Cav_One_Style);
        }

        for (int i = 0; i < Battle.CavLeftTwo.GetUnitsCount(); i++)
        {
            unitBox.Left = Battle.CavLeftTwo.GetUnit(i).X - RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.CavLeft) / 2.0f;
            unitBox.Top = Battle.CavLeftTwo.GetUnit(i).Y - RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.CavLeft) / 2.0f;
            unitBox.Width = unitBox.Height = RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.Inf);
            float angle = Battle.CavLeftTwo.GetUnit(i).Angle;
            GUIGUI.CreateRotateBox(unitBox, angle, Unit_Cav_Two_Style);
        }
        for (int i = 0; i < Battle.CavRightTwo.GetUnitsCount(); i++)
        {
            unitBox.Left = Battle.CavRightTwo.GetUnit(i).X - RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.CavRight) / 2.0f;
            unitBox.Top = Battle.CavRightTwo.GetUnit(i).Y - RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.CavRight) / 2.0f;
            unitBox.Width = unitBox.Height = RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.Inf);
            float angle = Battle.CavRightTwo.GetUnit(i).Angle;
            GUIGUI.CreateRotateBox(unitBox, angle, Unit_Cav_Two_Style);
        }
        #endregion

        #region ART
        for (int i = 0; i < Battle.ArtOne.GetUnitsCount(); i++)
        {
            unitBox.Left = Battle.ArtOne.GetUnit(i).X - RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.Inf) / 2.0f;
            unitBox.Top = Battle.ArtOne.GetUnit(i).Y - RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.Inf) / 2.0f;
            unitBox.Width = unitBox.Height = RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.Inf);
            GUIGUI.CreateBox(unitBox, Unit_Art_One_Style);
        }

        for (int i = 0; i < Battle.ArtTwo.GetUnitsCount(); i++)
        {
            unitBox.Left = Battle.ArtTwo.GetUnit(i).X - RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.Inf) / 2.0f;
            unitBox.Top = Battle.ArtTwo.GetUnit(i).Y - RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.Inf) / 2.0f;
            unitBox.Width = unitBox.Height = RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.Inf);
            GUIGUI.CreateBox(unitBox, Unit_Art_Two_Style);
        }
        #endregion

        #region BALL
        for (int i = 0; i < Battle.BallOne.Units.Count; i++)
        {
            unitBox.Left = Battle.BallOne.Units[i].Pos.x - RealBattleInfoScript.Ball_UnitSide / 2.0f;
            unitBox.Top = Battle.BallOne.Units[i].Pos.y - RealBattleInfoScript.Ball_UnitSide / 2.0f;
            unitBox.Width = unitBox.Height = RealBattleInfoScript.Ball_UnitSide;
            GUIGUI.CreateBox(unitBox, Unit_Art_One_Style);
        }

        for (int i = 0; i < Battle.BallTwo.Units.Count; i++)
        {
            unitBox.Left = Battle.BallTwo.Units[i].Pos.x - RealBattleInfoScript.Ball_UnitSide / 2.0f;
            unitBox.Top = Battle.BallTwo.Units[i].Pos.y - RealBattleInfoScript.Ball_UnitSide / 2.0f;
            unitBox.Width = unitBox.Height = RealBattleInfoScript.Ball_UnitSide;
            GUIGUI.CreateBox(unitBox, Unit_Art_Two_Style);
        }
        #endregion

        #endregion
    }
    void drawPost()
    {
        #region VICTORY
        ArmyUnitScript oneArmy = state.OneArmy;
        ArmyUnitScript twoArmy = state.TwoArmy;
        int myID = Engine.World.GetMyCountryID();
        int oneID = oneArmy.GetCountryID();
        int twoID = twoArmy.GetCountryID();
        bool isOneWin = state.GetIsOneWin();
        bool isMyOne = myID == oneID;

        string text = isMyOne == isOneWin ? Text.GetPopup(TextPopup.BattleInfo_Victory) : Text.GetPopup(TextPopup.BattleInfo_Defeat);
        if (GUIGUI.CreateButton(GUIGUI.GUIel.Battle_Pre_StartBattle, text, Choice_But_Style))
        {

            #region MAKE_INFO
            ArmyStateClass preOne, preTwo;
            preOne = new ArmyStateClass((int)oneArmy.GetInfantry(), (int)oneArmy.GetCavalry(), (int)oneArmy.GetArtillery(), (int)oneArmy.GetMorale());
            preTwo = new ArmyStateClass((int)twoArmy.GetInfantry(), (int)twoArmy.GetCavalry(), (int)twoArmy.GetArtillery(), (int)twoArmy.GetMorale());
            #endregion

            #region CHANGE_UNTS_COUNT
            oneArmy.SubInfantry(oneArmy.GetInfantry() - Battle.InfOne.GetCountReal());
            oneArmy.SubCavalry(oneArmy.GetCavalry() - Battle.CavLeftOne.GetCountReal() - Battle.CavRightOne.GetCountReal());
            oneArmy.SubArtillery(oneArmy.GetArtillery() - Battle.ArtOne.GetCountReal());

            twoArmy.SubInfantry(twoArmy.GetInfantry() - Battle.InfTwo.GetCountReal());
            twoArmy.SubCavalry(twoArmy.GetCavalry() - Battle.CavLeftTwo.GetCountReal() - Battle.CavRightTwo.GetCountReal());
            twoArmy.SubArtillery(twoArmy.GetArtillery() - Battle.ArtTwo.GetCountReal());
            #endregion

            #region MAKE_POST_INFO
            ArmyStateClass postOne, postTwo;
            postOne = new ArmyStateClass((int)oneArmy.GetInfantry(), (int)oneArmy.GetCavalry(), (int)oneArmy.GetArtillery(), (int)oneArmy.GetMorale());
            postTwo = new ArmyStateClass((int)twoArmy.GetInfantry(), (int)twoArmy.GetCavalry(), (int)twoArmy.GetArtillery(), (int)twoArmy.GetMorale());
            #endregion

            #region WAR_STATES
            int areaID = oneArmy.GetCurrAreaID();

            Engine.warStatesAndPopup_in_chechAndDoBattlesInArea(isOneWin, oneID, twoID, areaID,
                preOne, preTwo, postOne, postTwo, true);
            #endregion

            #region DELETE
            Engine.checkAndDeleteArmy_in_chechAndDoBattlesInArea(oneID);
            Engine.checkAndDeleteArmy_in_chechAndDoBattlesInArea(twoID);
            #endregion

            #region STATE_M
            GUIGUI.StateM.ClickEndRealBattle();
            #endregion
        }
        #endregion
    }
    #endregion
}
