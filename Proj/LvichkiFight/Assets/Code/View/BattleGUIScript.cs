using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Spine.Unity;
using Com.LuisPedroFonseca.ProCamera2D;

//здесь запуск и отрисовка боя
public class BattleGUIScript : MonoBehaviour
{
    //состояния анимации юнитов
    public enum AnimState { IDLE, WALK, HURT, ATTACK, BOW_SHOOT, DEATH }

    //номер раскраски левой армии
    [SerializeField] [Range(0, 2)]
    //номер раскраски левой армии
    private int leftSkin;
    //номер раскраски правой армии
    [SerializeField] [Range(0, 2)]
    private int rightSkin;
    //номер раскраски стрел левой армии
    private int lballSkin;
    //номер раскраски стрел правой армии
    private int rballSkin;
    //префабы пехоты
    [SerializeField]
    private GameObject[] infsPrefabs;
    //префабы кавелерии
    [SerializeField]
    private GameObject[] cavsPrefabs;
    //префабы артиллерии
    [SerializeField]
    private GameObject[] artsPrefabs;
    //префабы стрел
    [SerializeField]
    private GameObject[] ballsPrefabs;
    //класс с логикой битвы
    private RealBattleScript battle;
    //список игровых объектов пехоты левой армии
    private SkeletonAnimation[] linf;
    //список игровых объектов верхней пехоты левой армии
    private SkeletonAnimation[] llcav;
    //список игровых объектов нижней пехоты левой армии
    private SkeletonAnimation[] lrcav;
    //список игровых объектов артиллерии левой армии
    private SkeletonAnimation[] lart;
    //список игровых объектов пехоты правой армии
    private SkeletonAnimation[] rinf;
    //список игровых объектов верхней пехоты правой армии
    private SkeletonAnimation[] rlcav;
    //список игровых объектов нижней пехоты правой армии
    private SkeletonAnimation[] rrcav;
    //список игровых объектов артиллерии правой армии
    private SkeletonAnimation[] rart;
    //список игровых объектов стрел левой армии
    private SpriteRenderer[] lball;
    //список игровых объектов стрел правой армии
    private SpriteRenderer[] rball;
    //обраточик ввода (управляет камерой)
    private IInputManager inputManager;

    #region MOVE_ZOOM_CAMERA
    //коэфициент преобразования данных от колеса мыши
    //в смену масштаба камеры
    private float zoomSpeed = 110.0f;
    //в какому масштабу камера прибрижается сейчас
    //если она уже не в таком масштабе
    private float targetOrtho;
    //скорость смены масштаба камеры
    private float smoothSpeed = 200.0f;
    //минимальный масштаб камеры
    private float minOrtho = 25.0f;
    //максимальный масштаб камеры
    private float maxOrtho = 296.0f;
    //скорость движения камеры
    private float moveCameraSpeed = 170.0f;
    //фон битвы
    [SerializeField]
    private SpriteRenderer back;
    #endregion

    #region INIT_PREFABS
    //создает игровые объекты юнитов в нужном количестве
    void InitPrefabs(ref SkeletonAnimation[] arr, GameObject prefab, int count)
    {
        arr = new SkeletonAnimation[count];
        for (int i = 0; i < count; i++)
        {
            arr[i] = Instantiate<GameObject>(prefab).GetComponent<SkeletonAnimation>();
            SetAnim(arr[i], AnimState.IDLE);
        }
    }
    //создает игровые объекты стрел в нужном количестве
    //предполагается что не больше чем 2 стрелы на каждого лучника на сцене
    //остальные стрелы будут не видны
    void InitBallPrefabs(ref SpriteRenderer[] arr, GameObject prefab, int count)
    {
        count *= 2;
        arr = new SpriteRenderer[count];
        for (int i = 0; i < count; i++)
        { arr[i] = Instantiate<GameObject>(prefab).GetComponent<SpriteRenderer>(); }
    }
    #endregion

