using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using Dev.Scripts.GUI;
using Dev.Scripts.System;
using Dev.Scripts.Targets;
using UnityEngine.UI;

public class SquareBlocks
{
    public SquareTypes Block;
    public SquareTypes Obstacle;
}

public enum GameState
{
    Map,
    PrepareGame,
    Playing,
    Highscore,
    GameOver,
    Pause,
    PreWinAnimations,
    Win,
    WaitForPopup,
    WaitAfterClose,
    BlockedGame,
    Tutorial,
    PreTutorial,
    WaitForPotion,
    PreFailed,
    RegenLevel
}


public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    [Header("Item & Block Prefabs")]
    public GameObject itemPrefab;
    public GameObject squarePrefab;
    public GameObject blockPrefab;
    public GameObject wireBlockPrefab;
    public GameObject solidBlockPrefab;
    public GameObject undestroyableBlockPrefab;
    public GameObject thrivingBlockPrefab;
    
    [Space,Header("Sprites")]
    public Sprite squareSprite;
    public Sprite squareSprite1;
    public Sprite outline1;
    public Sprite outline2;
    public Sprite outline3;
    
    public LifeShop lifeShop;
    public Transform gameField;
    public bool enableInApps;
    public int maxRows = 9;
    public int maxCols = 9;
    public float squareWidth = 1.2f;
    public float squareHeight = 1.2f;
    public Vector2 firstSquarePosition;
    public Square[] squaresArray;
    
    [HideInInspector]
    public Item lastDraggedItem;
    [HideInInspector]
    public Item lastSwitchedItem;
    
    public List<Item> destroyAnyway = new List<Item>();
    public GameObject popupScore;
    
    [Space,Header("Score Settings")]
    public int scoreForItem = 10;
    public int scoreForBlock = 100;
    public int scoreForWireBlock = 100;
    public int scoreForSolidBlock = 100;
    public int scoreForThrivingBlock = 100;
    
    public LIMIT limitType;
    public int limit = 30;
    public int targetScore = 1000;
    public int currentLevel = 1;
    public int failedCost;
    public int extraFailedMoves = 5;
    public int extraFailedSecs = 30;
    public List<GemProduct> gemsProducts = new List<GemProduct>();

    public int boostColorfullBomb;
    public int boostPackage;
    public int boostStriped;
    public BoostIcon emptyBoostIcon;
    public BoostIcon activatedBoost;
    public bool thrivingBlockDestroyed;
    
    private LineRenderer _line;
    private List<List<Item>> _newCombines;
    private List<List<Item>> _combinedItems = new List<List<Item>>();
    private bool _dragBlocked;
    

    public BoostIcon ActivatedBoost
    {
        get
        {
            if (activatedBoost == null)
            {
                return emptyBoostIcon;
            }
            return activatedBoost;
        }
        set
        {
            if (value == null)
            {
                if (activatedBoost != null && GameStatus == GameState.Playing)
                    InitScript.Instance.SpendBoost(activatedBoost.type);
                UnLockBoosts();
            }
            activatedBoost = value;

            if (value != null)
            {
                LockBoosts();
            }

            if (activatedBoost != null)
            {
                if (activatedBoost.type == BoostType.ExtraMoves || activatedBoost.type == BoostType.ExtraTime)
                {
                    if (limitType == LIMIT.MOVES)
                        limit += 5;
                    else
                        limit += 30;

                    ActivatedBoost = null;
                }

            }
        }
    }
    
    private bool _ingredientFly;
    private int _linePoint;
    private SquareBlocks[] _levelSquaresFile = new SquareBlocks[81];
    
    public int targetBlocks;

    public GameObject[] itemExplPool = new GameObject[20];
    public static int Score;
    public int stars;
    
    public int star1;
    public int star2;
    public int star3;
    public bool showPopupScores;

    public GameObject stripesEffect;
    public GameObject star1Anim;
    public GameObject star2Anim;
    public GameObject star3Anim;
    public GameObject snowParticle;
    public Color[] scoresColors;
    public Color[] scoresColorsOutline;
    public int colorLimit;
    public int[] ingrCountTarget = new int[2];
    public Ingredients[] ingrTarget = new Ingredients[2];
    public CollectItems[] collectItems = new CollectItems[2];
    public Sprite[] ingrediendSprites;
    public string[] targetDiscriptions;
    public GameObject ingrObject;
    public GameObject blocksObject;
    public GameObject scoreTargetObject;
    
    public GameObject[] gratzWords;

    public GameObject level;
    public GameObject levelsMap;

    public BoostIcon[] inGameBoosts;
    public int passLevelCounter;

    public Target target;

    public int TargetBlocks
    {
        get
        {
            return targetBlocks;
        }
        set
        {
            if (targetBlocks < 0)
                targetBlocks = 0;
            targetBlocks = value;
        }
    }

    public bool DragBlocked
    {
        get => _dragBlocked;
        set
        {
            if (value)
            {
                List<Item> items = GetItems();
                foreach (Item item in items)
                {
                    if (item != null)
                       item.anim.SetBool(Stop, true);
                }
            }
            else
            {
                  StartCoroutine( StartIdleCor());
            }
            _dragBlocked = value;
        }
    }

    private GameState _gameStatus;
    public bool itemsHided;
    public int moveID;
    public int lastRandColor;
    public bool onlyFalling;
    public bool levelLoaded;
    public Hashtable countedSquares;
    public Sprite doubleBlock;
    private CombineManager _combineManager;
    public TargetObject[] targetObject;
    
    private static readonly int Stop = Animator.StringToHash("stop");

    #region EVENTS

    public delegate void GameStateEvents();

    public static event GameStateEvents OnMapState;
    public static event GameStateEvents OnEnterGame;
    public static event GameStateEvents OnLevelLoaded;
    public static event GameStateEvents OnMenuPlay;
    public static event GameStateEvents OnMenuComplete;
    public static event GameStateEvents OnStartPlay;
    public static event GameStateEvents OnWin;
    public static event GameStateEvents OnLose;

    public GameState GameStatus
    {
        get
        {
            return _gameStatus;
        }
        set
        {
            _gameStatus = value;
            InitScript.Instance.CheckAdsEvents(value);

            if (value == GameState.PrepareGame)
            {
                passLevelCounter++;

                MusicBase.Instance.GetComponent<AudioSource>().Stop();
                MusicBase.Instance.GetComponent<AudioSource>().loop = true;
                MusicBase.Instance.GetComponent<AudioSource>().clip = MusicBase.Instance.music[1];
                MusicBase.Instance.GetComponent<AudioSource>().Play();
                PrepareGame();
            }
            else if (value == GameState.WaitForPopup)
            {
                InitLevel();
                if (OnLevelLoaded != null) OnLevelLoaded();
            }
            else if (value == GameState.PreFailed)
            {
                GameObject.Find("CanvasGlobal").transform.Find("PreFailed").gameObject.SetActive(true);

            }
            else if (value == GameState.Map)
            {
                if (PlayerPrefs.GetInt("OpenLevelTest") <= 0)
                {
                    MusicBase.Instance.GetComponent<AudioSource>().Stop();
                    MusicBase.Instance.GetComponent<AudioSource>().loop = true;
                    MusicBase.Instance.GetComponent<AudioSource>().clip = MusicBase.Instance.music[0];
                    MusicBase.Instance.GetComponent<AudioSource>().Play();
                    EnableMap(true);
                    OnMapState();
                }
                else
                {
                    LevelManager.Instance.GameStatus = GameState.PrepareGame;
                    PlayerPrefs.SetInt("OpenLevelTest", 0);
                    PlayerPrefs.Save();
                }
                if (passLevelCounter > 0 && InitScript.Instance.ShowRateEvery > 0)
                {
                    if (passLevelCounter % InitScript.Instance.ShowRateEvery == 0 && InitScript.Instance.ShowRateEvery > 0 && PlayerPrefs.GetInt("Rated", 0) == 0)
                        InitScript.Instance.ShowRate();
                }
            }
            else if (value == GameState.Pause)
            {
                Time.timeScale = 0;

            }
            else if (value == GameState.Playing)
            {
                Time.timeScale = 1;
                StartCoroutine(AI.THIS.CheckPossibleCombines());
            }
            else if (value == GameState.GameOver)
            {
                GameObject.Find("CanvasGlobal").transform.Find("MenuFailed").gameObject.SetActive(true);
                OnLose();
            }
            else if (value == GameState.PreWinAnimations)
            {
                StartCoroutine(PreWinAnimationsCor());
            }
            else if (value == GameState.Win)
            {
                //InitScript.THIS.AddGems(1);//Add coins after win
                if (!InitScript.Instance.losingLifeEveryGame)
                    InitScript.Instance.AddLife(1);
                OnMenuComplete();
                if (PlayerPrefs.GetInt(string.Format("Level.{0:000}.StarsCount", currentLevel), 0) < stars)
                    PlayerPrefs.SetInt(string.Format("Level.{0:000}.StarsCount", currentLevel), stars);
                if (Score > PlayerPrefs.GetInt("Score" + currentLevel))
                {
                    PlayerPrefs.SetInt("Score" + currentLevel, Score);
                }

#if PLAYFAB 
                NetworkManager.dataManager.SetPlayerScore(currentLevel, Score);
                NetworkManager.dataManager.SetPlayerLevel(currentLevel + 1);
                NetworkManager.dataManager.SetStars();
#endif
                GameObject.Find("CanvasGlobal").transform.Find("MenuComplete").gameObject.SetActive(true);
                SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.complete[1]);
                if (OnWin != null) OnWin();
            }


        }
    }

    public void MenuPlayEvent()
    {
        if (OnMenuPlay != null) OnMenuPlay();
    }


    #endregion

    void LockBoosts()
    {
        foreach (BoostIcon item in inGameBoosts)
        {
            if (item != ActivatedBoost)
                item.LockBoost();
        }
    }

    public void UnLockBoosts()
    {
        foreach (BoostIcon item in inGameBoosts)
        {
            item.UnLockBoost();
        }
    }


    public void LoadLevel()
    {
        currentLevel = PlayerPrefs.GetInt("OpenLevel");
        if (currentLevel == 0)
            currentLevel = 1;
        LoadDataFromLocal(currentLevel);

    }

    public void EnableMap(bool enable)
    {
        float aspect = (float)Screen.height / (float)Screen.width;
        GetComponent<Camera>().orthographicSize = 5.3f;
        aspect = (float)Math.Round(aspect, 2);
        GameObject.Find("CanvasGlobal").GetComponent<GraphicRaycaster>().enabled = false;
        GameObject.Find("CanvasGlobal").GetComponent<GraphicRaycaster>().enabled = true;
        if (enable)
        {
            if (aspect == 1.6f)
                GetComponent<Camera>().orthographicSize = 6.25f;                    //16:10
            else if (aspect == 1.78f)
                GetComponent<Camera>().orthographicSize = 7f;    //16:9
            else if (aspect == 1.5f)
                GetComponent<Camera>().orthographicSize = 5.9f;                  //3:2
            else if (aspect == 1.33f)
                GetComponent<Camera>().orthographicSize = 5.25f;                  //4:3
            else if (aspect == 1.67f)
                GetComponent<Camera>().orthographicSize = 6.6f;                  //5:3
            else if (aspect == 1.25f)
                GetComponent<Camera>().orthographicSize = 4.9f;                  //5:4
            else if (aspect == 2.06f)
                GetComponent<Camera>().orthographicSize = 8.2f;                  //2960:1440
            else if (aspect == 2.17f)
                GetComponent<Camera>().orthographicSize = 8.7f;                  //iphone x

            GetComponent<Camera>().GetComponent<MapCamera>().SetPosition(new Vector2(0, GetComponent<Camera>().transform.position.y));
        }
        else
        {
            InitScript.DateOfExit = DateTime.Now.ToString();  //1.4
            
            GetComponent<Camera>().orthographicSize = 6.5f;
            level.transform.Find("Canvas/Panel").GetComponent<RectTransform>().anchoredPosition = Vector3.zero;//2.2
            if (aspect == 2.06f)
                GetComponent<Camera>().orthographicSize = 7.6f;                  //2960:1440
            else if (aspect == 2.17f)
            {
                GetComponent<Camera>().orthographicSize = 8.1f;                  //iphone x
                level.transform.Find("Canvas/Panel").GetComponent<RectTransform>().anchoredPosition = Vector3.down * 50;//2.2

            }

            level.transform.Find("Canvas").GetComponent<GraphicRaycaster>().enabled = false;
            level.transform.Find("Canvas").GetComponent<GraphicRaycaster>().enabled = true;

        }
        Camera.main.GetComponent<MapCamera>().enabled = enable;
        levelsMap.SetActive(!enable);
        levelsMap.SetActive(enable);
        level.SetActive(!enable);

        if (enable)
            gameField.gameObject.SetActive(false);

        if (!enable)
            Camera.main.transform.position = new Vector3(0, 0, -10);
        foreach (Transform item in gameField.transform)
        {
            Destroy(item.gameObject);
        }
    }

    // Use this for initialization
    void Start()
    {
#if UNITY_INAPPS

        gameObject.AddComponent<UnityInAppsIntegration>();
        enableInApps = true;//1.6.1
#else
        enableInApps = false;

#endif
        _combineManager = new CombineManager();
        Instance = this;
        Instance = this;
        //        if (!LevelManager.THIS.enableInApps)
        //            GameObject.Find("CanvasMap/Gems").gameObject.SetActive(false);//2.1.2

        GameStatus = GameState.Map;
        for (int i = 0; i < 20; i++)
        {
            //itemExplPool[i] = Instantiate(Resources.Load("Prefabs/Effects/ItemExpl"), transform.position, Quaternion.identity) as GameObject;
            //itemExplPool[i].GetComponent<SpriteRenderer>().enabled = false;

            // itemExplPool[i].SetActive(false);
        }
        passLevelCounter = 0;
    }

    void InitLevel()
    {

        GenerateLevel();
        GenerateOutline();
        ReGenLevel();
        if (limitType == LIMIT.TIME)
        {
            StopCoroutine(TimeTick());
            StartCoroutine(TimeTick());
        }
//        InitTargets();
        gameField.gameObject.SetActive(true);

    }

    void PrepareGame()
    {
        ActivatedBoost = null;
        Score = 0;
        stars = 0;
        moveID = 0;


        blocksObject.SetActive(false);
        ingrObject.SetActive(false);
        scoreTargetObject.SetActive(false);

        star1Anim.SetActive(false);
        star2Anim.SetActive(false);
        star3Anim.SetActive(false);

        collectItems[0] = CollectItems.None;
        collectItems[1] = CollectItems.None;

        ingrTarget[0] = Ingredients.None;
        ingrTarget[1] = Ingredients.None;

        ingrCountTarget[0] = 0;
        ingrCountTarget[1] = 0;

        TargetBlocks = 0;
        EnableMap(false);


        gameField.transform.position = Vector3.zero;
        firstSquarePosition = gameField.transform.position;

        squaresArray = new Square[maxCols * maxRows];
        LoadLevel();
        for (int row = 0; row < maxRows; row++)
        {
            for (int col = 0; col < maxCols; col++)
            {
                if (_levelSquaresFile[row * maxCols + col].Block == SquareTypes.BLOCK)
                    TargetBlocks++;
                else if (_levelSquaresFile[row * maxCols + col].Block == SquareTypes.DOUBLEBLOCK)
                    TargetBlocks += 2;
            }
        }
        //float getSize = maxCols - 9;
        //if (getSize < maxRows - 9)
        //    getSize = maxRows - 9;
        //if (getSize > 0)
        //    camera.orthographicSize = 6.5f + getSize * 0.5f;
        GameObject.Find("Canvas").transform.Find("PrePlay").gameObject.SetActive(true);
        if (limitType == LIMIT.MOVES)
        {
            inGameBoosts[0].gameObject.SetActive(true);
            inGameBoosts[1].gameObject.SetActive(false);
        }
        else
        {
            inGameBoosts[0].gameObject.SetActive(false);
            inGameBoosts[1].gameObject.SetActive(true);

        }
        OnEnterGame();
    }

    public void CheckCollectedTarget(GameObject _item)
    {
        _item.GetComponent<Item>();
        void AnimateCollectedItem(Vector2 pos)
        {
            GameObject item = new GameObject();
            item.transform.position = _item.transform.position;
            item.transform.localScale = Vector3.one / 2f;
            SpriteRenderer spr = item.AddComponent<SpriteRenderer>();
            var spriteRenderer = _item.GetComponentInChildren<SpriteRenderer>();
            spr.sprite = spriteRenderer.sprite;
            spr.sortingLayerName = "UI";
            spr.sortingOrder = 1;

            StartCoroutine(StartAnimateIngredient(item, pos));
        }

        foreach (var objectTarget in targetObject)
        {
            if(!objectTarget.Done())
            {
                if(objectTarget.SetOff(_item))     
                    AnimateCollectedItem(objectTarget.guiObj.transform.position);
            }
        }
    }

    public GameObject GetExplFromPool()
    {
        for (int i = 0; i < itemExplPool.Length; i++)
        {
            if (!itemExplPool[i].GetComponent<SpriteRenderer>().enabled)
            {
                // itemExplPool[i].SetActive(true);
                itemExplPool[i].GetComponent<SpriteRenderer>().enabled = true;
                StartCoroutine(HideDelayed(itemExplPool[i]));
                return itemExplPool[i];
            }

        }
        return null;
    }

    IEnumerator HideDelayed(GameObject gm)
    {
        yield return new WaitForSeconds(1f);
        gm.GetComponent<Animator>().SetTrigger("stop");
        gm.GetComponent<Animator>().SetInteger("color", 10);
        gm.GetComponent<SpriteRenderer>().enabled = false;
        //gm.SetActive(false);
    }

    IEnumerator StartAnimateIngredient(GameObject item, Vector2 pos)
    {
        _ingredientFly = true;
        GameObject[] ingr = new GameObject[2];
        ingr[0] = ingrObject.transform.Find("Ingr1").gameObject;
        ingr[1] = ingrObject.transform.Find("Ingr2").gameObject;
        if (targetBlocks > 0)
        {
            ingr[0] = blocksObject.transform.gameObject;
            ingr[1] = blocksObject.transform.gameObject;
        }
        AnimationCurve curveX = new AnimationCurve(new Keyframe(0, item.transform.localPosition.x), new Keyframe(0.4f, pos.x));
        AnimationCurve curveY = new AnimationCurve(new Keyframe(0, item.transform.localPosition.y), new Keyframe(0.5f, pos.y));
        curveY.AddKey(0.2f, item.transform.localPosition.y + UnityEngine.Random.Range(-2, 0.5f));
        float startTime = Time.time;
        Vector3 startPos = item.transform.localPosition;
        float speed = UnityEngine.Random.Range(0.4f, 0.6f);
        float distCovered = 0;
        while (distCovered < 0.5f)
        {
            distCovered = (Time.time - startTime) * speed;
            item.transform.localPosition = new Vector3(curveX.Evaluate(distCovered), curveY.Evaluate(distCovered), 0);
            item.transform.Rotate(Vector3.back, Time.deltaTime * 1000);
            yield return new WaitForFixedUpdate();
        }
        //     SoundBase.Instance.audio.PlayOneShot(SoundBase.Instance.getStarIngr);
        Destroy(item);
        if (GameStatus == GameState.Playing && !IsIngredientFalling())//1.6.1
            CheckWinLose();
        _ingredientFly = false;
    }

    public void CheckWinLose()
    {
        //		print ("check win lose");
        if (limit <= 0)
        {
            bool lose = false;
            limit = 0;
            if (targetObject.Any(i => !i.Done())) lose = true;
            if (lose)
                GameStatus = GameState.PreFailed;
            else if (Score >= Instance.star1 && targetObject.All(i => i.Done()))
            {
                GameStatus = GameState.PreWinAnimations;
            }
        }
        else
        {
            bool win = targetObject.All(i => i.Done());
            if (win)
            {
                    GameStatus = GameState.PreWinAnimations;
            }


        }
    }

    IEnumerator PreWinAnimationsCor()
    {

        GameObject.Find("Canvas").transform.Find("CompleteLabel").gameObject.SetActive(true);
        yield return new WaitForSeconds(1);

        List<Item> items = GetRandomItems(Mathf.Clamp(limitType == LIMIT.MOVES ? limit : 8, 0, 15)); //2.2.2
        foreach (Item item in items)
        {
            if (limitType == LIMIT.MOVES)
                limit--;
            item.NextType = (ItemsTypes)UnityEngine.Random.Range(1, 3);
            item.ChangeType();
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(0.3f);
        while (GetAllExtaItems().Count > 0 && GameStatus != GameState.Win)
        { //1.6
            Item item = GetAllExtaItems()[0];
            item.DestroyItem();
            _dragBlocked = true;
            yield return new WaitForSeconds(0.1f);
            FindMatches();
            yield return new WaitForSeconds(1f);

            //           GenerateNewItems();
            while (_dragBlocked)
                yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(1f);
        while (_dragBlocked || GetMatches().Count > 0)
            yield return new WaitForSeconds(0.2f);

        GameObject.Find("Canvas").transform.Find("CompleteLabel").gameObject.SetActive(false);
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.complete[0]);

        GameObject.Find("Canvas").transform.Find("PreCompleteBanner").gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        GameObject.Find("Canvas").transform.Find("PreCompleteBanner").gameObject.SetActive(false);


        GameStatus = GameState.Win;
    }

    void Update()
    {
        //  AvctivatedBoostView = ActivatedBoost;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            NoMatches();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            GameStatus = GameState.PreWinAnimations;
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            limit = 1;
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {   //save items state
            print("Saving items...");
            int[] items = new int[99];

            for (int row = 0; row < maxRows; row++)
            {
                for (int col = 0; col < maxCols; col++)
                {
                    if (GetSquare(col, row, false) != null)
                    {
                        if (GetSquare(col, row, false).item != null)
                            items[row * maxCols + col] = GetSquare(col, row, false).item.color;
                    }
                    else
                        items[row * maxCols + col] = -1;

                }
            }
            LevelDebugger.SaveMap(items, maxCols, maxRows);
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {   //load items state
            print("load items...");

            int[] items = new int[99];
            items = LevelDebugger.LoadMap(maxCols, maxRows);
            for (int row = 0; row < maxRows; row++)
            {
                for (int col = 0; col < maxCols; col++)
                {
                    if (items[row * maxCols + col] > -1)
                    {
                        if (GetSquare(col, row).item != null)
                            GetSquare(col, row).item.SetColor(items[row * maxCols + col]);
                    }
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (LevelManager.Instance.GameStatus == GameState.Playing)
                GameObject.Find("CanvasGlobal").transform.Find("MenuPause").gameObject.SetActive(true);
            else if (LevelManager.Instance.GameStatus == GameState.Map)
                Application.Quit();

        }


        if (LevelManager.Instance.GameStatus == GameState.Playing)
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnStartPlay();
                Collider2D hit = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                if (hit != null)
                {
                    Item item = hit.gameObject.GetComponent<Item>();
                    if (!LevelManager.Instance.DragBlocked && LevelManager.Instance.GameStatus == GameState.Playing)
                    {
                        if (LevelManager.Instance.ActivatedBoost.type == BoostType.Bomb && item.currentType != ItemsTypes.BOMB && item.currentType != ItemsTypes.INGREDIENT)
                        {
                            SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.boostBomb);
                            LevelManager.Instance.DragBlocked = true;
                            GameObject obj = Instantiate(Resources.Load("Prefabs/Effects/bomb"), item.transform.position, item.transform.rotation) as GameObject;
                            obj.GetComponent<SpriteRenderer>().sortingOrder = 4;
                            obj.GetComponent<BoostAnimation>().square = item.square;
                            LevelManager.Instance.ActivatedBoost = null;
                        }
                        else if (LevelManager.Instance.ActivatedBoost.type == BoostType.Random_color && item.currentType != ItemsTypes.BOMB)
                        {
                            SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.boostColorReplace);
                            LevelManager.Instance.DragBlocked = true;
                            GameObject obj = Instantiate(Resources.Load("Prefabs/Effects/random_color_item"), item.transform.position, item.transform.rotation) as GameObject;
                            obj.GetComponent<BoostAnimation>().square = item.square;
                            obj.GetComponent<SpriteRenderer>().sortingOrder = 4;
                            LevelManager.Instance.ActivatedBoost = null;
                        }
                        else if (item.square.type != SquareTypes.WIREBLOCK)
                        {
                            item.dragThis = true;
                            item.mousePos = item.GetMousePosition();
                            item.deltaPos = Vector3.zero;
                        }
                    }
                }

            }
            else if (Input.GetMouseButtonUp(0))
            {
                Collider2D hit = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                if (hit != null)
                {
                    Item item = hit.gameObject.GetComponent<Item>();
                    item.dragThis = false;
                    item.switchDirection = Vector3.zero;
                }

            }
        }
    }

    IEnumerator TimeTick()
    {
        while (true)
        {
            if (GameStatus == GameState.Playing)
            {
                if (LevelManager.Instance.limitType == LIMIT.TIME)
                {
                    LevelManager.Instance.limit--;
                    if (IsAllItemsFallDown())
                        CheckWinLose();
                }
            }
            if (GameStatus == GameState.Map)
                yield break;
            yield return new WaitForSeconds(1);
        }
    }

    private void GenerateLevel()
    {
        bool chessColor = false;
        Vector3 fieldPos = new Vector3(-maxCols / 2.75f, maxRows / 2.75f, -10);
        for (int row = 0; row < maxRows; row++)
        {
            if (maxCols % 2 == 0)
                chessColor = !chessColor;
            for (int col = 0; col < maxCols; col++)
            {
                CreateSquare(col, row, chessColor);
                chessColor = !chessColor;
            }

        }
        AnimateField(fieldPos);

    }

    void AnimateField(Vector3 pos)
    {

        float yOffset = 0;
        if (target == Target.INGREDIENT)
            yOffset = 0.3f;
        Animation anim = gameField.GetComponent<Animation>();
        AnimationClip clip = new AnimationClip();
        AnimationCurve curveX = new AnimationCurve(new Keyframe(0, pos.x + 15), new Keyframe(0.7f, pos.x - 0.2f), new Keyframe(0.8f, pos.x));
        AnimationCurve curveY = new AnimationCurve(new Keyframe(0, pos.y + yOffset), new Keyframe(1, pos.y + yOffset));
        clip.legacy = true;//2.1.6
        clip.SetCurve("", typeof(Transform), "localPosition.x", curveX);
        clip.SetCurve("", typeof(Transform), "localPosition.y", curveY);
        clip.AddEvent(new AnimationEvent() { time = 1, functionName = "EndAnimGamField" });
        anim.AddClip(clip, "appear");
        anim.Play("appear");
        gameField.transform.position = new Vector2(pos.x + 15, pos.y + yOffset);

    }

    void CreateSquare(int col, int row, bool chessColor = false)
    {
        GameObject square = null;
        square = Instantiate(squarePrefab, firstSquarePosition + new Vector2(col * squareWidth, -row * squareHeight), Quaternion.identity) as GameObject;
        if (chessColor)
        {
            square.GetComponent<SpriteRenderer>().sprite = squareSprite1;
        }
        square.transform.SetParent(gameField);
        square.transform.localPosition = firstSquarePosition + new Vector2(col * squareWidth, -row * squareHeight);
        squaresArray[row * maxCols + col] = square.GetComponent<Square>();
        square.GetComponent<Square>().row = row;
        square.GetComponent<Square>().col = col;
        square.GetComponent<Square>().type = SquareTypes.EMPTY;
        if (_levelSquaresFile[row * maxCols + col].Block == SquareTypes.EMPTY)
        {
            CreateObstacles(col, row, square, SquareTypes.NONE);
        }
        else if (_levelSquaresFile[row * maxCols + col].Block == SquareTypes.NONE)
        {
            square.GetComponent<SpriteRenderer>().enabled = false;
            square.GetComponent<Square>().type = SquareTypes.NONE;

        }
        else if (_levelSquaresFile[row * maxCols + col].Block == SquareTypes.BLOCK)
        {
            GameObject block = Instantiate(blockPrefab, firstSquarePosition + new Vector2(col * squareWidth, -row * squareHeight), Quaternion.identity) as GameObject;
            block.transform.SetParent(square.transform);
            block.transform.localPosition = new Vector3(0, 0, -0.01f);
            square.GetComponent<Square>().block.Add(block);
            square.GetComponent<Square>().type = SquareTypes.BLOCK;

            // TargetBlocks++;
            CreateObstacles(col, row, square, SquareTypes.NONE);
        }
        else if (_levelSquaresFile[row * maxCols + col].Block == SquareTypes.DOUBLEBLOCK)
        {
            GameObject block = Instantiate(blockPrefab, firstSquarePosition + new Vector2(col * squareWidth, -row * squareHeight), Quaternion.identity) as GameObject;
            block.transform.SetParent(square.transform);
            block.transform.localPosition = new Vector3(0, 0, -0.01f);
            square.GetComponent<Square>().block.Add(block);
            square.GetComponent<Square>().type = SquareTypes.BLOCK;

            //  TargetBlocks++;
            block = Instantiate(blockPrefab, firstSquarePosition + new Vector2(col * squareWidth, -row * squareHeight), Quaternion.identity) as GameObject;
            block.transform.SetParent(square.transform);
            block.transform.localPosition = new Vector3(0, 0, -0.01f);
            square.GetComponent<Square>().block.Add(block);
            square.GetComponent<Square>().type = SquareTypes.BLOCK;
            block.GetComponent<SpriteRenderer>().sprite = doubleBlock;
            block.GetComponent<SpriteRenderer>().sortingOrder = 1;
            //  TargetBlocks++;
            CreateObstacles(col, row, square, SquareTypes.NONE);
        }

    }

    void GenerateOutline()
    {
        int row = 0;
        int col = 0;
        for (row = 0; row < maxRows; row++)
        { //down
            SetOutline(col, row, 0);
        }
        row = maxRows - 1;
        for (col = 0; col < maxCols; col++)
        { //right
            SetOutline(col, row, 90);
        }
        col = maxCols - 1;
        for (row = maxRows - 1; row >= 0; row--)
        { //up
            SetOutline(col, row, 180);
        }
        row = 0;
        for (col = maxCols - 1; col >= 0; col--)
        { //left
            SetOutline(col, row, 270);
        }
        col = 0;
        for (row = 1; row < maxRows - 1; row++)
        {
            for (col = 1; col < maxCols - 1; col++)
            {
                //  if (GetSquare(col, row).type == SquareTypes.NONE)
                SetOutline(col, row, 0);
            }
        }
    }


    void SetOutline(int col, int row, float zRot)
    {
        Square square = GetSquare(col, row, true);
        if (square.type != SquareTypes.NONE)
        {
            if (row == 0 || col == 0 || col == maxCols - 1 || row == maxRows - 1)
            {
                GameObject outline = CreateOutline(square);
                SpriteRenderer spr = outline.GetComponent<SpriteRenderer>();
                outline.transform.localRotation = Quaternion.Euler(0, 0, zRot);
                if (zRot == 0)
                    outline.transform.localPosition = Vector3.zero + Vector3.left * 0.425f;
                if (zRot == 90)
                    outline.transform.localPosition = Vector3.zero + Vector3.down * 0.425f;
                if (zRot == 180)
                    outline.transform.localPosition = Vector3.zero + Vector3.right * 0.425f;
                if (zRot == 270)
                    outline.transform.localPosition = Vector3.zero + Vector3.up * 0.425f;
                if (row == 0 && col == 0)
                {   //top left
                    spr.sprite = outline3;
                    outline.transform.localRotation = Quaternion.Euler(0, 0, 180);
                    outline.transform.localPosition = Vector3.zero + Vector3.left * 0.015f + Vector3.up * 0.015f;
                }
                if (row == 0 && col == maxCols - 1)
                {   //top right
                    spr.sprite = outline3;
                    outline.transform.localRotation = Quaternion.Euler(0, 0, 90);
                    outline.transform.localPosition = Vector3.zero + Vector3.right * 0.015f + Vector3.up * 0.015f;
                }
                if (row == maxRows - 1 && col == 0)
                {   //bottom left
                    spr.sprite = outline3;
                    outline.transform.localRotation = Quaternion.Euler(0, 0, -90);
                    outline.transform.localPosition = Vector3.zero + Vector3.left * 0.015f + Vector3.down * 0.015f;
                }
                if (row == maxRows - 1 && col == maxCols - 1)
                {   //bottom right
                    spr.sprite = outline3;
                    outline.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    outline.transform.localPosition = Vector3.zero + Vector3.right * 0.015f + Vector3.down * 0.015f;
                }
            }
            else
            {
                //top left
                if (GetSquare(col - 1, row - 1, true).type == SquareTypes.NONE && GetSquare(col, row - 1, true).type == SquareTypes.NONE && GetSquare(col - 1, row, true).type == SquareTypes.NONE)
                {
                    GameObject outline = CreateOutline(square);
                    SpriteRenderer spr = outline.GetComponent<SpriteRenderer>();
                    spr.sprite = outline3;
                    outline.transform.localPosition = Vector3.zero + Vector3.left * 0.015f + Vector3.up * 0.015f;
                    outline.transform.localRotation = Quaternion.Euler(0, 0, 180);
                }
                //top right
                if (GetSquare(col + 1, row - 1, true).type == SquareTypes.NONE && GetSquare(col, row - 1, true).type == SquareTypes.NONE && GetSquare(col + 1, row, true).type == SquareTypes.NONE)
                {
                    GameObject outline = CreateOutline(square);
                    SpriteRenderer spr = outline.GetComponent<SpriteRenderer>();
                    spr.sprite = outline3;
                    outline.transform.localPosition = Vector3.zero + Vector3.right * 0.015f + Vector3.up * 0.015f;
                    outline.transform.localRotation = Quaternion.Euler(0, 0, 90);
                }
                //bottom left
                if (GetSquare(col - 1, row + 1, true).type == SquareTypes.NONE && GetSquare(col, row + 1, true).type == SquareTypes.NONE && GetSquare(col - 1, row, true).type == SquareTypes.NONE)
                {
                    GameObject outline = CreateOutline(square);
                    SpriteRenderer spr = outline.GetComponent<SpriteRenderer>();
                    spr.sprite = outline3;
                    outline.transform.localPosition = Vector3.zero + Vector3.left * 0.015f + Vector3.down * 0.015f;
                    outline.transform.localRotation = Quaternion.Euler(0, 0, 270);
                }
                //bottom right
                if (GetSquare(col + 1, row + 1, true).type == SquareTypes.NONE && GetSquare(col, row + 1, true).type == SquareTypes.NONE && GetSquare(col + 1, row, true).type == SquareTypes.NONE)
                {
                    GameObject outline = CreateOutline(square);
                    SpriteRenderer spr = outline.GetComponent<SpriteRenderer>();
                    spr.sprite = outline3;
                    outline.transform.localPosition = Vector3.zero + Vector3.right * 0.015f + Vector3.down * 0.015f;
                    outline.transform.localRotation = Quaternion.Euler(0, 0, 0);
                }


            }
        }
        else
        {
            bool corner = false;
            if (GetSquare(col - 1, row, true).type != SquareTypes.NONE && GetSquare(col, row - 1, true).type != SquareTypes.NONE)
            {
                GameObject outline = CreateOutline(square);
                SpriteRenderer spr = outline.GetComponent<SpriteRenderer>();
                spr.sprite = outline2;
                outline.transform.localPosition = Vector3.zero;
                outline.transform.localRotation = Quaternion.Euler(0, 0, 0);
                corner = true;
            }
            if (GetSquare(col + 1, row, true).type != SquareTypes.NONE && GetSquare(col, row + 1, true).type != SquareTypes.NONE)
            {
                GameObject outline = CreateOutline(square);
                SpriteRenderer spr = outline.GetComponent<SpriteRenderer>();
                spr.sprite = outline2;
                outline.transform.localPosition = Vector3.zero;
                outline.transform.localRotation = Quaternion.Euler(0, 0, 180);
                corner = true;
            }
            if (GetSquare(col + 1, row, true).type != SquareTypes.NONE && GetSquare(col, row - 1, true).type != SquareTypes.NONE)
            {
                GameObject outline = CreateOutline(square);
                SpriteRenderer spr = outline.GetComponent<SpriteRenderer>();
                spr.sprite = outline2;
                outline.transform.localPosition = Vector3.zero;
                outline.transform.localRotation = Quaternion.Euler(0, 0, 270);
                corner = true;
            }
            if (GetSquare(col - 1, row, true).type != SquareTypes.NONE && GetSquare(col, row + 1, true).type != SquareTypes.NONE)
            {
                GameObject outline = CreateOutline(square);
                SpriteRenderer spr = outline.GetComponent<SpriteRenderer>();
                spr.sprite = outline2;
                outline.transform.localPosition = Vector3.zero;
                outline.transform.localRotation = Quaternion.Euler(0, 0, 90);
                corner = true;
            }


            if (!corner)
            {
                if (GetSquare(col, row - 1, true).type != SquareTypes.NONE)
                {
                    GameObject outline = CreateOutline(square);
                    SpriteRenderer spr = outline.GetComponent<SpriteRenderer>();
                    outline.transform.localPosition = Vector3.zero + Vector3.up * 0.395f;
                    outline.transform.localRotation = Quaternion.Euler(0, 0, 90);
                }
                if (GetSquare(col, row + 1, true).type != SquareTypes.NONE)
                {
                    GameObject outline = CreateOutline(square);
                    SpriteRenderer spr = outline.GetComponent<SpriteRenderer>();
                    outline.transform.localPosition = Vector3.zero + Vector3.down * 0.395f;
                    outline.transform.localRotation = Quaternion.Euler(0, 0, 90);
                }
                if (GetSquare(col - 1, row, true).type != SquareTypes.NONE)
                {
                    GameObject outline = CreateOutline(square);
                    SpriteRenderer spr = outline.GetComponent<SpriteRenderer>();
                    outline.transform.localPosition = Vector3.zero + Vector3.left * 0.395f;
                    outline.transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
                if (GetSquare(col + 1, row, true).type != SquareTypes.NONE)
                {
                    GameObject outline = CreateOutline(square);
                    SpriteRenderer spr = outline.GetComponent<SpriteRenderer>();
                    outline.transform.localPosition = Vector3.zero + Vector3.right * 0.395f;
                    outline.transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
            }
        }
        //Vector3 pos = GameField.transform.TransformPoint((Vector3)firstSquarePosition + new Vector3(col * squareWidth - squareWidth / 2, -row * squareHeight, 10));
        //line.SetVertexCount(linePoint + 1);
        //line.SetPosition(linePoint++, pos);

    }

    GameObject CreateOutline(Square square)
    {
        GameObject outline = new GameObject();
        outline.name = "outline";
        outline.transform.SetParent(square.transform);
        outline.transform.localPosition = Vector3.zero;
        SpriteRenderer spr = outline.AddComponent<SpriteRenderer>();
        spr.sprite = outline1;
        spr.sortingOrder = 1;
        return outline;
    }

    void CreateObstacles(int col, int row, GameObject square, SquareTypes type)
    {
        if ((_levelSquaresFile[row * maxCols + col].Obstacle == SquareTypes.WIREBLOCK && type == SquareTypes.NONE) || type == SquareTypes.WIREBLOCK)
        {
            GameObject block = Instantiate(wireBlockPrefab, firstSquarePosition + new Vector2(col * squareWidth, -row * squareHeight), Quaternion.identity) as GameObject;
            block.transform.SetParent(square.transform);
            block.transform.localPosition = new Vector3(0, 0, -0.5f);
            square.GetComponent<Square>().block.Add(block);
            square.GetComponent<Square>().type = SquareTypes.WIREBLOCK;
            block.GetComponent<SpriteRenderer>().sortingOrder = 3;
            //   TargetBlocks++;
        }
        else if ((_levelSquaresFile[row * maxCols + col].Obstacle == SquareTypes.SOLIDBLOCK && type == SquareTypes.NONE) || type == SquareTypes.SOLIDBLOCK)
        {
            GameObject block = Instantiate(solidBlockPrefab, firstSquarePosition + new Vector2(col * squareWidth, -row * squareHeight), Quaternion.identity) as GameObject;
            block.transform.SetParent(square.transform);
            block.transform.localPosition = new Vector3(0, 0, -0.5f);
            square.GetComponent<Square>().block.Add(block);
            block.GetComponent<SpriteRenderer>().sortingOrder = 3;
            square.GetComponent<Square>().type = SquareTypes.SOLIDBLOCK;

            //  TargetBlocks++;
        }
        else if ((_levelSquaresFile[row * maxCols + col].Obstacle == SquareTypes.UNDESTROYABLE && type == SquareTypes.NONE) || type == SquareTypes.UNDESTROYABLE)
        {
            GameObject block = Instantiate(undestroyableBlockPrefab, firstSquarePosition + new Vector2(col * squareWidth, -row * squareHeight), Quaternion.identity) as GameObject;
            block.transform.SetParent(square.transform);
            block.transform.localPosition = new Vector3(0, 0, -0.5f);
            square.GetComponent<Square>().block.Add(block);
            square.GetComponent<Square>().type = SquareTypes.UNDESTROYABLE;

            //  TargetBlocks++;
        }
        else if ((_levelSquaresFile[row * maxCols + col].Obstacle == SquareTypes.THRIVING && type == SquareTypes.NONE) || type == SquareTypes.THRIVING)
        {
            GameObject block = Instantiate(thrivingBlockPrefab, firstSquarePosition + new Vector2(col * squareWidth, -row * squareHeight), Quaternion.identity) as GameObject;
            block.transform.SetParent(square.transform);
            block.transform.localPosition = new Vector3(0, 0, -0.5f);
            block.GetComponent<SpriteRenderer>().sortingOrder = 3;
            if (square.GetComponent<Square>().item != null)
                Destroy(square.GetComponent<Square>().item.gameObject);
            square.GetComponent<Square>().block.Add(block);
            square.GetComponent<Square>().type = SquareTypes.THRIVING;

            //   TargetBlocks++;
        }

    }

    void GenerateNewItems(bool falling = true)
    {
        for (int col = 0; col < maxCols; col++)
        {
            for (int row = maxRows - 1; row >= 0; row--)
            {
                if (GetSquare(col, row) != null)
                {
                    if (!GetSquare(col, row).IsNone() && GetSquare(col, row).CanGoInto() && GetSquare(col, row).item == null)
                    {
                        if ((GetSquare(col, row).item == null && !GetSquare(col, row).IsHaveSolidAbove()) || !falling)
                        {
                            GetSquare(col, row).GenItem(falling);
                        }
                    }
                }
            }
        }

    }

    public void NoMatches()
    {
        StartCoroutine(NoMatchesCor());
    }

    IEnumerator NoMatchesCor()
    {
        if (GameStatus == GameState.Playing)
        {
            SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.noMatch);

            GameObject.Find("Canvas").transform.Find("NoMoreMatches").gameObject.SetActive(true);
            GameStatus = GameState.RegenLevel;
            yield return new WaitForSeconds(1);
            ReGenLevel();
        }
    }

    public void ReGenLevel()
    {
        itemsHided = false;
        DragBlocked = true;
        if (GameStatus != GameState.Playing && GameStatus != GameState.RegenLevel)
            DestroyItems();
        else if (GameStatus == GameState.RegenLevel)
            DestroyItems(true);
        if (OnLevelLoaded != null)
        {
            OnLevelLoaded();
            StartCoroutine(RegenMatches());
            OnLevelLoaded();
        }
    }

    IEnumerator RegenMatches(bool onlyFalling = false)
    {
        if (GameStatus == GameState.RegenLevel)
        {
            //while (!itemsHided)
            //{
            yield return new WaitForSeconds(0.5f);
            //}
        }
        if (!onlyFalling)
            GenerateNewItems(false);
        else
            LevelManager.Instance.onlyFalling = true;
        //   yield return new WaitForSeconds(1f);
        yield return new WaitForFixedUpdate();

        List<List<Item>> combs = GetMatches();
        //while (!matchesGot)
        //{
        //    yield return new WaitForFixedUpdate();

        //}
        //combs = newCombines;
        //matchesGot = false;
        do
        {
            foreach (List<Item> comb in combs)
            {
                int colorOffset = 0;
                foreach (Item item in comb)
                {
                    item.GenColor(item.color + colorOffset);
                    colorOffset++;
                }
            }
            combs = GetMatches();
            //while (!matchesGot)
            //{
            //    yield return new WaitForFixedUpdate();

            //}
            //combs = newCombines;
            //matchesGot = false;

            //     yield return new WaitForFixedUpdate();
        } while (combs.Count > 0);
        yield return new WaitForFixedUpdate();
        SetPreBoosts();
        LevelManager.Instance.onlyFalling = false;
        if (GameStatus == GameState.RegenLevel)
            GameStatus = GameState.Playing;
        if (!onlyFalling)//2.1.5 prevents early move
            DragBlocked = false;
        StartCoroutine(AI.THIS.CheckPossibleCombines());//2.1.5 prevents early move

        //StartCoroutine(CheckFallingAtStart());

    }

    void SetPreBoosts()
    {
        if (boostPackage > 0)
        {
            InitScript.Instance.SpendBoost(BoostType.Packages);
            foreach (Item item in GetRandomItems(boostPackage))
            {
                item.NextType = ItemsTypes.PACKAGE;
                item.ChangeType();
                item.boost = true;
            }
            boostPackage = 0;
        }
        if (boostColorfullBomb > 0)
        {
            InitScript.Instance.SpendBoost(BoostType.Colorful_bomb);
            foreach (Item item in GetRandomItems(boostColorfullBomb))
            {
                item.NextType = ItemsTypes.BOMB;
                item.ChangeType();
                item.boost = true;
            }
            boostColorfullBomb = 0;
        }
        if (boostStriped > 0)
        {
            InitScript.Instance.SpendBoost(BoostType.Stripes);
            foreach (Item item in GetRandomItems(boostStriped))
            {
                item.NextType = (ItemsTypes)UnityEngine.Random.Range(1, 3);
                item.ChangeType();
                item.boost = true;
            }
            boostStriped = 0;
        }
    }


    public List<Item> GetRandomItems(int count)
    {
        List<Item> list = new List<Item>();
        List<Item> list2 = new List<Item>();
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
        if (items.Length < count)
            count = items.Length;
        foreach (GameObject item in items)
        {
            if (item.GetComponent<Item>().currentType == ItemsTypes.NONE && item.GetComponent<Item>().NextType == ItemsTypes.NONE && !item.GetComponent<Item>().destroying)
            {
                list.Add(item.GetComponent<Item>());
            }
        }

        while (list2.Count < count)
        {

            try
            {
                Item newItem = list[UnityEngine.Random.Range(0, list.Count)];
                if (list2.IndexOf(newItem) < 0)
                {
                    list2.Add(newItem);
                }
            }
            catch (Exception ex)
            {
                GameStatus = GameState.Win;
            }
        }
        return list2;
    }

    List<Item> GetAllExtaItems()
    {
        List<Item> list = new List<Item>();
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
        foreach (GameObject item in items)
        {
            if (item.GetComponent<Item>().currentType != ItemsTypes.NONE)
            {
                list.Add(item.GetComponent<Item>());
            }
        }

        return list;
    }

    public int GetIngredientsCount(string spritename)
    {
        return FindObjectsOfType<SpriteRenderer>().Count(i => i.sprite.name==spritename);
    }


    public void DestroyItems(bool withoutEffects = false)
    {

        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
        foreach (GameObject item in items)
        {
            if (item != null)
            {
                if (item.GetComponent<Item>().currentType != ItemsTypes.INGREDIENT && item.GetComponent<Item>().currentType == ItemsTypes.NONE)
                {
                    if (!withoutEffects)
                        item.GetComponent<Item>().DestroyItem();
                    else
                        item.GetComponent<Item>().SmoothDestroy();
                }
            }
        }

    }

    public IEnumerator FindMatchDelay()
    {
        yield return new WaitForSeconds(0.2f);
        LevelManager.Instance.FindMatches();

    }

    public void FindMatches()
    {
        StartCoroutine(FallingDown());
    }

    public List<List<Item>> GetMatches(FindSeparating separating = FindSeparating.NONE, int matches = 3)
    {
        _newCombines = new List<List<Item>>();
        //       List<Item> countedSquares = new List<Item>();
        countedSquares = new Hashtable();
        countedSquares.Clear();
        for (int col = 0; col < maxCols; col++)
        {
            for (int row = 0; row < maxRows; row++)
            {
                if (GetSquare(col, row) != null)
                {
                    if (!countedSquares.ContainsValue(GetSquare(col, row).item))
                    {
                        List<Item> newCombine = GetSquare(col, row).FindMatchesAround(separating, matches, countedSquares);
                        if (newCombine.Count >= matches)
                            _newCombines.Add(newCombine);
                    }
                }
            }
        }
        //print("global " + countedSquares.Count);
        //  Debug.Break();
        return _newCombines;
    }
    IEnumerator FallingDown()
    {
        bool nearEmptySquareDetected = false;
        int combo = 0;
        AI.THIS.allowShowTip = false;
        List<Item> it = GetItems();
        for (int i = 0; i < it.Count; i++)
        {
            Item item = it[i];
            if (item != null)
            {
                item.anim.StopPlayback();
            }
        }

        while (true)
        {

            //find matches
            yield return new WaitForSeconds(0.1f);

            _combinedItems.Clear();
            _combinedItems = _combineManager.GetCombine();

            if (_combinedItems.Count > 0)
                combo++;
            foreach (List<Item> desrtoyItems in _combinedItems)
            {
                foreach (Item item in desrtoyItems)
                {
                    if (item.currentType != ItemsTypes.NONE)
                        yield return new WaitForSeconds(0.1f);
                    item.DestroyItem(true);
                }
            }

            foreach (Item item in destroyAnyway)
            {
                item.DestroyItem(true, "", true);
            }
            destroyAnyway.Clear();

            if (lastDraggedItem != null)
            {
                if (lastDraggedItem.NextType != ItemsTypes.NONE)
                {
                    yield return new WaitForSeconds(0.5f);

                }
                lastDraggedItem = null;
            }

            while (!IsAllDestoyFinished())
            {
                yield return new WaitForSeconds(0.1f);
            }
            for (int i = 0; i < 20; i++)
            {
                for (int col = 0; col < maxCols; col++)
                {
                    for (int row = maxRows - 1; row >= 0; row--)
                    {
                        if (GetSquare(col, row) != null)
                            GetSquare(col, row).FallOut();
                    }
                }
            }
            if (!nearEmptySquareDetected)
                yield return new WaitForSeconds(0.2f);

            CheckIngredient();
            for (int col = 0; col < maxCols; col++)
            {
                for (int row = maxRows - 1; row >= 0; row--)
                {
                    if (GetSquare(col, row) != null)
                    {
                        if (!GetSquare(col, row).IsNone())
                        {
                            if (GetSquare(col, row).item != null)
                            {
                                GetSquare(col, row).item.StartFalling();
                            }
                        }
                    }
                }
            }
            yield return new WaitForSeconds(0.2f);
            GenerateNewItems();
            StartCoroutine(RegenMatches(true));
            yield return new WaitForSeconds(0.1f);
            while (!IsAllItemsFallDown())
            {
                yield return new WaitForSeconds(0.1f);
            }
            nearEmptySquareDetected = false;

            for (int col = 0; col < maxCols; col++)
            {
                for (int row = maxRows - 1; row >= 0; row--)
                {
                    if (GetSquare(col, row) != null)
                    {
                        if (!GetSquare(col, row).IsNone())
                        {
                            if (GetSquare(col, row).item != null)
                            {
                                if (GetSquare(col, row).item.GetNearEmptySquares())
                                    nearEmptySquareDetected = true;

                            }
                        }
                    }
                }
            }
            while (!IsAllItemsFallDown())
            {
                yield return new WaitForSeconds(0.1f);
            }

            if (destroyAnyway.Count > 0)
                nearEmptySquareDetected = true;
            if (GetMatches().Count <= 0 && !nearEmptySquareDetected)
                break;
        }

        List<Item> item_ = GetItems();
        for (int i = 0; i < item_.Count; i++)
        {
            Item item1 = item_[i];
            if (item1 != null)
            {
                if (item1 != item1.square.item)
                {
                    Destroy(item1.gameObject);
                }
            }
        }
        if (!thrivingBlockDestroyed)
        {
            bool thrivingBlockSelected = false;
            for (int col = 0; col < maxCols; col++)
            {
                if (thrivingBlockSelected)
                    break;
                for (int row = maxRows - 1; row >= 0; row--)
                {
                    if (thrivingBlockSelected)
                        break;
                    if (GetSquare(col, row) != null)
                    {
                        if (GetSquare(col, row).type == SquareTypes.THRIVING)
                        {
                            List<Square> sqList = GetSquare(col, row).GetAllNeghbors();
                            foreach (Square sq in sqList)
                            {
                                if (sq.CanGoInto() && UnityEngine.Random.Range(0, 1) == 0 && sq.type == SquareTypes.EMPTY)
                                {
                                    if (sq.item != null)
                                    {
                                        if (sq.item.currentType == ItemsTypes.NONE)
                                        {
                                            CreateObstacles(sq.col, sq.row, sq.gameObject, SquareTypes.THRIVING);

                                            thrivingBlockSelected = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        thrivingBlockDestroyed = false;

        if (GameStatus == GameState.Playing && !_ingredientFly)
            LevelManager.Instance.CheckWinLose();

        if (combo > 2 && GameStatus == GameState.Playing)
        {
            gratzWords[UnityEngine.Random.Range(0, gratzWords.Length)].SetActive(true);
            combo = 0;
        }
        CheckItemsPositions();
        DragBlocked = false;
        if (GameStatus == GameState.Playing)
            StartCoroutine(AI.THIS.CheckPossibleCombines());




    }

    void CheckItemsPositions()
    {
        List<Item> items = GetItems();
        foreach (var item in items)
        {
            if (item)
                item.transform.position = item.square.transform.position + Vector3.back * 0.2f;
        }
    }


    public void DestroyDoubleBomb(int col)
    {
        StartCoroutine(DestroyDoubleBombCor(col));
        StartCoroutine(DestroyDoubleBombCorBack(col));
    }

    IEnumerator DestroyDoubleBombCor(int col)
    {
        for (int i = col; i < maxCols; i++)
        {
            List<Item> list = GetColumn(i);
            foreach (Item item in list)
            {
                if (item != null)
                    item.DestroyItem(true, "", true);
            }
            yield return new WaitForSeconds(0.3f);
        }
        if (col <= maxCols - col - 1)
            FindMatches();
    }

    IEnumerator DestroyDoubleBombCorBack(int col)
    {
        for (int i = col - 1; i >= 0; i--)
        {
            List<Item> list = GetColumn(i);
            foreach (Item item in list)
            {
                if (item != null)
                    item.DestroyItem(true, "", true);
            }
            yield return new WaitForSeconds(0.3f);
        }
        if (col > maxCols - col - 1)
            FindMatches();
    }


    public Square GetSquare(int col, int row, bool safe = false)
    {
        if (!safe)
        {
            if (row >= maxRows || col >= maxCols)
                return null;
            return squaresArray[row * maxCols + col];
        }
        else
        {
            row = Mathf.Clamp(row, 0, maxRows - 1);
            col = Mathf.Clamp(col, 0, maxCols - 1);
            return squaresArray[row * maxCols + col];
        }
    }

    bool IsAllDestoyFinished()
    {
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
        foreach (GameObject item in items)
        {
            Item itemComponent = item.GetComponent<Item>();
            if (itemComponent == null)
            {
                return false;
            }
            if (itemComponent.destroying && !itemComponent.animationFinished)
                return false;
        }
        return true;
    }

    bool IsIngredientFalling()
    {
        if (GameStatus == GameState.PreWinAnimations)
            return true;
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
        foreach (GameObject item in items)
        {
            Item itemComponent = item.GetComponent<Item>();
            if (itemComponent != null)
            {
                if (itemComponent.falling && itemComponent.currentType == ItemsTypes.INGREDIENT)
                    return true;
            }
        }
        return false;

    }

    bool IsAllItemsFallDown()
    {
        if (GameStatus == GameState.PreWinAnimations)
            return true;
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
        foreach (GameObject item in items)
        {
            Item itemComponent = item.GetComponent<Item>();
            if (itemComponent == null)
            {
                return false;
            }
            if (itemComponent.falling)
                return false;
        }
        return true;
    }

    public Vector2 GetPosition(Item item)
    {
        return GetPosition(item.square);
    }

    private Vector2 GetPosition(Square square)
    {
        return new Vector2(square.col, square.row);
    }

    public List<Item> GetRow(int row)
    {
        List<Item> itemsList = new List<Item>();
        for (int col = 0; col < maxCols; col++)
        {
            itemsList.Add(GetSquare(col, row, true).item);
        }
        return itemsList;
    }

    public List<Item> GetColumn(int col)
    {
        List<Item> itemsList = new List<Item>();
        for (int row = 0; row < maxRows; row++)
        {
            itemsList.Add(GetSquare(col, row, true).item);
        }
        return itemsList;
    }

    public List<Square> GetColumnSquaresObstacles(int col)
    {
        List<Square> itemsList = new List<Square>();
        for (int row = 0; row < maxRows; row++)
        {
            if (GetSquare(col, row, true).IsHaveDestroybleObstacle())
                itemsList.Add(GetSquare(col, row, true));
        }
        return itemsList;
    }

    public List<Square> GetRowSquaresObstacles(int row)
    {
        List<Square> itemsList = new List<Square>();
        for (int col = 0; col < maxCols; col++)
        {
            if (GetSquare(col, row, true).IsHaveDestroybleObstacle())
                itemsList.Add(GetSquare(col, row, true));
        }
        return itemsList;
    }

    public List<Item> GetItemsAround(Square square)
    {
        int col = square.col;
        int row = square.row;
        List<Item> itemsList = new List<Item>();
        for (int r = row - 1; r <= row + 1; r++)
        {
            for (int c = col - 1; c <= col + 1; c++)
            {
                itemsList.Add(GetSquare(c, r, true).item);
            }
        }
        return itemsList;
    }
    
    public List<Item> GetItems()
    {
        List<Item> itemsList = new List<Item>();
        for (int row = 0; row < maxRows; row++)
        {
            for (int col = 0; col < maxCols; col++)
            {
                if (GetSquare(col, row) != null)
                    itemsList.Add(GetSquare(col, row, true).item);
            }
        }
        return itemsList;
    }

    public void SetTypeByColor(int p, ItemsTypes nextType)
    {
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
        foreach (GameObject item in items)
        {
            if (item.GetComponent<Item>().color == p)
            {
                if (nextType == ItemsTypes.HORIZONTAL_STRIPPED || nextType == ItemsTypes.VERTICAL_STRIPPED)
                    item.GetComponent<Item>().NextType = (ItemsTypes)UnityEngine.Random.Range(1, 3);
                else
                    item.GetComponent<Item>().NextType = nextType;

                item.GetComponent<Item>().ChangeType();
                if (nextType == ItemsTypes.NONE)
                    destroyAnyway.Add(item.GetComponent<Item>());
            }
        }
    }

    private void CheckIngredient()
    {
        int row = maxRows;
        List<Square> sqList = GetBottomRow();
        foreach (Square sq in sqList)
        {
            if (sq.item != null)
            {
                if (sq.item.currentType == ItemsTypes.INGREDIENT)
                {
                    destroyAnyway.Add(sq.item);
                }
            }
        }
    }

    private List<Square> GetBottomRow()
    {
        List<Square> itemsList = new List<Square>();
        int listCounter = 0;
        for (int col = 0; col < maxCols; col++)
        {
            for (int row = maxRows - 1; row >= 0; row--)
            {
                Square square = GetSquare(col, row, true);
                if (square.type != SquareTypes.NONE)
                {
                    itemsList.Add(square);
                    listCounter++;
                    break;
                }
            }
        }
        return itemsList;
    }

    private IEnumerator StartIdleCor()
    {
        for (int col = 0; col < maxCols; col++)
        {
            for (int row = 0; row < maxRows; row++)
            {
                if (GetSquare(col, row, true).item != null)
                    GetSquare(col, row, true).item.StartIdleAnim();
            }

        }

        yield return new WaitForFixedUpdate();
    }

    public void StrippedShow(GameObject obj, bool horizontal)
    {
        GameObject effect = Instantiate(stripesEffect, obj.transform.position, Quaternion.identity) as GameObject;
        if (!horizontal)
            effect.transform.Rotate(Vector3.back, 90);
        Destroy(effect, 1);
    }

    public void PopupScore(int value, Vector3 pos, int color)
    {
        Score += value;
        if( targetObject.Any(i => i.type == Target.SCORE))
            targetObject.First(i => i.type == Target.SCORE).SetOff(null, value);
        UpdateBar();
        CheckStars();
        if (showPopupScores)
        {
            Transform parent = GameObject.Find("CanvasScore").transform;
            GameObject poptxt = Instantiate(popupScore, pos, Quaternion.identity) as GameObject;
            poptxt.transform.GetComponentInChildren<Text>().text = "" + value;
            if (color <= scoresColors.Length - 1)
            {
                poptxt.transform.GetComponentInChildren<Text>().color = scoresColors[Mathf.Clamp(color, 0, scoresColors.Length)];
                poptxt.transform.GetComponentInChildren<Outline>().effectColor = scoresColorsOutline[Mathf.Clamp(color, 0, scoresColorsOutline.Length)];
            }
            poptxt.transform.SetParent(parent);
            poptxt.transform.position = pos;
            poptxt.transform.localScale = Vector3.one / 1.5f;
            Destroy(poptxt, 0.3f);
        }
    }

    private void UpdateBar()
    {
        ProgressBarScript.Instance.UpdateDisplay((float)Score * 100f / ((float)star1 / ((star1 * 100f / star3)) * 100f) / 100f);
    }

    private void CheckStars()
    {
        if (Score >= star1 && stars <= 0)
        {
            stars = 1;
        }
        if (Score >= star2 && stars <= 1)
        {
            stars = 2;
        }
        if (Score >= star3 && stars <= 2)
        {
            stars = 3;
        }

        if (Score >= star1)
        {
            if (!star1Anim.activeSelf)
                SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.getStarIngr);
            star1Anim.SetActive(true);
        }
        if (Score >= star2)
        {
            if (!star2Anim.activeSelf)
                SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.getStarIngr);
            star2Anim.SetActive(true);
        }
        if (Score >= star3)
        {
            if (!star3Anim.activeSelf)
                SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.getStarIngr);
            star3Anim.SetActive(true);
        }
    }

    private void LoadDataFromLocal(int currentLevel)
    {
        levelLoaded = false;
        TextAsset mapText = Resources.Load("Levels/" + currentLevel) as TextAsset;
        if (mapText == null)
        {
            mapText = Resources.Load("Levels/" + currentLevel) as TextAsset;
        }
        ProcessGameDataFromString(mapText.text);
    }

    void ProcessGameDataFromString(string mapText)
    {
        string[] lines = mapText.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

        int mapLine = 0;
        foreach (string line in lines)
        {
            if (line.StartsWith("MODE"))
            {
                string modeString = line.Replace("MODE", string.Empty).Trim();
                target = (Target)int.Parse(modeString);
            }
            else if (line.StartsWith("SIZE "))
            {
                string blocksString = line.Replace("SIZE", string.Empty).Trim();
                string[] sizes = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                maxCols = int.Parse(sizes[0]);
                maxRows = int.Parse(sizes[1]);
                squaresArray = new Square[maxCols * maxRows];
                _levelSquaresFile = new SquareBlocks[maxRows * maxCols];
                for (int i = 0; i < _levelSquaresFile.Length; i++)
                {

                    SquareBlocks sqBlocks = new SquareBlocks();
                    sqBlocks.Block = SquareTypes.EMPTY;
                    sqBlocks.Obstacle = SquareTypes.NONE;

                    _levelSquaresFile[i] = sqBlocks;
                }
            }
            else if (line.StartsWith("LIMIT"))
            {
                string blocksString = line.Replace("LIMIT", string.Empty).Trim();
                string[] sizes = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                limitType = (LIMIT)int.Parse(sizes[0]);
                limit = int.Parse(sizes[1]);
            }
            else if (line.StartsWith("COLOR LIMIT "))
            {
                string blocksString = line.Replace("COLOR LIMIT", string.Empty).Trim();
                colorLimit = int.Parse(blocksString);
            }
            else if (line.StartsWith("STARS"))
            {
                string blocksString = line.Replace("STARS", string.Empty).Trim();
                string[] blocksNumbers = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                star1 = int.Parse(blocksNumbers[0]);
                star2 = int.Parse(blocksNumbers[1]);
                star3 = int.Parse(blocksNumbers[2]);
                if (ProgressBarScript.Instance != null)
                    ProgressBarScript.Instance.InitBar();
            }
            else if (line.StartsWith("COLLECT COUNT "))
            {
                string blocksString = line.Replace("COLLECT COUNT", string.Empty).Trim();
                string[] blocksNumbers = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < blocksNumbers.Length; i++)
                {
                    ingrCountTarget[i] = int.Parse(blocksNumbers[i]);
                }
            }
            else if (line.StartsWith("COLLECT ITEMS "))
            {
                string blocksString = line.Replace("COLLECT ITEMS", string.Empty).Trim();
                string[] blocksNumbers = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < blocksNumbers.Length; i++)
                {
                    if (target == Target.INGREDIENT)
                        ingrTarget[i] = (Ingredients)int.Parse(blocksNumbers[i]);
                    else if (target == Target.COLLECT)
                        collectItems[i] = (CollectItems)int.Parse(blocksNumbers[i]);

                }
            }
            else
            {
                string[] st = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < st.Length; i++)
                {
                    _levelSquaresFile[mapLine * maxCols + i].Block = (SquareTypes)int.Parse(st[i][0].ToString());
                    _levelSquaresFile[mapLine * maxCols + i].Obstacle = (SquareTypes)int.Parse(st[i][1].ToString());
                }
                mapLine++;
            }

            var tar = Resources.Load<TargetLevel>("Targets/Level" + currentLevel);
            targetObject = new TargetObject[tar.targets.Length];
            for (var index = 0; index < tar.targets.Count; index++)
            {
                var tarTarget = tar.targets[index];
                targetObject[index] = tarTarget.DeepCopy();
            }
        }
        levelLoaded = true;
    }

}

[System.Serializable]
public class GemProduct
{
    public int count;
    public float price;
}
