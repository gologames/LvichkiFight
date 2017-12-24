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
    private int lballSkin;
    private int rballSkin;
    [SerializeField]
    private GameObject[] infsPrefabs;
    [SerializeField]
    private GameObject[] cavsPrefabs;
    [SerializeField]
    private GameObject[] artsPrefabs;
    [SerializeField]
    private GameObject[] ballsPrefabs;
    private RealBattleScript battle;
    private SkeletonAnimation[] linf;
    private SkeletonAnimation[] llcav;
    private SkeletonAnimation[] lrcav;
    private SkeletonAnimation[] lart;
    private SkeletonAnimation[] rinf;
    private SkeletonAnimation[] rlcav;
    private SkeletonAnimation[] rrcav;
    private SkeletonAnimation[] rart;
    private SpriteRenderer[] lball;
    private SpriteRenderer[] rball;
    private bool cameraTargetFlag = false;

    #region MOVE_ZOOM_CAMERA
    private float zoomSpeed = 110.0f;
    private float targetOrtho;
    private float smoothSpeed = 200.0f;
    private float minOrtho = 50.0f;
    private float maxOrtho = 296.0f;
    private int edgeScreenBound = 20;
    private float moveCameraSpeed = 1700.0f;
    [SerializeField]
    private SpriteRenderer back;
    #endregion

    #region INIT_PREFABS
    void InitPrefabs(ref SkeletonAnimation[] arr, GameObject prefab, int count)
    {
        arr = new SkeletonAnimation[count];
        for (int i = 0; i < count; i++)
        {
            arr[i] = Instantiate<GameObject>(prefab).GetComponent<SkeletonAnimation>();
            SetAnim(arr[i], AnimState.IDLE);
        }
    }
    void InitBallPrefabs(ref SpriteRenderer[] arr, GameObject prefab, int count)
    {
        count *= 2;
        arr = new SpriteRenderer[count];
        for (int i = 0; i < count; i++)
        { arr[i] = Instantiate<GameObject>(prefab).GetComponent<SpriteRenderer>(); }
    }
    #endregion

    #region CAMERA_TARGET
    void AddCameraTargets()
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
        ArmyUnitScript leftArmy = new ArmyUnitScript(80000, 25000, 15000);
        ArmyUnitScript rightArmy = new ArmyUnitScript(80000, 25000, 15000);
        battle = new RealBattleScript(leftArmy, rightArmy);

        InitPrefabs(ref linf, infsPrefabs[leftSkin], battle.InfOne.GetUnitsCount());
        InitPrefabs(ref llcav, cavsPrefabs[leftSkin], battle.CavLeftOne.GetUnitsCount());
        InitPrefabs(ref lrcav, cavsPrefabs[leftSkin], battle.CavRightOne.GetUnitsCount());
        InitPrefabs(ref lart, artsPrefabs[leftSkin], battle.ArtOne.GetUnitsCount());
        InitPrefabs(ref rinf, infsPrefabs[rightSkin], battle.InfTwo.GetUnitsCount());
        InitPrefabs(ref rlcav, cavsPrefabs[rightSkin], battle.CavLeftTwo.GetUnitsCount());
        InitPrefabs(ref rrcav, cavsPrefabs[rightSkin], battle.CavRightTwo.GetUnitsCount());
        InitPrefabs(ref rart, artsPrefabs[rightSkin], battle.ArtTwo.GetUnitsCount());

        lballSkin = leftSkin == 0 ? 0 : 1;
        rballSkin = (lballSkin + 1) % 2;
        InitBallPrefabs(ref lball, ballsPrefabs[lballSkin], battle.ArtOne.GetUnitsCount());
        InitBallPrefabs(ref rball, ballsPrefabs[rballSkin], battle.ArtTwo.GetUnitsCount());
    
        #region ZOOM_CAMERA
        targetOrtho = Camera.main.orthographicSize;
        #endregion
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
        MoveCamera();
        ZoomCamera();
    }

    #region ANIMATION
    void SetAnim(SkeletonAnimation anim, AnimState state)
    {
        if (anim.AnimationName != state.ToString())
        { anim.state.SetAnimation(0, state.ToString(), true); }
    }
    #endregion

    #region MOVE_ZOOM_CAMERA
    void MoveCamera()
    {
        Vector3 cameraPos = Camera.main.transform.position;
        float speed = moveCameraSpeed /
            (2.0f - (Camera.main.orthographicSize - minOrtho) / maxOrtho);
        float vSize = Camera.main.orthographicSize;
        float hSize = Camera.main.orthographicSize * Screen.width / Screen.height;
        if (Input.mousePosition.x > Screen.width - edgeScreenBound)
        {
            float newX = cameraPos.x + moveCameraSpeed * Time.deltaTime;
            float newRight = newX + hSize;
            newRight = Mathf.Clamp(newRight, back.bounds.min.x, back.bounds.max.x);
            newX = newRight - hSize;
            cameraPos = new Vector3(newX, cameraPos.y, cameraPos.z);
        }

        if (Input.mousePosition.x < edgeScreenBound)
        {
            float newX = cameraPos.x - moveCameraSpeed * Time.deltaTime;
            float newLeft = newX - hSize;
            newLeft = Mathf.Clamp(newLeft, back.bounds.min.x, back.bounds.max.x);
            newX = newLeft + hSize;
            cameraPos = new Vector3(newX, cameraPos.y, cameraPos.z);
        }

        if (Input.mousePosition.y > Screen.height - edgeScreenBound)
        {
            float newY = cameraPos.y + moveCameraSpeed * Time.deltaTime;
            float newUp = newY + vSize;
            newUp = Mathf.Clamp(newUp, back.bounds.min.y, back.bounds.max.y);
            newY = newUp - vSize;
            cameraPos = new Vector3(cameraPos.x, newY, cameraPos.z);
        }

        if (Input.mousePosition.y < edgeScreenBound)
        {
            float newY = cameraPos.y - moveCameraSpeed * Time.deltaTime;
            float newDown = newY - vSize;
            newDown = Mathf.Clamp(newDown, back.bounds.min.y, back.bounds.max.y);
            newY = newDown + vSize;
            cameraPos = new Vector3(cameraPos.x, newY, cameraPos.z);
        }
        Camera.main.transform.position = cameraPos;
        //Debug.logger(Camera.main.transform.position.y Camera.main.orthographicSize)
    }
    void ZoomCamera()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            targetOrtho -= scroll * zoomSpeed;
            targetOrtho = Mathf.Clamp(targetOrtho, minOrtho, maxOrtho);
        }

        Camera.main.orthographicSize = Mathf.MoveTowards(
            Camera.main.orthographicSize,
            targetOrtho, smoothSpeed * Time.deltaTime);

        #region REPLACE_INTO_BOUNDS
        float vSize = targetOrtho;
        float hSize = targetOrtho * Screen.width / Screen.height;

        //float maxX = back.bounds.max.x 

        float newX = Camera.main.transform.position.x;
        float newRight = newX + hSize;
        newRight = Mathf.Clamp(newRight, back.bounds.min.x, back.bounds.max.x);
        newX = newRight - hSize;

        float newLeft = newX + hSize;
        newLeft = Mathf.Clamp(newLeft, back.bounds.min.x, back.bounds.max.x);
        newX = newRight - hSize;

        float newY = Camera.main.transform.position.y;
        float newUp = newY + vSize;
        newUp = Mathf.Clamp(newUp, back.bounds.min.y, back.bounds.max.y);
        newY = newUp - vSize;

        float newDown = newY - vSize;
        newDown = Mathf.Clamp(newDown, back.bounds.min.y, back.bounds.max.y);
        newY = newDown + vSize;

        Camera.main.transform.position = new Vector3(newX,
            newY, Camera.main.transform.position.z);
        #endregion
    }
    #endregion

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
            #region SCALE_AND_POSITION
            float half_side = RealBattleInfoScript.GetUnitSide(troop) / 2.0f;
            float left = group.GetUnit(i).X - half_side - RealBattleInfoScript.FieldLeft;
            float top = group.GetUnit(i).Y - half_side - RealBattleInfoScript.FieldTop;
            arr[i].transform.position = new Vector3(left, top, 0);
            float scalePer = 1.0f - ((group.GetUnit(i).Y - RealBattleInfoScript.FieldLeft) /
                (RealBattleInfoScript.FieldWidth - RealBattleInfoScript.FieldLeft));
            scalePer *= scalePer;
            float scaleFactor = troop == RealBattleScript.RealBattleTroops.Inf ||
                troop == RealBattleScript.RealBattleTroops.Art ? 5.0f : 3.2f;
            float xScale = scaleFactor * scalePer * (group.IsOne ? 1 : -1);
            float yScale = scaleFactor * scalePer;
            arr[i].transform.localScale = new Vector3(xScale, yScale, 1.0f);
            #endregion

            #region ANIM
            switch (group.GetUnit(i).State)
            {
                case RealBattleScript.UnitStates.Stay:
                    SetAnim(arr[i], AnimState.IDLE);
                    break;
                case RealBattleScript.UnitStates.MoveOne:
                case RealBattleScript.UnitStates.MoveAll:
                    SetAnim(arr[i], AnimState.WALK);
                    break;
                case RealBattleScript.UnitStates.Fight:
                    SetAnim(arr[i], AnimState.ATTACK);
                    break;
            }
            #endregion
        }
    }
    void _drawGroupBall(RealBattleScript.BallGroup group, SpriteRenderer[] arr)
    {
        for (int i = 0; i < group.Units.Count; i++)
        {
            #region SCALE_AND_POSITION
            RealBattleScript.BallUnit ball = group.Units[i];
            float half_side = RealBattleInfoScript.Ball_UnitSide / 2.0f;
            float left = ball.Pos.x - half_side - RealBattleInfoScript.FieldLeft;
            float top = ball.Pos.y - half_side - RealBattleInfoScript.FieldTop;
            if (i >= arr.Length) break;
            arr[i].transform.position = new Vector3(left, top, 0);
            #endregion

            #region ROTATION
            Vector3 dir = ball.Dest - ball.Pos;
            if (dir != Vector3.zero)
            {
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90.0f;
                arr[i].transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
            #endregion
        }
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
        _drawGroupBall(battle.BallOne, lball);
        _drawGroupBall(battle.BallTwo, rball);

        if (cameraTargetFlag)
        {
            AddCameraTargets();
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