    void Start()
    {
        //правую часть от равно этой строчки изменить на создание
        //нужно класса для обработки ввода с джойстика
        inputManager = new InputManager();

        //создания класса с логикой битвы
        ArmyUnitScript leftArmy = new ArmyUnitScript(
            PlayerPrefs.GetInt(StartFight.LeftInfKey) * 1000,
            PlayerPrefs.GetInt(StartFight.LeftCavKey) * 1000,
            PlayerPrefs.GetInt(StartFight.LeftArtKey) * 1000);
        ArmyUnitScript rightArmy = new ArmyUnitScript(
            PlayerPrefs.GetInt(StartFight.RightInfKey) * 1000,
            PlayerPrefs.GetInt(StartFight.RightCavKey) * 1000,
            PlayerPrefs.GetInt(StartFight.RightArtKey) * 1000);
        battle = new RealBattleScript(leftArmy, rightArmy);

        //получение информации о номерах скинов
        leftSkin = PlayerPrefs.GetInt(StartFight.LeftSkinKey);
        rightSkin = PlayerPrefs.GetInt(StartFight.RightSkinKey);

        //создание игровых объектов юнитов
        InitPrefabs(ref linf, infsPrefabs[leftSkin], battle.InfOne.GetUnitsCount());
        InitPrefabs(ref llcav, cavsPrefabs[leftSkin], battle.CavLeftOne.GetUnitsCount());
        InitPrefabs(ref lrcav, cavsPrefabs[leftSkin], battle.CavRightOne.GetUnitsCount());
        InitPrefabs(ref lart, artsPrefabs[leftSkin], battle.ArtOne.GetUnitsCount());
        InitPrefabs(ref rinf, infsPrefabs[rightSkin], battle.InfTwo.GetUnitsCount());
        InitPrefabs(ref rlcav, cavsPrefabs[rightSkin], battle.CavLeftTwo.GetUnitsCount());
        InitPrefabs(ref rrcav, cavsPrefabs[rightSkin], battle.CavRightTwo.GetUnitsCount());
        InitPrefabs(ref rart, artsPrefabs[rightSkin], battle.ArtTwo.GetUnitsCount());

        //скинов юнитов 3 типа, а стрел 2 типа и я решил сделать так
        lballSkin = leftSkin == 0 ? 0 : 1;
        rballSkin = (lballSkin + 1) % 2;
        //создание игровых объектов стрел
        InitBallPrefabs(ref lball, ballsPrefabs[lballSkin], battle.ArtOne.GetUnitsCount());
        InitBallPrefabs(ref rball, ballsPrefabs[rballSkin], battle.ArtTwo.GetUnitsCount());
    
        #region ZOOM_CAMERA
        targetOrtho = Camera.main.orthographicSize;
        #endregion
    }

    void Update()
    {
        //код обновляющий логику битвы
        BattleDeltaTimeScript.Update();
        if (BattleDeltaTimeScript.IsNextFrame())
        { battle.Update(BattleDeltaTimeScript.SpeedTime); }

        //удаление игровых объект для умерших юнитов
        DestroyDead();
        //отрисока битвы
        drawFight();
        //обработка движения камеры
        MoveCamera();
        //обработка смены масштаба камеры
        ZoomCamera();
        //проверка на победу одной из армий
        CheckVictory();
    }

    #region ANIMATION
    //устанавливает для данного юнита нужную анимацию
    void SetAnim(SkeletonAnimation anim, AnimState state, bool loop = true)
    {
        if (anim.AnimationName != state.ToString())
        { anim.state.SetAnimation(0, state.ToString(), loop); }
    }
    #endregion

