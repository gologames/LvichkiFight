using UnityEngine;
using System.Collections.Generic;
using Spine.Unity;
using Com.LuisPedroFonseca.ProCamera2D;

public class BattleGUIScript : MonoBehaviour
{
    public enum AnimState { IDLE, WALK, HURT, ATTACK, DEATH }

    [SerializeField] [Range(0, 2)]
    private int leftSkin;
    [SerializeField] [Range(0, 2)]
    private int rightSkin;
    [SerializeField]
    private GameObject[] infsPrefabs;
    [SerializeField]
    private GameObject[] cavsPrefabs;
    [SerializeField]
    private GameObject[] artsPrefabs;
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
        {
            arr[i] = Instantiate<GameObject>(prefab).GetComponent<SkeletonAnimation>();
            arr[i].state.SetAnimation(0, AnimState.WALK.ToString(), true);
        }
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
        ArmyUnitScript leftArmy = new ArmyUnitScript(30000, 20000, 10000);
        ArmyUnitScript rightArmy = new ArmyUnitScript(30000, 20000, 10000);
        battle = new RealBattleScript(leftArmy, rightArmy);

        InitPrefabs(ref linf, infsPrefabs[leftSkin], battle.InfOne.GetUnitsCount());
        InitPrefabs(ref llcav, cavsPrefabs[leftSkin], battle.CavLeftOne.GetUnitsCount());
        InitPrefabs(ref lrcav, cavsPrefabs[leftSkin], battle.CavRightOne.GetUnitsCount());
        InitPrefabs(ref lart, artsPrefabs[leftSkin], battle.ArtOne.GetUnitsCount());
        InitPrefabs(ref rinf, infsPrefabs[rightSkin], battle.InfTwo.GetUnitsCount());
        InitPrefabs(ref rlcav, cavsPrefabs[rightSkin], battle.CavLeftTwo.GetUnitsCount());
        InitPrefabs(ref rrcav, cavsPrefabs[rightSkin], battle.CavRightTwo.GetUnitsCount());
        InitPrefabs(ref rart, artsPrefabs[rightSkin], battle.ArtTwo.GetUnitsCount());
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
        { if (arr[i] != null) Destroy(arr[i].gameObject); else break; }
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

    #region DRAW_FIGHT


    void _drawGroup(RealBattleScript.BattleGroup group,
        RealBattleScript.RealBattleTroops troop, SkeletonAnimation[] arr)
    {
        for (int i = 0; i < group.GetUnitsCount(); i++)
        {
            float side = RealBattleInfoScript.GetUnitSide(troop);
            float left = group.GetUnit(i).X - side / 2.0f - RealBattleInfoScript.FieldLeft;
            float top = group.GetUnit(i).Y - side / 2.0f - RealBattleInfoScript.FieldTop;
            arr[i].transform.position = new Vector3(left, top, 0);
        }
    }
    void _drawGroupBall(RealBattleScript.BallGroup group,
        RealBattleScript.RealBattleTroops troop, SkeletonAnimation[] arr)
    {
        //for (int i = 0; i < group.Units.Count; i++)
        //{
        //    unitBox.Left = battle.BallOne.Units[i].Pos.x - RealBattleInfoScript.Ball_UnitSide / 2.0f;
        //    unitBox.Top = battle.BallOne.Units[i].Pos.y - RealBattleInfoScript.Ball_UnitSide / 2.0f;
        //    unitBox.Width = unitBox.Height = RealBattleInfoScript.Ball_UnitSide;
        //    GUIGUI.CreateBox(unitBox, Unit_Art_One_Style);
        //}
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

        _drawGroup(battle.InfOne, RealBattleScript.RealBattleTroops.Inf, linf);
        _drawGroup(battle.InfTwo, RealBattleScript.RealBattleTroops.Inf, rinf);
        _drawGroup(battle.CavLeftOne, RealBattleScript.RealBattleTroops.CavLeft, llcav);
        _drawGroup(battle.CavRightOne, RealBattleScript.RealBattleTroops.CavRight, lrcav);
        _drawGroup(battle.CavLeftTwo, RealBattleScript.RealBattleTroops.CavLeft, rlcav);
        _drawGroup(battle.CavRightTwo, RealBattleScript.RealBattleTroops.CavRight, rrcav);
        _drawGroup(battle.ArtOne, RealBattleScript.RealBattleTroops.Art, lart);
        _drawGroup(battle.ArtTwo, RealBattleScript.RealBattleTroops.Art, rart);

        if (cameraTargetFlag)
        {
            addCameraTargets();
            cameraTargetFlag = false;
        }


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
    #endregion

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
