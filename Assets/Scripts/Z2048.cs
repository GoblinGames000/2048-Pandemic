using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Survival2048
{
    public class Z2048 : MonoBehaviour
    {
        #region Core Properties
        [Header("Grid Properties")]
        #region Grid Properties
        public int spawnValue1 = 2;
        public int spawnValue2 = 4;
        private int[,] gridValue = new int[4, 4];
        public static int gridwidth = 4, gridheight = 4;
        private Transform[,] gridPos = new Transform[gridwidth, gridheight];
        public static Transform[,] previousgrid = new Transform[gridwidth, gridheight];
        #endregion

        [Space(10)]
        [Header("Grid Properties")]
        #region Game Properties
        public GameObject zUnit;
        private GameObject[,] zGridUnit = new GameObject[4, 4];
        public Transform transZunitHolder;
        private enum GameState { mainMenu, inGame, progression }
        private GameState gameState = new GameState();
        private int stepCount = 0;
        #endregion

        [Space(10)]
        [Header("Survival Mode Properties")]
        #region Survival Mode Properties
        public bool survivalMode = false;
        public int survivalmode = 1;
        public int survivalmodeend = 0;
        public float Showtrophytime = 2f;
        private int survivalModeSaveData = 0;
        #endregion

        #region Database Properties
        public int currenttrophy = 4;
        public int score = 0;
        // public int normalscore = 0;
        public int bestscore = 0;
        public int previousscore = 0;
        public int wins = 50;

        public int Level = 1;
        public Slider levelbar;
        public int maxlevelbarvalue;
        public int currentlevelbarvalue;
        private static int trophyUnlockScore = 1000;
        private int[] data_Trophy = new int[40];
        #endregion

        [Space(10)]
        [Header("User Input")]
        #region User Input
        public Transform transSwipeFX;
        public ParticleSystem parSwipeFX;
        public float SWIPE_THRESHOLD = 20f;
        private Vector2 fingerDown;
        private Vector2 fingerUp;
        private Vector2 currentSwipe;
        private bool canReceiveTouchInput = false;
        [HideInInspector]
        public bool detectSwipeOnlyAfterRelease = false;
        [HideInInspector]
        public enum Direction { up, right, down, left }
        [HideInInspector]
        public Direction dir = new Direction();
        private List<Direction> dirList = new List<Direction>();
        #endregion
        #endregion

        #region This is where you'll want to modify for your own game
        [Space(10)]
        [Header("UI Properties")]
        #region UI Properties
        public GameObject panelMainMenu;
        public GameObject panelContinueGame;
        public GameObject panelBottomUI;
        public GameObject panelProgression;
        public GameObject panelProgressionnormal;
        public GameObject panelWin;

        public Text[] txtCurrentScore;
        public GameObject[] txtsurvival;
        public GameObject[] txtbest;
        public GameObject titleTrophy;
        public GameObject titleGameOver;
        public GameObject uiTrophyItem;
        public GameObject[] uiTrophy;
        public GameObject btnBackToGame;
        public Transform transTrophyContent;
        public LayoutGroup layoutTrophyContent;
        #endregion

        [Space(10)]
        [Header("Camera & Efx")]
        #region Camera & Efx
        public Animation animCameraController;
        #endregion

        [Space(10)]
        [Header("Audio Properties")]
        #region Audio Properties

        private SoundManager soundManager;
        private MainMenuUIManager mainMenuUIManager;

        #endregion
        #endregion

        public int startnewgametime;
        public Button[] norm_surv;
        #region Debug Properties
        public bool DebugMode = false;
        TextMesh[,] textMesh_GridValue = new TextMesh[4, 4];

        // In Game Undo Button UI Stuff
        public TextMeshProUGUI undoCountText;
        public Button undoButton;

        [Space(10)]
        [Header("Gameover Undo Stuff")]

        private bool gameOverUndoUsed; // false = first time game over, true = second time game over after undoing on gameover
        public GameObject gameOverUndoPanel;
        public Slider gameOverUndoTimerSlider;
        private bool gameOverUndoTimerStarted = false; // true = gameOverUndoPanel is active, false = panel inactive
        public float timer = 5.0f; // Timer for undoing after gameover
        private float remainingTimer; // the countdown var

        //private int deaths;
        public int adWatched;
        private bool showInterstitial;
        public MobileAd mobileAd;


        #endregion
        #region Routine
        private void Start()
        {
            soundManager = GameObject.FindWithTag("Sound Manager").GetComponent<SoundManager>();
            mainMenuUIManager = GameObject.Find("UI Manager").GetComponent<MainMenuUIManager>();
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            if (!PlayerPrefs.HasKey("UndoCount"))
            {
                PlayerPrefs.SetInt("UndoCount", 0);
            }
            InitUI();
            InitiateGame();
            SetUndoCount();
            gameOverUndoUsed = false;

        }
        private void Update()
        {
            /*
            if (gameState == GameState.inGame && canReceiveTouchInput)
            {
                OnSwipe();
            }*/
            if(gameOverUndoTimerStarted)
            {
                if (remainingTimer > 0)
                {
                    remainingTimer -= Time.deltaTime;
                    gameOverUndoTimerSlider.value = (remainingTimer / timer) * gameOverUndoTimerSlider.maxValue;
                }
                else 
                {
                    gameOverUndoTimerStarted = false;
                    GameOver();
                }
            }

            if (gameState == GameState.inGame)
            {
                OnSwipe();
                if (survivalmode == 1)
                {

                    currentlevelbarvalue = score;
                    levelbar.value = currentlevelbarvalue;
                    PlayerPrefs.SetInt("currentlvl", currentlevelbarvalue);
                    if (currenttrophy == 40)
                    {
                        currenttrophy = 40;
                        PlayerPrefs.SetInt("troph", currenttrophy);
                        survivalmodeend = 1;
                        PlayerPrefs.SetInt("survmodeend", survivalmodeend);
                        if (survivalmodeend == 1)
                        {
                            if (survivalmode == 1)
                            {
                                if (gameState == GameState.inGame)
                                {
                                    panelWin.SetActive(true);
                                    txtsurvival[5].SetActive(false);
                                    ResetGrid();
                                }

                            }


                        }

                    }
                }
                if (dirList.Count > 0)
                {
                    if (canReceiveTouchInput)
                    {
                        StartCoroutine(ShiftGridValue(dirList[dirList.Count - 1]));
                    }
                }
            }
            if (gameState == GameState.mainMenu)
            {

                panelWin.SetActive(false);
                score = 0;
                PlayerPrefs.SetInt("Score", score);

            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                //score = 3300;
                //GameOver();
                if (survivalmode == 1)
                {
                    currenttrophy = 40;
                }
            }

        }
        public void Set(int a)
        {
            this.Invoke(() => { ShowMAp.Instance.ShowMap((a)); }, 2f);

        }
        void updatelevel()
        {



            if (survivalmode == 1)
            {
                maxlevelbarvalue = wins;
                PlayerPrefs.SetInt("maxlevelbar", maxlevelbarvalue);
                levelbar.maxValue = maxlevelbarvalue;



                if (score >= wins)
                {

                    if ((currenttrophy + 1) <= Level)
                    {
                        uiTrophy[currenttrophy].GetComponent<ZTrophy>().SetProgress(100000, 100000, trophyUnlockScore);
                        if (soundManager.makevibration == 1)
                        {
                            Handheld.Vibrate();
                        }
                        currenttrophy += 1;
                        PlayerPrefs.SetInt("troph", currenttrophy);
                        // sfxUnlock.Play();
                        soundManager.PlayUnlockMusic();
                        print("Unlocked played");
                        if (currenttrophy != 40)
                        {
                            StartCoroutine(showtroph1s());
                        }
                    }
                    print("score equal wins");
                    score = 0;
                    PlayerPrefs.SetInt("Score", score);
                    Level += 1;



                    PlayerPrefs.SetInt("level", Level);
                    wins += 50;
                    PlayerPrefs.SetInt("wins", wins);
                    //  _trophy.GetComponent<ZTrophy>().UnLocked();
                }

                //if (Level == 41 && wins == 2000 && score == 2000)
                //{
                //    GameOver();
                //}
            }
            if (survivalmode == 0)
            {


                if (bestscore <= score)
                {
                    bestscore = score;
                    PlayerPrefs.SetInt("bestscore", bestscore);
                }
                if (PlayerPrefs.GetInt("bestscore") <= bestscore)
                {
                    PlayerPrefs.SetInt("bestscore", bestscore);
                }
            }

        }
        IEnumerator showtroph1s()
        {
            ShowTrophy();

            yield return new WaitForSeconds(Showtrophytime);
            BackToGame();
        }
        public void Set(float val)
        {
            Debug.Log(val % 5);
            Debug.Log(val / 5);
        }
        private void SetUndoCount()
        {
            int undoCount = PlayerPrefs.GetInt("UndoCount");
            undoCountText.text = undoCount.ToString();
            undoCountText.fontSize = 20;
            if (undoCount > 0 && PlayerPrefs.GetInt("CanUndo") == 1)
            {
                undoButton.interactable = true;
            }
            else
            {
                undoButton.interactable = false;
            }
        }
        public void IncrementUndoCount()
        {
            int undoCount = PlayerPrefs.GetInt("UndoCount");
            undoCount++;
            PlayerPrefs.SetInt("UndoCount", undoCount);
            SetUndoCount();
        }
        private void DecrementUndoCount()
        {
            int undoCount = PlayerPrefs.GetInt("UndoCount");
            undoCount--;
            PlayerPrefs.SetInt("UndoCount", undoCount);
            SetUndoCount();
        }

        public void GameOverUndo()
        {
            Undo(false);
        }
        public void InGameUndo()
        {
            Undo(true);
        }
        private void Undo(bool inGameUndo)
        {
            if (inGameUndo)
            {
                if (PlayerPrefs.HasKey("CanUndo") && PlayerPrefs.HasKey("TotalMoves"))
                {
                    if (PlayerPrefs.GetInt("UndoCount") > 0 &&PlayerPrefs.GetInt("TotalMoves") > 0 && PlayerPrefs.GetInt("CanUndo") == 1)
                    {
                        StartCoroutine(LoadPrevoiusMove());
                        //PlayerPrefs.SetInt("CanUndo", 0);
                        DecrementUndoCount();
                    }
                }
            }
            // Game Over Undo x3
            else
            {
                if (PlayerPrefs.HasKey("TotalMoves"))
                {
                    if (PlayerPrefs.GetInt("TotalMoves") >= 5)
                    {
                        gameOverUndoPanel.SetActive(false);
                        panelBottomUI.SetActive(true);
                        gameOverUndoTimerStarted = false;
                        int totalMoves = PlayerPrefs.GetInt("TotalMoves");
                        for (int i = 0; i < 4; i++)
                        {
                            for (int x = 0; x < 4; x++)
                            {
                                for (int y = 0; y < 4; y++)
                                {
                                    PlayerPrefs.DeleteKey("PreviousGrid" + totalMoves + "Value" + x + y);
                                }
                            }
                            totalMoves--;
                        }
                        PlayerPrefs.SetInt("TotalMoves", totalMoves);
                        StartCoroutine(LoadPrevoiusMove());
                    }
                }
            }
            //for (int x = 0; x < 4; x++)
            //{
            //    for (int y = 0; y < 4; y++)
            //    {
            //        //gridValue[x, y] = gridValue[x, y] ;
            //        gridPos[x, y].transform.position = previousgrid[x, y].transform.position;
            //        PlayerPrefs.SetInt("GridValue" + x + y, gridValue[x, y]);
            //    }
            //}

            //if (transZunitHolder.childCount > 0)
            //{
            //    foreach (Transform child in transZunitHolder)
            //    {
            //        Destroy(child.gameObject);
            //    }
            //}
            //for (int x = 0; x < gridwidth; x++)
            //{
            //    for (int y = 0; y < gridheight; y++)
            //    {
            //        gridPos[x, y] = null;
            //        Nottile nottile = previousgrid[x, y];
            //        if (nottile != null)
            //        {
            //            int tilevalue = nottile.value;
            //            string newtilename = "tile_" + tilevalue;
            //            GameObject newtile = (GameObject)Instantiate(Resources.Load(newtilename, typeof(GameObject)), nottile.location, Quaternion.identity);
            //            zGridUnit[x, y] = newtile;
            //            newtile.transform.parent = transform;
            //            gridPos[x, y] = newtile.transform;
            //        }

            //    }
            //}
        }
        #endregion

        #region Touch + Mouse Events
        void OnSwipe()
        {
            bool outOfRange = false;

            //Check if the swipe area is within grid area
            if (Input.mousePosition.y > Screen.height * 0.7f || Input.mousePosition.y < Screen.height * 0.15f)
            {
                outOfRange = true;
            }

            if (!outOfRange)
            {

                #region Mouse Input Detection
                if (Input.GetMouseButtonDown(0))
                {
                    fingerDown = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                }
                else if (Input.GetMouseButton(0))
                {
                    transSwipeFX.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    parSwipeFX.emissionRate = 120;
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    fingerUp = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                    currentSwipe = new Vector2(fingerUp.x - fingerDown.x, fingerUp.y - fingerDown.y);
                    currentSwipe.Normalize();
                    parSwipeFX.emissionRate = 0;
                    //storeprevioustile();
                    if (currentSwipe.y > 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
                    {
                        dir = Direction.up;
                    }

                    if (currentSwipe.y < 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
                    {
                        dir = Direction.down;
                    }

                    if (currentSwipe.x < 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
                    {
                        dir = Direction.left;
                    }

                    if (currentSwipe.x > 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
                    {

                        dir = Direction.right;
                    }

                    //StartCoroutine(ShiftGridValue(dir));
                    dirList.Add(dir);
                }
                #endregion
                else
                {
                    #region Touch Input Detection
                    foreach (Touch touch in Input.touches)
                    {
                        if (touch.phase == TouchPhase.Began)
                        {
                            fingerUp = touch.position;
                            fingerDown = touch.position;
                        }

                        if (touch.phase == TouchPhase.Moved)
                        {
                            transSwipeFX.position = Camera.main.ScreenToWorldPoint(touch.position);
                            parSwipeFX.emissionRate = 120;

                            if (!detectSwipeOnlyAfterRelease)
                            {
                                fingerDown = touch.position;
                                CheckSwipe();
                            }
                        }

                        if (touch.phase == TouchPhase.Ended)
                        {
                            fingerDown = touch.position;
                            parSwipeFX.emissionRate = 0;
                            CheckSwipe();
                        }
                    }
                    #endregion
                }
            }
        }

        #region For Touch Events Only
        void CheckSwipe()
        {
            if (VerticalMove() > SWIPE_THRESHOLD && VerticalMove() > HorizontalValMove())
            {
                if (fingerDown.y - fingerUp.y > 0)
                {
                    dir = Direction.up;
                }
                else if (fingerDown.y - fingerUp.y < 0)
                {
                    dir = Direction.down;
                }

                //StartCoroutine(ShiftGridValue(dir));
                dirList.Add(dir);
                fingerUp = fingerDown;
            }

            else if (HorizontalValMove() > SWIPE_THRESHOLD && HorizontalValMove() > VerticalMove())
            {
                if (fingerDown.x - fingerUp.x > 0)
                {
                    dir = Direction.right;
                }
                else if (fingerDown.x - fingerUp.x < 0)
                {
                    dir = Direction.left;
                }

                //StartCoroutine(ShiftGridValue(dir));
                dirList.Add(dir);
                fingerUp = fingerDown;
            }
            else
            {
                //Debug.Log("No Swipe!");
            }
        }

        float VerticalMove()
        {
            return Mathf.Abs(fingerDown.y - fingerUp.y);
        }

        float HorizontalValMove()
        {
            return Mathf.Abs(fingerDown.x - fingerUp.x);
        }
        #endregion
        #endregion

        #region 2040 Core Mechanics

        private void storeprevioustile()
        {
            //previousscore = score;
            //for (int x = 0; x < gridwidth; x++)
            //{
            //    for (int y = 0; y < gridheight; y++)
            //    {
            //        previousgrid[x, y].transform.position = gridPos[x, y].transform.position;
            //    }
            //}

        }
        private void InitiateGame()
        {
            gameState = GameState.mainMenu;


            #region Save / Load Data
            if (PlayerPrefs.HasKey("StepCount"))
            {
                stepCount = PlayerPrefs.GetInt("StepCount");
            }
            else
            {
                PlayerPrefs.SetInt("StepCount", stepCount);
            }

            if (PlayerPrefs.HasKey("SurvivalMode"))
            {
                survivalModeSaveData = PlayerPrefs.GetInt("SurvivalMode");

            }


            else
            {

                PlayerPrefs.SetInt("SurvivalMode", survivalModeSaveData);

            }

            if (PlayerPrefs.HasKey("level"))
            {
                Level = PlayerPrefs.GetInt("level");
            }
            else
            {
                PlayerPrefs.SetInt("level", Level);
            }
            if (PlayerPrefs.HasKey("wins"))
            {
                wins = PlayerPrefs.GetInt("wins");
            }
            else
            {
                PlayerPrefs.SetInt("wins", wins);
            }
            if (PlayerPrefs.HasKey("survivalmode"))
            {
                survivalmode = PlayerPrefs.GetInt("survivalmode");
            }
            else
            {
                PlayerPrefs.SetInt("survivalmode", survivalmode);
            }
            if (PlayerPrefs.HasKey("bestscore"))
            {
                bestscore = PlayerPrefs.GetInt("bestscore");
            }
            else
            {
                PlayerPrefs.SetInt("bestscore", bestscore);
            }
            if (PlayerPrefs.HasKey("currentlvl"))
            {
                currentlevelbarvalue = PlayerPrefs.GetInt("currentlvl");
            }
            else
            {
                PlayerPrefs.SetInt("currentlvl", currentlevelbarvalue);
            }
            if (PlayerPrefs.HasKey("maxlevelbar"))
            {
                maxlevelbarvalue = PlayerPrefs.GetInt("maxlevelbar");
            }
            else
            {
                PlayerPrefs.SetInt("maxlevelbar", maxlevelbarvalue);
            }
            if (PlayerPrefs.HasKey("bestscore"))
            {
                bestscore = PlayerPrefs.GetInt("bestscore");
            }
            else
            {
                PlayerPrefs.SetInt("bestscore", bestscore);
            }
            if (PlayerPrefs.HasKey("troph"))
            {

                currenttrophy = PlayerPrefs.GetInt("troph");
                SoundManager.Instance.SetBackgroundLevel(currenttrophy);
            }
            else
            {

                PlayerPrefs.SetInt("troph", currenttrophy);
                SoundManager.Instance.SetBackgroundLevel(currenttrophy);

            }
            if (PlayerPrefs.HasKey("survmodeend"))
            {
                survivalmodeend = PlayerPrefs.GetInt("survmodeend");
            }
            else
            {
                PlayerPrefs.SetInt("survmodeend", survivalmodeend);
            }
            if (PlayerPrefs.HasKey("vibrate"))
            {
                soundManager.makevibration = PlayerPrefs.GetInt("vibrate");
            }
            else
            {
                PlayerPrefs.SetInt("vibrate", soundManager.makevibration);
            }
            if (PlayerPrefs.HasKey("backgroundm"))
            {
                soundManager.backgroundm = PlayerPrefs.GetInt("backgroundm");
            }
            else
            {
                PlayerPrefs.SetInt("backgroundm", soundManager.backgroundm);
            }


            #endregion
            //score = 0;
            //PlayerPrefs.SetInt("Score", score);
            if (soundManager.backgroundm == 1)
            {
                soundManager.MusicOn();
            }
            if (soundManager.backgroundm == 0)
            {
                soundManager.MusicOff();
            }
            if (soundManager.makevibration == 0)
            {
                soundManager.VibrateOff();
            }
            if (soundManager.makevibration == 1)
            {
                soundManager.VibrateOn();
            }
            //for(int t = 0; t<uiTrophy.Length; t++)
            //{
            for (int y = 0; y < currenttrophy; y++)
            {
                uiTrophy[y].GetComponent<ZTrophy>().SetProgress(100000, 100000, trophyUnlockScore);
            }
            //}
            #region Reseting Grid Values
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    gridPos[x, y] = GameObject.Find("tm" + x.ToString() + "-" + y.ToString()).transform;
                    zGridUnit[x, y] = null;
                }
            }
            #endregion

            #region Loading Grid Value
            if (stepCount > 0)
            {
                LoadScore();
                StartCoroutine(LoadGrid());

                if (survivalModeSaveData > 0)
                {
                    survivalMode = true;

                }
                else
                {


                    survivalMode = false;
                }
            }


            else
            {
                ResetScore();
                ResetGrid();

                #region UI
                panelMainMenu.SetActive(true);
                panelContinueGame.SetActive(false);
                #endregion
            }
            if (survivalmode == 1)
            {
                foreach (GameObject go in txtsurvival)
                {
                    go.SetActive(true);
                }
                foreach (GameObject be in txtbest)
                {
                    be.SetActive(false);
                }
            }
            else
            {
                foreach (GameObject go in txtsurvival)
                {
                    go.SetActive(false);
                }
                foreach (GameObject be in txtbest)
                {
                    be.SetActive(true);
                }
            }
            #endregion

            #region Debug Use (you may remove them if not needed)
            AssignGridTextMesh();
            UpdateGrigDebugValue();
            #endregion
        }

        private IEnumerator LoadGrid()
        {
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    gridValue[x, y] = PlayerPrefs.GetInt("GridValue" + x + y);

                    if (gridValue[x, y] > 0)
                    {
                        GameObject tempZUnit = Instantiate(zUnit, gridPos[x, y].position, gridPos[x, y].rotation, transZunitHolder);
                        zGridUnit[x, y] = tempZUnit;
                        tempZUnit.GetComponent<ZUnit>().SetUnitValue(gridValue[x, y], 0);

                        #region Setting Animation / Tweening
                        Hashtable ht = new Hashtable();
                        ht.Add("scale", new Vector3(0, 0, 0));
                        ht.Add("time", 0.3f);
                        ht.Add("easetype", iTween.EaseType.easeInOutExpo);
                        iTween.ScaleFrom(tempZUnit, ht);
                        #endregion

                        #region Sfx
                        // PlaySound(sfxSpawnGrid, 1);
                        soundManager.PlaySpawnGridMusic();
                        #endregion
                        yield return new WaitForSeconds(0.2f);
                    }
                }
            }

            #region UI
            panelMainMenu.SetActive(false);
            panelContinueGame.SetActive(true);
            #endregion
        }

        private IEnumerator LoadPrevoiusMove()
        {
            canReceiveTouchInput = false;
            foreach (Transform child in transZunitHolder)
            {
                GameObject.Destroy(child.gameObject);
            }
            int totalMoves = PlayerPrefs.GetInt("TotalMoves");
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    gridValue[x, y] = PlayerPrefs.GetInt("PreviousGrid" + totalMoves + "Value" + x + y);
                    PlayerPrefs.DeleteKey("PreviousGrid" + totalMoves + "Value" + x + y);
                    if (gridValue[x, y] > 0)
                    {
                        GameObject tempZUnit = Instantiate(zUnit, gridPos[x, y].position, gridPos[x, y].rotation, transZunitHolder);
                        zGridUnit[x, y] = tempZUnit;
                        tempZUnit.GetComponent<ZUnit>().SetUnitValue(gridValue[x, y], 0);

                        #region Setting Animation / Tweening
                        Hashtable ht = new Hashtable();
                        ht.Add("scale", new Vector3(0, 0, 0));
                        ht.Add("time", 0.3f);
                        ht.Add("easetype", iTween.EaseType.easeInOutExpo);
                        iTween.ScaleFrom(tempZUnit, ht);
                        #endregion

                        #region Sfx
                        // PlaySound(sfxSpawnGrid, 1);
                        soundManager.PlaySpawnGridMusic();
                        #endregion
                        yield return new WaitForSeconds(0.2f);
                    }
                }
            }
            totalMoves--;
            PlayerPrefs.SetInt("TotalMoves", totalMoves);
            Debug.Log("Total Moves: " + PlayerPrefs.GetInt("TotalMoves"));
            canReceiveTouchInput = true;
            SaveGrid();
        }

        private void SaveGrid()
        {
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    PlayerPrefs.SetInt("GridValue" + x + y, gridValue[x, y]);
                }
            }
        }

        private void SavePreviousMove()
        {
            PlayerPrefs.SetInt("CanUndo", 1);
            if (PlayerPrefs.GetInt("UndoCount") > 0)
            {
                undoButton.interactable = true;
            }
            // If undo Exists store based on count
            if (PlayerPrefs.HasKey("TotalMoves"))
            {
                int totalMoves = PlayerPrefs.GetInt("TotalMoves");
                // If undo exists
                if (totalMoves > 0)
                {
                    totalMoves++;
                    for (int x = 0; x < 4; x++)
                    {
                        for (int y = 0; y < 4; y++)
                        {
                            PlayerPrefs.SetInt("PreviousGrid" + totalMoves + "Value" + x + y, gridValue[x, y]);
                        }
                    }
                    PlayerPrefs.SetInt("TotalMoves", totalMoves);
                }

                // else store first undo
                else
                {
                    for (int x = 0; x < 4; x++)
                    {
                        for (int y = 0; y < 4; y++)
                        {
                            PlayerPrefs.SetInt("PreviousGrid" + 1 + "Value" + x + y, gridValue[x, y]);
                        }
                    }
                    PlayerPrefs.SetInt("TotalMoves", 1);
                }
            }
            // else store first undo
            else 
            {
                for (int x = 0; x < 4; x++)
                {
                    for (int y = 0; y < 4; y++)
                    {
                        PlayerPrefs.SetInt("PreviousGrid" + 1 + "Value" + x + y, gridValue[x, y]);
                    }
                }
                PlayerPrefs.SetInt("TotalMoves", 1);
            }
        }


        private void ResetGrid()
        {
            adWatched = 0;
            PlayerPrefs.SetInt("TotalMoves", 0);
            Debug.Log("Total Moves: " + PlayerPrefs.GetInt("TotalMoves"));
            PlayerPrefs.SetInt("CanUndo", 0);
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    gridValue[x, y] = 0;
                    PlayerPrefs.SetInt("GridValue" + x + y, gridValue[x, y]);
                }
            }

            if (transZunitHolder.childCount > 0)
            {
                foreach (Transform child in transZunitHolder)
                {
                    Destroy(child.gameObject);
                }
            }
            ResetScore();
        }

        private bool HaveEmptyGrid()
        {
            bool _emptyGrid = false;
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    if (gridValue[x, y] == 0)
                    {
                        _emptyGrid = true;
                    }
                }
            }

            return _emptyGrid;
        }

        private int[] GetRandomEmptyGrid()
        {
            int[] _randomEmptyGrid = new int[2];
            List<int[]> _emptyGridList = new List<int[]>();

            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    if (gridValue[x, y] == 0)
                    {
                        int[] _tempValue = new int[2];
                        _tempValue[0] = x;
                        _tempValue[1] = y;
                        _emptyGridList.Add(_tempValue);
                    }
                }
            }

            _randomEmptyGrid = _emptyGridList[UnityEngine.Random.Range(0, _emptyGridList.Count - 1)];

            return _randomEmptyGrid;
        }

        private void AddNewGridValue()
        {
            if (HaveEmptyGrid())
            {
                int _diceValue = UnityEngine.Random.Range(0, 100);
                int[] xy = GetRandomEmptyGrid();

                if (survivalMode)
                {

                    if (_diceValue > 90)
                    {
                        gridValue[xy[0], xy[1]] = 3;
                    }
                    else if (_diceValue > 40 && _diceValue < 91)
                    {
                        gridValue[xy[0], xy[1]] = spawnValue1;
                    }
                    else if (_diceValue < 41)
                    {
                        gridValue[xy[0], xy[1]] = spawnValue2;
                    }
                }
                else
                {

                    if (_diceValue < 50)
                    {
                        gridValue[xy[0], xy[1]] = spawnValue1;
                    }
                    else
                    {
                        gridValue[xy[0], xy[1]] = spawnValue2;
                    }
                }

                GameObject tempZUnit = Instantiate(zUnit, gridPos[xy[0], xy[1]].position, gridPos[xy[0], xy[1]].rotation, transZunitHolder);
                zGridUnit[xy[0], xy[1]] = tempZUnit;
                tempZUnit.GetComponent<ZUnit>().SetUnitValue(gridValue[xy[0], xy[1]], 0);

                #region Setting Animation
                Hashtable ht = new Hashtable();
                ht.Add("scale", new Vector3(0, 0, 0));
                ht.Add("time", 0.3f);
                ht.Add("easetype", iTween.EaseType.easeInOutExpo);
                iTween.ScaleFrom(tempZUnit, ht);
                #endregion

                #region Debug Use
                UpdateGrigDebugValue();
                #endregion
            }
        }

        private void AddPlayer()
        {
            if (HaveEmptyGrid())
            {


                int _diceValue = UnityEngine.Random.Range(0, 100);
                int[] xy = GetRandomEmptyGrid();
                gridValue[xy[0], xy[1]] = 1;

                GameObject tempZUnit = Instantiate(zUnit, gridPos[xy[0], xy[1]].position, gridPos[xy[0], xy[1]].rotation, transZunitHolder);
                zGridUnit[xy[0], xy[1]] = tempZUnit;
                tempZUnit.GetComponent<ZUnit>().SetUnitValue(gridValue[xy[0], xy[1]], 0);

                #region Setting Animation
                Hashtable ht = new Hashtable();
                ht.Add("scale", new Vector3(0, 0, 0));
                ht.Add("time", 0.3f);
                ht.Add("easetype", iTween.EaseType.easeInOutExpo);
                iTween.ScaleFrom(tempZUnit, ht);
                #endregion

                #region Debug Use
                UpdateGrigDebugValue();
                #endregion
            }


        }

        private IEnumerator ShiftGridValue(Direction _dir)
        {
            //Debug.Log("ShiftGridValue = " + _dir);
            canReceiveTouchInput = false;
            SavePreviousMove();
            bool _moved = false;
            bool _merged = false;
            float _pitch = 1;
            dirList.RemoveAt(dirList.Count - 1);

            #region Assign directional value
            if (dir == Direction.up)
            {
                for (int x = 0; x < 4; x++)
                {
                    bool[] lockedValue = new bool[4];
                    bool[] initiatedEfx = new bool[4];

                    for (int y = 2; y >= 0; y--)
                    {
                        if (gridValue[x, y] != 0)
                        {
                            int _count = y + 1;

                            while (_count <= 3)
                            {
                                if (!lockedValue[_count])
                                {
                                    if (gridValue[x, _count] == 0)
                                    {
                                        gridValue[x, _count] = gridValue[x, _count - 1];
                                        zGridUnit[x, _count] = zGridUnit[x, _count - 1];
                                        gridValue[x, _count - 1] = 0;
                                        zGridUnit[x, _count - 1] = null;
                                        iTween.MoveTo(zGridUnit[x, _count], gridPos[x, _count].position, 0.2f);
                                        _moved = true;

                                        if (!initiatedEfx[x])
                                        {
                                            zGridUnit[x, _count].GetComponent<ZUnit>().ShowHitEfx(dir, 0.18f);
                                            initiatedEfx[x] = true;
                                        }
                                    }
                                    else if (gridValue[x, _count - 1] == gridValue[x, _count] && gridValue[x, _count - 1] != 3)
                                    {
                                        gridValue[x, _count] = gridValue[x, _count] * 2;
                                        AddScore(gridValue[x, _count]);
                                        zGridUnit[x, _count].GetComponent<ZUnit>().SetUnitValue(gridValue[x, _count], 0.2f);
                                        zGridUnit[x, _count].GetComponent<ZUnit>().ShowMergeEfx(0.18f);
                                        gridValue[x, _count - 1] = 0;
                                        iTween.MoveTo(zGridUnit[x, _count - 1], gridPos[x, _count].position, 0.2f);
                                        _moved = true;
                                        _merged = true;
                                        Destroy(zGridUnit[x, _count - 1], 0.18f);
                                        zGridUnit[x, _count - 1] = null;

                                        for (int i = _count; i < 4; i++)
                                        {
                                            lockedValue[i] = true;
                                        }

                                        if (!initiatedEfx[x])
                                        {
                                            zGridUnit[x, _count].GetComponent<ZUnit>().ShowHitEfx(dir, 0.18f);
                                            initiatedEfx[x] = true;
                                        }

                                        float _pitchLocal = GetPitchValue(gridValue[x, _count]);

                                        if (_pitchLocal > _pitch)
                                        {
                                            _pitch = _pitchLocal;
                                        }

                                        break;
                                    }
                                }

                                _count++;
                            }
                        }
                    }
                }
            }

            if (dir == Direction.right)
            {
                for (int y = 0; y < 4; y++)
                {
                    bool[] lockedValue = new bool[4];
                    bool[] initiatedEfx = new bool[4];

                    for (int x = 2; x >= 0; x--)
                    {
                        if (gridValue[x, y] != 0)
                        {
                            int _count = x + 1;

                            while (_count <= 3)
                            {
                                if (!lockedValue[_count])
                                {
                                    if (gridValue[_count, y] == 0)
                                    {
                                        gridValue[_count, y] = gridValue[_count - 1, y];
                                        zGridUnit[_count, y] = zGridUnit[_count - 1, y];
                                        gridValue[_count - 1, y] = 0;
                                        zGridUnit[_count - 1, y] = null;
                                        iTween.MoveTo(zGridUnit[_count, y], gridPos[_count, y].position, 0.2f);
                                        _moved = true;

                                        if (!initiatedEfx[y])
                                        {
                                            zGridUnit[_count, y].GetComponent<ZUnit>().ShowHitEfx(dir, 0.18f);
                                            initiatedEfx[y] = true;
                                        }
                                    }
                                    else if (gridValue[_count - 1, y] == gridValue[_count, y] && gridValue[_count - 1, y] != 3)
                                    {
                                        gridValue[_count, y] = gridValue[_count, y] * 2;
                                        AddScore(gridValue[_count, y]);
                                        zGridUnit[_count, y].GetComponent<ZUnit>().SetUnitValue(gridValue[_count, y], 0.2f);
                                        zGridUnit[_count, y].GetComponent<ZUnit>().ShowMergeEfx(0.18f);
                                        gridValue[_count - 1, y] = 0;
                                        iTween.MoveTo(zGridUnit[_count - 1, y], gridPos[_count, y].position, 0.2f);
                                        _moved = true;
                                        _merged = true;
                                        Destroy(zGridUnit[_count - 1, y], 0.18f);
                                        zGridUnit[_count - 1, y] = null;

                                        for (int i = _count; i < 4; i++)
                                        {
                                            lockedValue[i] = true;
                                        }

                                        if (!initiatedEfx[y])
                                        {
                                            zGridUnit[_count, y].GetComponent<ZUnit>().ShowHitEfx(dir, 0.18f);
                                            initiatedEfx[y] = true;
                                        }

                                        float _pitchLocal = GetPitchValue(gridValue[_count, y]);

                                        if (_pitchLocal > _pitch)
                                        {
                                            _pitch = _pitchLocal;
                                        }

                                        break;
                                    }
                                }

                                _count++;
                            }
                        }
                    }
                }
            }

            if (dir == Direction.down)
            {
                for (int x = 0; x < 4; x++)
                {
                    bool[] lockedValue = new bool[4];
                    bool[] initiatedEfx = new bool[4];

                    for (int y = 1; y < 4; y++)
                    {
                        if (gridValue[x, y] != 0)
                        {
                            int _count = y - 1;

                            while (_count >= 0)
                            {
                                if (!lockedValue[_count])
                                {
                                    if (gridValue[x, _count] == 0)
                                    {
                                        gridValue[x, _count] = gridValue[x, _count + 1];
                                        zGridUnit[x, _count] = zGridUnit[x, _count + 1];
                                        gridValue[x, _count + 1] = 0;
                                        zGridUnit[x, _count + 1] = null;
                                        iTween.MoveTo(zGridUnit[x, _count], gridPos[x, _count].position, 0.2f);
                                        _moved = true;

                                        if (!initiatedEfx[x])
                                        {
                                            zGridUnit[x, _count].GetComponent<ZUnit>().ShowHitEfx(dir, 0.18f);
                                            initiatedEfx[x] = true;
                                        }
                                    }
                                    else if (gridValue[x, _count + 1] == gridValue[x, _count] && gridValue[x, _count + 1] != 3)
                                    {
                                        gridValue[x, _count] = gridValue[x, _count] * 2;
                                        AddScore(gridValue[x, _count]);
                                        zGridUnit[x, _count].GetComponent<ZUnit>().SetUnitValue(gridValue[x, _count], 0.2f);
                                        zGridUnit[x, _count].GetComponent<ZUnit>().ShowMergeEfx(0.18f);
                                        gridValue[x, _count + 1] = 0;
                                        iTween.MoveTo(zGridUnit[x, _count + 1], gridPos[x, _count].position, 0.2f);
                                        _moved = true;
                                        _merged = true;
                                        Destroy(zGridUnit[x, _count + 1], 0.18f);
                                        zGridUnit[x, _count + 1] = null;

                                        for (int i = 0; i < _count + 1; i++)
                                        {
                                            lockedValue[i] = true;
                                        }

                                        if (!initiatedEfx[x])
                                        {
                                            zGridUnit[x, _count].GetComponent<ZUnit>().ShowHitEfx(dir, 0.18f);
                                            initiatedEfx[x] = true;
                                        }

                                        float _pitchLocal = GetPitchValue(gridValue[x, _count]);

                                        if (_pitchLocal > _pitch)
                                        {
                                            _pitch = _pitchLocal;
                                        }

                                        break;
                                    }
                                }

                                _count--;
                            }
                        }
                    }
                }
            }

            if (dir == Direction.left)
            {
                for (int y = 0; y < 4; y++)
                {
                    bool[] lockedValue = new bool[4];
                    bool[] initiatedEfx = new bool[4];

                    for (int x = 1; x < 4; x++)
                    {
                        if (gridValue[x, y] != 0)
                        {
                            int _count = x - 1;

                            while (_count >= 0)
                            {
                                if (!lockedValue[_count])
                                {
                                    if (gridValue[_count, y] == 0)
                                    {
                                        gridValue[_count, y] = gridValue[_count + 1, y];
                                        zGridUnit[_count, y] = zGridUnit[_count + 1, y];
                                        gridValue[_count + 1, y] = 0;
                                        zGridUnit[_count + 1, y] = null;
                                        iTween.MoveTo(zGridUnit[_count, y], gridPos[_count, y].position, 0.2f);
                                        _moved = true;

                                        if (!initiatedEfx[y])
                                        {
                                            zGridUnit[_count, y].GetComponent<ZUnit>().ShowHitEfx(dir, 0.18f);
                                            initiatedEfx[y] = true;
                                        }
                                    }
                                    else if (gridValue[_count + 1, y] == gridValue[_count, y] && gridValue[_count + 1, y] != 3)
                                    {
                                        gridValue[_count, y] = gridValue[_count, y] * 2;
                                        AddScore(gridValue[_count, y]);
                                        zGridUnit[_count, y].GetComponent<ZUnit>().SetUnitValue(gridValue[_count, y], 0.2f);
                                        zGridUnit[_count, y].GetComponent<ZUnit>().ShowMergeEfx(0.18f);
                                        gridValue[_count + 1, y] = 0;
                                        iTween.MoveTo(zGridUnit[_count + 1, y], gridPos[_count, y].position, 0.2f);
                                        _moved = true;
                                        _merged = true;
                                        canReceiveTouchInput = false;
                                        Destroy(zGridUnit[_count + 1, y], 0.18f);
                                        zGridUnit[_count + 1, y] = null;

                                        for (int i = 0; i < _count + 1; i++)
                                        {
                                            lockedValue[i] = true;
                                        }

                                        if (!initiatedEfx[y])
                                        {
                                            zGridUnit[_count, y].GetComponent<ZUnit>().ShowHitEfx(dir, 0.18f);
                                            initiatedEfx[y] = true;
                                        }

                                        float _pitchLocal = GetPitchValue(gridValue[_count, y]);

                                        if (_pitchLocal > _pitch)
                                        {
                                            _pitch = _pitchLocal;
                                        }
                                        break;
                                    }
                                }

                                _count--;
                            }
                        }
                    }
                }
            }

            UpdateGrigDebugValue(); //Debug use

            #endregion

            #region Sfx
            if (_moved)
            {
                soundManager.PlayMovingTileMusic();
            }
            else
            {
                int totalMoves = PlayerPrefs.GetInt("TotalMoves");
                for (int x = 0; x < 4; x++)
                {
                    for (int y = 0; y < 4; y++)
                    {
                        PlayerPrefs.DeleteKey("PreviousGrid" + totalMoves + "Value" + x + y);
                    }
                }
                totalMoves--;
                PlayerPrefs.SetInt("TotalMoves", totalMoves);
            }
            #endregion

            yield return new WaitForSeconds(0.07f);

            if (_moved)
            {
                if (_merged)
                {
                    #region Efx + Sfx
                    animCameraController.Play(dir.ToString());
                    soundManager.PlayCombineTileMusic();
                    #endregion
                }

                if (survivalMode)
                {
                    StartCoroutine("KillEnemy");
                }

                AddNewGridValue();
                SaveGrid();
                yield return new WaitForSeconds(0.3f);
                canReceiveTouchInput = true;

                #region Debug Use
                UpdateGrigDebugValue();
                #endregion
            }
            else
            {
                yield return new WaitForSeconds(0.3f);
                canReceiveTouchInput = true;
            }

            #region Check If the game should be ended
            if (!HaveEmptyGrid())
            {
                if (!HaveMergeablePair())
                {
                    //Debug.Log("GAME OVER");

                    yield return new WaitForSeconds(1);
                    SaveGrid();
                    GameOver();
                }
                else
                {
                    stepCount++;
                }
            }
            else
            {
                stepCount++;
            }

            PlayerPrefs.SetInt("StepCount", stepCount);
            #endregion
        }
        public int DummyIndex;
        private bool HaveMergeablePair()
        {
            bool _havePair = false;

            #region Check verticle pair
            for (int x = 0; x < 4; x++)
            {
                for (int y = 1; y < 4; y++)
                {
                    if (gridValue[x, y] == gridValue[x, y - 1] && gridValue[x, y] != 3)
                    {
                        _havePair = true;
                    }
                }
            }
            #endregion

            #region Check horizontal pair
            if (!_havePair)
            {
                for (int y = 0; y < 4; y++)
                {
                    for (int x = 1; x < 4; x++)
                    {
                        if (gridValue[x, y] == gridValue[x - 1, y] && gridValue[x, y] != 3)
                        {
                            _havePair = true;
                        }
                    }
                }
            }
            #endregion

            return _havePair;
        }
        #endregion

        #region Survival Mechanic (where you'll want to enhance for your game base on your game design)
        private IEnumerator KillEnemy()
        {
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    if (gridValue[x, y] == 1)
                    {
                        if (y + 1 < 4)
                        {
                            if (gridValue[x, y + 1] == 3)
                            {
                                gridValue[x, y + 1] = 0;
                                zGridUnit[x, y + 1].GetComponent<ZUnit>().ShowHitEfx();
                                Destroy(zGridUnit[x, y + 1]);
                                yield return new WaitForSeconds(0.3f);
                            }
                        }

                        if (y - 1 > -1)
                        {
                            if (gridValue[x, y - 1] == 3)
                            {
                                gridValue[x, y - 1] = 0;
                                zGridUnit[x, y - 1].GetComponent<ZUnit>().ShowHitEfx();
                                Destroy(zGridUnit[x, y - 1]);
                                yield return new WaitForSeconds(0.3f);
                            }
                        }

                        if (x + 1 < 4)
                        {
                            if (gridValue[x + 1, y] == 3)
                            {
                                gridValue[x + 1, y] = 0;
                                zGridUnit[x + 1, y].GetComponent<ZUnit>().ShowHitEfx();
                                Destroy(zGridUnit[x + 1, y]);
                                yield return new WaitForSeconds(0.3f);
                            }
                        }

                        if (x - 1 > -1)
                        {
                            if (gridValue[x - 1, y] == 3)
                            {
                                gridValue[x - 1, y] = 0;
                                zGridUnit[x - 1, y].GetComponent<ZUnit>().ShowHitEfx();
                                Destroy(zGridUnit[x - 1, y]);
                                yield return new WaitForSeconds(0.3f);
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region UI Methods (where you might want to modify based on your game theme)
        private void InitUI()
        {
            panelMainMenu.SetActive(false);
            IniteTrophy();
        }

        #region Start / Continue Game
        public void PlayGame(bool _survivalMode)
        {
            gameOverUndoPanel.SetActive(false);
            gameOverUndoUsed = false;
            gameState = GameState.inGame;
            survivalMode = _survivalMode;
            canReceiveTouchInput = false;

            #region UI + Sfx



            panelMainMenu.SetActive(false);
            panelBottomUI.SetActive(true);
            panelContinueGame.SetActive(false);
            soundManager.PlayStartGameMusic();

            #endregion

            if (survivalMode)
            {
                survivalModeSaveData = 1;
            }
            else
            {
                survivalModeSaveData = 0;
            }

            PlayerPrefs.SetInt("SurvivalMode", survivalModeSaveData);

            if (stepCount <= 0)
            {
                AddNewGridValue();
                AddNewGridValue();

                if (survivalMode)
                {
                    AddPlayer();
                }
            }

            stepCount++;
            PlayerPrefs.SetInt("StepCount", stepCount);
            SaveGrid();

            StartCoroutine("PlayGameCoroutine");

            //if (survivalmode == 1 && survivalmodeend ==0 )
            //{
            //    gameState = GameState.inGame;
            //    survivalMode = _survivalMode;
            //    canReceiveTouchInput = false;

            //    #region UI + Sfx



            //    panelMainMenu.SetActive(false);
            //    panelBottomUI.SetActive(true);
            //    panelContinueGame.SetActive(false);
            //    PlaySound(sfxStartGame, 1);

            //    #endregion

            //    if (survivalMode)
            //    {
            //        survivalModeSaveData = 1;
            //    }
            //    else
            //    {
            //        survivalModeSaveData = 0;
            //    }

            //    PlayerPrefs.SetInt("SurvivalMode", survivalModeSaveData);

            //    if (stepCount <= 0)
            //    {
            //        AddNewGridValue();
            //        AddNewGridValue();

            //        if (survivalMode)
            //        {
            //            AddPlayer();
            //        }
            //    }

            //    stepCount++;
            //    PlayerPrefs.SetInt("StepCount", stepCount);
            //    SaveGrid();

            //    StartCoroutine("PlayGameCoroutine");
            //}

        }
        public void Resetbutt()
        {
            survivalmodeend = 0;
            PlayerPrefs.SetInt("survmodeend", survivalmodeend);
            Level = 1;
            PlayerPrefs.SetInt("level", Level);
            wins = 50;
            PlayerPrefs.SetInt("wins", wins);
            for (int y = 4; y < currenttrophy; y++)
            {
                uiTrophy[y].GetComponent<ZTrophy>().SetProgress(0, 0, trophyUnlockScore);
            }

            currenttrophy = 4;
            PlayerPrefs.SetInt("troph", currenttrophy);
            //for (int y = 0; y < currenttrophy; y++)
            //{
            //    uiTrophy[y].GetComponent<ZTrophy>().SetProgress(0, 0, trophyUnlockScore);
            //}

            panelWin.SetActive(false);
            txtsurvival[5].SetActive(true);
            ResetScore();
            UpdateScore();
            AddPlayer();
        }
        public void ContinueGame()
        {
            PlayGame(survivalMode);
        }
        public void survivbutt()
        {

            //score = 0;
            //PlayerPrefs.SetInt("score", score);

            survivalmode = 1;
            PlayerPrefs.SetInt("survivalmode", survivalmode);
            if (survivalmodeend == 0)
            {
                foreach (GameObject go in txtsurvival)
                {
                    go.SetActive(true);
                }
                foreach (GameObject be in txtbest)
                {
                    be.SetActive(false);
                }
            }
            UpdateScore();
        }
        public void normalbutt()
        {
            survivalmode = 0;
            score = 0;
            PlayerPrefs.SetInt("score", score);
            PlayerPrefs.SetInt("survivalmode", survivalmode);
            foreach (GameObject go in txtsurvival)
            {
                go.SetActive(false);
            }
            foreach (GameObject be in txtbest)
            {
                be.SetActive(true);
            }
            UpdateScore();
        }

        public IEnumerator PlayGameCoroutine()
        {
            yield return new WaitForSeconds(1);
            canReceiveTouchInput = true;
        }
        #endregion

        #region Achievement Progression
        public Sprite virus;
        public Sprite doctor;
        private void IniteTrophy()
        {
            uiTrophy = new GameObject[data_Trophy.Length];

            for (int i = 0; i < data_Trophy.Length; i++)
            {
                int _trophyScore = 0;

                if (PlayerPrefs.HasKey("trophy" + i.ToString("000")))
                {
                    _trophyScore = PlayerPrefs.GetInt("trophy" + i.ToString("000"));
                }
                else
                {
                    PlayerPrefs.SetInt("trophy" + i.ToString("000"), _trophyScore);
                }

                GameObject _trophy = Instantiate(uiTrophyItem, transTrophyContent);
                _trophy.name = "trophy" + i.ToString("000");
                _trophy.GetComponent<ZTrophy>().SetupTrophyProgress(_trophyScore, trophyUnlockScore);
                if(((i+1)%5)==0&& i>0)
                {
                    _trophy.GetComponent<ZTrophy>().trophySprite.sprite = virus;
                }
                if( i==39)
                {
                    _trophy.GetComponent<ZTrophy>().trophySprite.sprite = doctor;
                }
                _trophy.GetComponent<ZTrophy>().SetNumber(i + 1);
                uiTrophy[i] = _trophy;

                if (_trophyScore >= trophyUnlockScore)
                {
                    _trophy.GetComponent<ZTrophy>().UnLocked();
                }
                else
                {
                    _trophy.GetComponent<ZTrophy>().Locked();
                }
            }
        }

        public void ShowTrophy()
        {
            gameState = GameState.progression;

            #region UI
            panelProgression.SetActive(true);

            if (currenttrophy % 5 == 0)
            {
                int index = 0;
                if (currenttrophy == 5) index = 0;
                if (currenttrophy == 10) index = 1;
                if (currenttrophy == 15) index = 2;
                if (currenttrophy == 20) index = 3;
                if (currenttrophy == 25) index = 4;
                if (currenttrophy == 30) index = 5;
                

            }
            SoundManager.Instance.SetBackgroundLevel(currenttrophy);
                this.Invoke(() => { ShowMAp.Instance.ShowMap((currenttrophy 
                    )); }, 2f);
            titleTrophy.SetActive(true);
            titleGameOver.SetActive(false);
            #endregion
        }

        public void BackToGame()
        {
            panelProgression.SetActive(false);
            panelProgressionnormal.SetActive(false);
            panelWin.SetActive(false);
            mainMenuUIManager.settingsPanel.SetActive(false);
            StartCoroutine(startnewgame());
            if (stepCount > 0)
            {
                gameState = GameState.inGame;
                panelBottomUI.SetActive(true);
                panelMainMenu.SetActive(false);
            }
            else
            {
                gameState = GameState.mainMenu;
                panelBottomUI.SetActive(false);
                panelMainMenu.SetActive(true);
            }

        }

        IEnumerator ShowGameProgression()
        {
            #region UI
            btnBackToGame.SetActive(false);
            #endregion

            for (int i = 0; i < data_Trophy.Length; i++)
            {
                int _trophyScore = 0;
                _trophyScore = PlayerPrefs.GetInt("trophy" + i.ToString("000"));

                if (_trophyScore < trophyUnlockScore && score > 0)
                {
                    #region Update trophy progress
                    int _remainingScore = trophyUnlockScore - _trophyScore;
                    int _toScore = 0;
                    bool _unlocked = false;

                    if (score - _remainingScore >= 0)
                    {
                        _toScore = trophyUnlockScore;
                        score -= _remainingScore;
                        _unlocked = true;
                    }

                    else
                    {
                        _toScore = _trophyScore + score;

                        score = 0;
                    }

                    uiTrophy[i].GetComponent<ZTrophy>().SetProgress(_trophyScore, _toScore, trophyUnlockScore);
                    //GameObject.Find("trophy" + i.ToString("000")).GetComponent<ZTrophy>,().SetProgress(_trophyScore, _toScore, trophyUnlockScore);
                    PlayerPrefs.SetInt("trophy" + i.ToString("000"), _toScore);
                    #endregion

                    yield return new WaitForSeconds(0.35f);

                    #region Sfx
                    if (_unlocked)
                    {
                        // PlaySound(sfxUnlock, 1);
                    }

                    if (score == 0)
                    {
                        soundManager.PlayGameOverMusic();
                    }
                    #endregion

                    yield return new WaitForSeconds(0.15f);
                }
            }

            #region UI
            btnBackToGame.SetActive(true);
            #endregion
        }

        private void UpdateScore()
        {
            #region UI
            txtCurrentScore[0].text = "SCORE:" + score.ToString();
            txtCurrentScore[1].text = "SCORE:" + score.ToString();
            txtCurrentScore[2].text = "Wins:" + wins.ToString();
            txtCurrentScore[3].text = "Wins:" + wins.ToString();
            if (Level > 0 && (Level % 5 == 0))
            {
                txtCurrentScore[5].color=Color.red;
            }
            else
            {
                txtCurrentScore[5].color=new Color32(103,224,255,1);
            }
            txtCurrentScore[4].text = "Level:" + Level.ToString();
            txtCurrentScore[5].text = "Level:" + Level.ToString();
            txtCurrentScore[6].text = "Best:" + bestscore.ToString();
            txtCurrentScore[7].text = "Best:" + bestscore.ToString();


            levelbar.value = currentlevelbarvalue;
            #endregion
        }
        #endregion

        #region Game Over + Restart
        public void RestartGame()
        {
            StartCoroutine(startnewgame());
            stepCount = 0;
            score = 0;
            PlayerPrefs.SetInt("Score", score);
            PlayerPrefs.SetInt("StepCount", stepCount);
            ResetScore();
            ResetGrid();
            UpdateScore();
            gameState = GameState.mainMenu;

            #region UI
            panelBottomUI.SetActive(false);
            panelMainMenu.SetActive(true);
            panelContinueGame.SetActive(false);
            #endregion
        }
        IEnumerator startnewgame()
        {
            for (int x = 0; x < norm_surv.Length; x++)
            {
                norm_surv[x].interactable = false;
            }
            yield return new WaitForSeconds(startnewgametime);
            for (int x = 0; x < norm_surv.Length; x++)
            {
                norm_surv[x].interactable = true;
            }

        }
        public void GotoProgression()
        {
            gameOverUndoTimerStarted = false;
            GameOver();
        }

        public void GameOver()
        {
            soundManager.PlayGameOverMusic();
            txtCurrentScore[8].text = "Best:" + bestscore.ToString();
            txtCurrentScore[9].text = "Best:" + bestscore.ToString();
            txtCurrentScore[10].text = "SCORE:" + score.ToString();
            txtCurrentScore[11].text = "SCORE:" + score.ToString();

            if (!gameOverUndoUsed)
            {
                showInterstitial = true;
                gameOverUndoUsed = true;
                gameOverUndoPanel.SetActive(true);
                panelBottomUI.SetActive(false);
                canReceiveTouchInput = false;
                remainingTimer = timer;
                gameOverUndoTimerStarted = true;
                return;
            }
            else if(gameOverUndoTimerStarted)
            {
                return;
            }


            if (showInterstitial)
            {
                if(Session.Instance.ad>=2)
                {

                    Session.Instance.ad = 0;
                            mobileAd.ShowInteristitialAd();
                     showInterstitial = false;
                }
                else
                {
                    Session.Instance.ad++;
                }
            }
            gameOverUndoPanel.SetActive(false);
            gameState = GameState.progression;
            stepCount = 0;
            PlayerPrefs.SetInt("StepCount", stepCount);

            ResetGrid();
            if (survivalmode == 1)
            {
                StartCoroutine("ShowGameProgression");
            }


            #region UI
            if (survivalmode == 1)
            {
                panelProgression.SetActive(true);
            }
            else
            {
                panelProgressionnormal.SetActive(true);
            }
            panelMainMenu.SetActive(false);
            panelContinueGame.SetActive(false);
            titleTrophy.SetActive(false);
            titleGameOver.SetActive(true);
            #endregion
        }
        #endregion
        #endregion

        #region Sound FX Control
        private float GetPitchValue(int _gridValue)
        {
            float _pitch = 1;

            switch (_gridValue)
            {
                case 4:
                    _pitch = 1.01f;
                    break;
                case 8:
                    _pitch = 1.02f;
                    break;
                case 16:
                    _pitch = 1.03f;
                    break;
                case 32:
                    _pitch = 1.04f;
                    break;
                case 64:
                    _pitch = 1.05f;
                    break;
                case 128:
                    _pitch = 1.06f;
                    break;
                case 256:
                    _pitch = 1.07f;
                    break;
                case 512:
                    _pitch = 1.08f;
                    break;
                case 1024:
                    _pitch = 1.09f;
                    break;
                case 2048:
                    _pitch = 1.10f;
                    break;
                default:
                    break;
            }

            return _pitch;
        }
        #endregion

        #region Database Management (where you'll want to enhance for your game base on your game design)
        private void AddScore(int _score)
        {
            score += _score;
            PlayerPrefs.SetInt("Score", score);
            updatelevel();

            UpdateScore();
        }

        private void ResetScore()
        {
            if (survivalmode == 1)
            {
                score = 0;
            }

            //wins = 50;
            //Level = 1;
            levelbar.value = 0;
            //levelbar.maxValue = 0;
            currentlevelbarvalue = 0;
            //maxlevelbarvalue = 0;

            PlayerPrefs.SetInt("currentlvl", currentlevelbarvalue);
            //PlayerPrefs.SetInt("maxlevelbar", maxlevelbarvalue);
            PlayerPrefs.SetInt("Score", score);
            //PlayerPrefs.SetInt("wins", wins);
            //PlayerPrefs.SetInt("level", Level);
            UpdateScore();
        }

        private void LoadScore()
        {
            wins = PlayerPrefs.GetInt("wins");
            Level = PlayerPrefs.GetInt("level");
            score = PlayerPrefs.GetInt("Score");
            maxlevelbarvalue = PlayerPrefs.GetInt("maxlevelbar");
            currentlevelbarvalue = PlayerPrefs.GetInt("currentlvl");
            UpdateScore();
        }
        #endregion

        #region Debug Codes
        private void AssignGridTextMesh()
        {
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    textMesh_GridValue[x, y] = GameObject.Find("tm" + x.ToString() + "-" + y.ToString()).GetComponent<TextMesh>();
                }
            }
        }

        private void UpdateGrigDebugValue()
        {
            if (DebugMode)
            {
                for (int x = 0; x < 4; x++)
                {
                    for (int y = 0; y < 4; y++)
                    {
                        if (gridValue[x, y] > 0)
                        {
                            textMesh_GridValue[x, y].text = gridValue[x, y].ToString();
                        }
                        else
                        {
                            textMesh_GridValue[x, y].text = "";
                        }
                    }
                }
            }
        }
        #endregion
    }
}