    #region MOVE_ZOOM_CAMERA
    //обрабатывает движение камеры на основе inputManager
    void MoveCamera()
    {
        Vector3 cameraPos = Camera.main.transform.position;
        float speed = moveCameraSpeed /
            (2.0f - (Camera.main.orthographicSize - minOrtho) / maxOrtho);
        float vSize = Camera.main.orthographicSize;
        float hSize = Camera.main.orthographicSize * Screen.width / Screen.height;
        if (inputManager.IsMoveRight())
        {
            float newX = cameraPos.x + moveCameraSpeed * Time.deltaTime;
            cameraPos = new Vector3(newX, cameraPos.y, cameraPos.z);
        }

        if (inputManager.IsMoveLeft())
        {
            float newX = cameraPos.x - moveCameraSpeed * Time.deltaTime;
            cameraPos = new Vector3(newX, cameraPos.y, cameraPos.z);
        }

        if (inputManager.IsMoveUp())
        {
            float newY = cameraPos.y + moveCameraSpeed * Time.deltaTime;
            cameraPos = new Vector3(cameraPos.x, newY, cameraPos.z);
        }

        if (inputManager.IsMoveDown())
        {
            float newY = cameraPos.y - moveCameraSpeed * Time.deltaTime;
            cameraPos = new Vector3(cameraPos.x, newY, cameraPos.z);
        }
        Camera.main.transform.position = cameraPos;
    }
    //обрабатывает смену масштаба камеры на основе inputManager
    void ZoomCamera()
    {
        float scroll = inputManager.GetScroll();
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

        float newX = Camera.main.transform.position.x;
        float newRight = newX + hSize;
        newRight = Mathf.Clamp(newRight, back.bounds.min.x, back.bounds.max.x);
        newX = newRight - hSize;

        float newLeft = newX - hSize;
        newLeft = Mathf.Clamp(newLeft, back.bounds.min.x, back.bounds.max.x);
        newX = newLeft + hSize;

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
    //выставляет анимацию смерти и прозрачность 0.2 для умерших юнитов
    void _DestroyDead(RealBattleScript.BattleGroup group, SkeletonAnimation[] arr)
    {
        for (int i = group.GetUnitsCount(); i < arr.Length; i++)
        {
            if (arr[i] != null)
            {
                SetAnim(arr[i], AnimState.DEATH, false);
                arr[i].skeleton.SetColor(new Color(0.0f, 0.0f, 0.0f, 0.2f));
                arr[i] = null;
            }
            else break;
        }
    }
    void DestroyDead()
    {
        _DestroyDead(battle.InfOne, linf);
        _DestroyDead(battle.CavLeftOne, llcav);
        _DestroyDead(battle.CavRightOne, lrcav);
        _DestroyDead(battle.ArtOne, lart);
        _DestroyDead(battle.InfTwo, rinf);
        _DestroyDead(battle.CavLeftTwo, rlcav);
        _DestroyDead(battle.CavRightTwo, rrcav);
        _DestroyDead(battle.ArtTwo, rart);
    }
    #endregion

    #region DRAW_FIGHT
    //отрисовывает группу юнитов
    void _drawGroup(RealBattleScript.BattleGroup group,
        RealBattleScript.RealBattleTroops troop, SkeletonAnimation[] arr)
    {
        for (int i = 0; i < group.GetUnitsCount(); i++)
        {
            //считает нужную пощицию на поле и размер юнита
            //повыше - поменьше, пониже - побольше
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

            //выставляет анимацию по информации из логики битвы
            #region ANIM
            RealBattleScript.BattleUnit unit = group.GetUnit(i);
            switch (unit.State)
            {
                case RealBattleScript.UnitStates.Stay:
                    SetAnim(arr[i], AnimState.IDLE);
                    break;
                case RealBattleScript.UnitStates.MoveOne:
                case RealBattleScript.UnitStates.MoveAll:
                    SetAnim(arr[i], AnimState.WALK);
                    break;
                case RealBattleScript.UnitStates.Fight:
                    if (troop != RealBattleScript.RealBattleTroops.Art)
                    { SetAnim(arr[i], AnimState.ATTACK); }
                    else if (unit.ArtShoot)
	                {
                        SetAnim(arr[i], AnimState.BOW_SHOOT, false);
                        arr[i].state.Complete += delegate(Spine.TrackEntry entry)
                        { unit.ArtShoot = false; };
                    }
                    else SetAnim(arr[i], AnimState.IDLE, true);
                    break;
            }
            #endregion
        }
    }
    //отрисовывает группу стрел
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
        
    }
    #endregion

    #region CHECK_VICTORY
    //если у одной армии нет войск больше, то другая победила
    //также тут запускается нужная сцена после боя
    void CheckVictory()
    {
        int leftInf = (int)battle.InfOne.GetCountReal();
        int leftCav = (int)(battle.CavLeftOne.GetCountReal() +
            battle.CavRightOne.GetCountReal());
        int leftArt = (int)battle.ArtOne.GetCountReal();
        int leftAll = leftInf + leftCav + leftArt;

        int rightInf = (int)battle.InfTwo.GetCountReal();
        int rightCav = (int)(battle.CavLeftTwo.GetCountReal() +
            battle.CavRightTwo.GetCountReal());
        int rightArt = (int)battle.ArtTwo.GetCountReal();
        int rightAll = rightInf + rightCav + rightArt;

        if (leftAll == 0  || rightAll == 0)
        {
            PlayerPrefs.SetInt(StartFight.LeftInfKey, leftInf / 1000);
            PlayerPrefs.SetInt(StartFight.LeftCavKey, leftCav / 1000);
            PlayerPrefs.SetInt(StartFight.LeftArtKey, leftArt / 1000);
            PlayerPrefs.SetInt(StartFight.RightInfKey, rightInf / 1000);
            PlayerPrefs.SetInt(StartFight.RightCavKey, rightCav / 1000);
            PlayerPrefs.SetInt(StartFight.RightArtKey, rightArt / 1000);
            PlayerPrefs.SetString(StartFight.WinKey,
                leftAll == 0 ? StartFight.RightWinKey :
                StartFight.LeftWinKey);
            SceneManager.LoadScene(PlayerPrefs.GetInt(
                StartFight.AfterSceneNumKey));
        }
    }
    #endregion
}
