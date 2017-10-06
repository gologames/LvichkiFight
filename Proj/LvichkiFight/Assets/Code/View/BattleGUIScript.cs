using UnityEngine;
using System.Collections;

public class BattleGUIScript : MonoBehaviour
{
    private GameObject EngineObj;
    public EngineScript Engine;

    private GameObject GUIGUIObj;
    public GUIGUIScript GUIGUI;

    private RealBattleScript Battle = null;

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
        EngineObj = GameObject.Find("Engine");
        Engine = (EngineScript)EngineObj.GetComponent("EngineScript");

        GUIGUIObj = GameObject.Find("GUIGUI");
        GUIGUI = (GUIGUIScript)GUIGUIObj.GetComponent("GUIGUIScript");

        initStyles();
    }

    #region INIT_STYLES
    void initStyles()
    {
        #region CHOICE

        #region BATTLE
        Choice_Battle_Style.fontSize = 32;
        Choice_Battle_Style.alignment = TextAnchor.MiddleCenter;
        Choice_Battle_Style.contentOffset = new Vector2(-1.0f, -0.0f);
        #endregion

        #region BUT
        Choice_But_Style.fontSize = 32;
        Choice_But_Style.alignment = TextAnchor.MiddleCenter;
        #endregion

        #endregion
    }
    #endregion

    void Update()
    {
        if (GUIGUI.GameScreenState == GameScreenEnum.Game &&
            GUIGUI.StateM.State == GameState.Battle &&
            GUIGUI.StateM.Battle_State == BattleState.Fight)
        {
            BattleDeltaTimeScript.Update();
            if (BattleDeltaTimeScript.IsNextFrame())
            {
                Battle.Update(BattleDeltaTimeScript.SpeedTime);
            }
        }
    }

    void OnGUI()
    {
        GUI.depth = GUIDepthScript.Battle;
        if (GUIGUI.GameScreenState == GameScreenEnum.Game &&
            GUIGUI.StateM.State == GameState.Battle)
        {
            switch (GUIGUI.StateM.Battle_State)
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
    }

    #region CHOICE
    void drawChoice()
    {
        ArmyUnitScript oneArmy = GUIGUI.StateM.OneArmy;
        ArmyUnitScript twoArmy = GUIGUI.StateM.TwoArmy;
        int oneID = oneArmy.GetCountryID();
        int twoID = twoArmy.GetCountryID();

        #region BACKGROUND
        GUIGUI.CreateBox(GUIGUI.GUIel.Popup_Msg_Background, Choice_Background_Style);
        #endregion

        #region BATTLE
        string battleText = Text.GetBattle(TextBattle.Battle);
        GUIGUI.CreateBox(GUIGUI.GUIel.Popup_Msg_BattleInfo_Victory_V, battleText, Choice_Battle_Style);
        #endregion

        #region FLAGS
        GUIGUI.CreateBox(GUIGUI.GUIel.Popup_Msg_BattleInfo_Flag_Left, GUIGUI.Icons_Flags_Styles[oneID]);
        GUIGUI.CreateBox(GUIGUI.GUIel.Popup_Msg_BattleInfo_Flag_Right, GUIGUI.Icons_Flags_Styles[twoID]);
        #endregion

        #region COUNTRY_NAMES
        string oneCountryName = Text.GetCountryName((TextCountriesNames)oneID);
        string twoCountryName = Text.GetCountryName((TextCountriesNames)twoID);

        GUIGUI.CreateBox(GUIGUI.GUIel.Popup_Msg_BattleInfo_CountryName_Left, oneCountryName, Text_Style);
        GUIGUI.CreateBox(GUIGUI.GUIel.Popup_Msg_BattleInfo_CountryName_Right, twoCountryName, Text_Style);
        #endregion

        #region AREA_NAME
        string areaNameText = Text.GetPopup(TextPopup.Area) + oneArmy.GetCurrAreaID().ToString();
        GUIGUI.CreateBox(GUIGUI.GUIel.Popup_Msg_BattleInfo_AreaName, areaNameText, Text_Style);
        #endregion

        #region INFO
        GUIGUI.CreateBox(GUIGUI.GUIel.Popup_Msg_BattleInfo_PreInfo_Left_Label_All, GUIGUI.Icons_AllTroops_Style);
        GUIGUI.CreateBox(GUIGUI.GUIel.Popup_Msg_BattleInfo_PreInfo_Left_Label_Inf, GUIGUI.Icons_Infantry_Style);
        GUIGUI.CreateBox(GUIGUI.GUIel.Popup_Msg_BattleInfo_PreInfo_Left_Label_Cav, GUIGUI.Icons_Cavalry_Style);
        GUIGUI.CreateBox(GUIGUI.GUIel.Popup_Msg_BattleInfo_PreInfo_Left_Label_Art, GUIGUI.Icons_Artillery_Style);
        GUIGUI.CreateBox(GUIGUI.GUIel.Popup_Msg_BattleInfo_PreInfo_Left_Label_Morale, GUIGUI.Icons_Morale_Style);

        GUIGUI.CreateBox(GUIGUI.GUIel.Popup_Msg_BattleInfo_PreInfo_Right_Label_All, GUIGUI.Icons_AllTroops_Style);
        GUIGUI.CreateBox(GUIGUI.GUIel.Popup_Msg_BattleInfo_PreInfo_Right_Label_Inf, GUIGUI.Icons_Infantry_Style);
        GUIGUI.CreateBox(GUIGUI.GUIel.Popup_Msg_BattleInfo_PreInfo_Right_Label_Cav, GUIGUI.Icons_Cavalry_Style);
        GUIGUI.CreateBox(GUIGUI.GUIel.Popup_Msg_BattleInfo_PreInfo_Right_Label_Art, GUIGUI.Icons_Artillery_Style);
        GUIGUI.CreateBox(GUIGUI.GUIel.Popup_Msg_BattleInfo_PreInfo_Right_Label_Morale, GUIGUI.Icons_Morale_Style);

        int oneInf = (int)oneArmy.GetInfantry();
        int oneCav = (int)oneArmy.GetCavalry();
        int oneArt = (int)oneArmy.GetArtillery();
        int oneAll = oneInf + oneCav + oneArt;
        int oneMorale = (int)oneArmy.GetMorale();

        int twoInf = (int)twoArmy.GetInfantry();
        int twoCav = (int)twoArmy.GetCavalry();
        int twoArt = (int)twoArmy.GetArtillery();
        int twoAll = twoInf + twoCav + twoArt;
        int twoMorale = (int)twoArmy.GetMorale();

        GUIGUI.CreateBox(GUIGUI.GUIel.Popup_Msg_BattleInfo_PreInfo_Left_Text_All, oneAll.ToString(), Text_Style);
        GUIGUI.CreateBox(GUIGUI.GUIel.Popup_Msg_BattleInfo_PreInfo_Left_Text_Inf, oneInf.ToString(), Text_Style);
        GUIGUI.CreateBox(GUIGUI.GUIel.Popup_Msg_BattleInfo_PreInfo_Left_Text_Cav, oneCav.ToString(), Text_Style);
        GUIGUI.CreateBox(GUIGUI.GUIel.Popup_Msg_BattleInfo_PreInfo_Left_Text_Art, oneArt.ToString(), Text_Style);
        GUIGUI.CreateBox(GUIGUI.GUIel.Popup_Msg_BattleInfo_PreInfo_Left_Text_Morale, oneMorale.ToString(), Text_Style);

        GUIGUI.CreateBox(GUIGUI.GUIel.Popup_Msg_BattleInfo_PreInfo_Right_Text_All, twoAll.ToString(), Text_Style);
        GUIGUI.CreateBox(GUIGUI.GUIel.Popup_Msg_BattleInfo_PreInfo_Right_Text_Inf, twoInf.ToString(), Text_Style);
        GUIGUI.CreateBox(GUIGUI.GUIel.Popup_Msg_BattleInfo_PreInfo_Right_Text_Cav, twoCav.ToString(), Text_Style);
        GUIGUI.CreateBox(GUIGUI.GUIel.Popup_Msg_BattleInfo_PreInfo_Right_Text_Art, twoArt.ToString(), Text_Style);
        GUIGUI.CreateBox(GUIGUI.GUIel.Popup_Msg_BattleInfo_PreInfo_Right_Text_Morale, twoMorale.ToString(), Text_Style);
        #endregion

        #region BUTTONS
        if (GUIGUI.CreateButton(GUIGUI.GUIel.SplitArmy_But_Cancel, Text.GetBattle(TextBattle.QuickBattle), Choice_But_Style))
        {
            ArmyStateClass preOne, preTwo, postOne, postTwo;
            bool isOneWin = Engine.autoBattle_in_chechAndDoBattlesInArea(oneArmy, twoArmy, out preOne, out preTwo, out postOne, out postTwo);
            int exAreaID = oneArmy.GetCurrAreaID();
            Engine.warStatesAndPopup_in_chechAndDoBattlesInArea(isOneWin, oneID, twoID, exAreaID,
                preOne, preTwo, postOne, postTwo, false);

            Engine.checkAndDeleteArmy_in_chechAndDoBattlesInArea(oneID);
            Engine.checkAndDeleteArmy_in_chechAndDoBattlesInArea(twoID);

            GUIGUI.StateM.ClickQuickBattle();
        }
        if (GUIGUI.CreateButton(GUIGUI.GUIel.SplitArmy_But_OK, Text.GetBattle(TextBattle.Simulation), Choice_But_Style))
        {
            Battle = new RealBattleScript(oneArmy, twoArmy);
            GUIGUI.StateM.ClickPreRealBattle();
        }
        #endregion
    }
    #endregion

    #region FIGHT
    void drawPre()
    {
        #region START_BATTLE
        if (GUIGUI.CreateButton(GUIGUI.GUIel.Battle_Pre_StartBattle, Text.GetBattle(TextBattle.StartBattle), Choice_But_Style))
        {
            GUIGUI.StateM.ClickFightRealBattle();
        }
        #endregion
    }
    void drawFight()
    {
        int oneID = GUIGUI.StateM.OneArmy.GetCountryID();
        int twoID = GUIGUI.StateM.TwoArmy.GetCountryID();

        #region BACKGORUND
        GUIGUI.CreateBox(GUIGUI.GUIel.Battle_Fight_Background, Fight_Back_Style);
        #endregion

        #region FLAGS
        GUIGUI.CreateBox(GUIGUI.GUIel.Battle_Fight_One_Flag, GUIGUI.Icons_Flags_Styles[oneID]);
        GUIGUI.CreateBox(GUIGUI.GUIel.Battle_Fight_Two_Flag, GUIGUI.Icons_Flags_Styles[twoID]);
        #endregion

        #region COUNTRY_NAMES
        string oneCountryName = Text.GetCountryName((TextCountriesNames)oneID);
        string twoCountryName = Text.GetCountryName((TextCountriesNames)twoID);

        GUIGUI.CreateBox(GUIGUI.GUIel.Battle_Fight_One_CountryName, oneCountryName, Text_Style);
        GUIGUI.CreateBox(GUIGUI.GUIel.Battle_Fight_Two_CountryName, twoCountryName, Text_Style);
        #endregion

        #region INFO
        GUIGUI.CreateBox(GUIGUI.GUIel.Battle_Fight_One_Label_All, GUIGUI.Icons_AllTroops_Style);
        GUIGUI.CreateBox(GUIGUI.GUIel.Battle_Fight_One_Label_Inf, GUIGUI.Icons_Infantry_Style);
        GUIGUI.CreateBox(GUIGUI.GUIel.Battle_Fight_One_Label_Cav, GUIGUI.Icons_Cavalry_Style);
        GUIGUI.CreateBox(GUIGUI.GUIel.Battle_Fight_One_Label_Art, GUIGUI.Icons_Artillery_Style);
        GUIGUI.CreateBox(GUIGUI.GUIel.Battle_Fight_One_Label_Morale, GUIGUI.Icons_Morale_Style);

        GUIGUI.CreateBox(GUIGUI.GUIel.Battle_Fight_Two_Label_All, GUIGUI.Icons_AllTroops_Style);
        GUIGUI.CreateBox(GUIGUI.GUIel.Battle_Fight_Two_Label_Inf, GUIGUI.Icons_Infantry_Style);
        GUIGUI.CreateBox(GUIGUI.GUIel.Battle_Fight_Two_Label_Cav, GUIGUI.Icons_Cavalry_Style);
        GUIGUI.CreateBox(GUIGUI.GUIel.Battle_Fight_Two_Label_Art, GUIGUI.Icons_Artillery_Style);
        GUIGUI.CreateBox(GUIGUI.GUIel.Battle_Fight_Two_Label_Morale, GUIGUI.Icons_Morale_Style);

        int oneInf = (int)Battle.InfOne.GetCountReal();
        int oneCav = (int)(Battle.CavLeftOne.GetCountReal() + Battle.CavRightOne.GetCountReal());
        int oneArt = (int)Battle.ArtOne.GetCountReal();
        int oneAll = oneInf + oneCav + oneArt;
        int oneMorale = (int)Battle.GetMoraleOne();

        int twoInf = (int)Battle.InfTwo.GetCountReal();
        int twoCav = (int)(Battle.CavLeftTwo.GetCountReal() + Battle.CavRightTwo.GetCountReal());
        int twoArt = (int)Battle.ArtTwo.GetCountReal();
        int twoAll = twoInf + twoCav + twoArt;
        int twoMorale = 0;

        GUIGUI.CreateBox(GUIGUI.GUIel.Battle_Fight_One_Text_All, oneAll.ToString(), Text_Style);
        GUIGUI.CreateBox(GUIGUI.GUIel.Battle_Fight_One_Text_Inf, oneInf.ToString(), Text_Style);
        GUIGUI.CreateBox(GUIGUI.GUIel.Battle_Fight_One_Text_Cav, oneCav.ToString(), Text_Style);
        GUIGUI.CreateBox(GUIGUI.GUIel.Battle_Fight_One_Text_Art, oneArt.ToString(), Text_Style);
        GUIGUI.CreateBox(GUIGUI.GUIel.Battle_Fight_One_Text_Morale, oneMorale.ToString(), Text_Style);

        GUIGUI.CreateBox(GUIGUI.GUIel.Battle_Fight_Two_Text_All, twoAll.ToString(), Text_Style);
        GUIGUI.CreateBox(GUIGUI.GUIel.Battle_Fight_Two_Text_Inf, twoInf.ToString(), Text_Style);
        GUIGUI.CreateBox(GUIGUI.GUIel.Battle_Fight_Two_Text_Cav, twoCav.ToString(), Text_Style);
        GUIGUI.CreateBox(GUIGUI.GUIel.Battle_Fight_Two_Text_Art, twoArt.ToString(), Text_Style);
        GUIGUI.CreateBox(GUIGUI.GUIel.Battle_Fight_Two_Text_Morale, twoMorale.ToString(), Text_Style);
        #endregion

        #region VICTORY
        if (GUIGUI.StateM.Battle_State == BattleState.Fight)
        {
            if (oneAll == 0)
            {
                GUIGUI.StateM.ClickPostRealBattle(false);
            }
            if (twoAll == 0)
            {
                GUIGUI.StateM.ClickPostRealBattle(true);
            }
        }
        #endregion

        #region FIELD
        GUIBox unitBox = new GUIBox(0, 0, 0, 0, "");

        #region INF
        for (int i = 0; i < Battle.InfOne.GetUnitsCount(); i++)
        {
            unitBox.Left = Battle.InfOne.GetUnit(i).X - RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.Inf) / 2.0f;
            unitBox.Top = Battle.InfOne.GetUnit(i).Y - RealBattleInfoScript.GetUnitSide(RealBattleScript.RealBattleTroops.Inf) / 2.0f;
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
        ArmyUnitScript oneArmy = GUIGUI.StateM.OneArmy;
        ArmyUnitScript twoArmy = GUIGUI.StateM.TwoArmy;
        int myID = Engine.World.GetMyCountryID();
        int oneID = oneArmy.GetCountryID();
        int twoID = twoArmy.GetCountryID();
        bool isOneWin = GUIGUI.StateM.GetIsOneWin();
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
