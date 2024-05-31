using System;
using Dev.Scripts.GUI;
using Dev.Scripts.Targets;
using UnityEngine;
using UnityEngine.UI;

namespace GameStates
{
    public class PrepareGame:GameState
    {
        [Header("Boost Icons")]
        public BoostIcon activatedBoost;
        public BoostIcon emptyBoostIcon;
        
        public int targetBlocks;
        public static int Score;
        public int stars;
        public int star1;
        public int star2;
        public int star3;
        
        [Header("Block Objects")]
        public GameObject ingrObject;
        public GameObject blocksObject;
        public GameObject scoreTargetObject;
        
        public GameObject star1Anim;
        public GameObject star2Anim;
        public GameObject star3Anim;
        
        public GameObject level;
        public GameObject levelsMap;
        
        public Transform gameField;
        
        public Vector2 firstSquarePosition;
        public Square[] squaresArray;
        
        public int[] ingrCountTarget = new int[2];
        public CollectItems[] collectItems = new CollectItems[2];
        public Ingredients[] ingrTarget = new Ingredients[2];
        SquareBlocks[] levelSquaresFile = new SquareBlocks[81];
        
        public int currentLevel = 1;
        public int passLevelCounter;

        public override void EnterState()
        {
            passLevelCounter++;

            MusicBase.Instance.GetComponent<AudioSource>().Stop();
            MusicBase.Instance.GetComponent<AudioSource>().loop = true;
            MusicBase.Instance.GetComponent<AudioSource>().clip = MusicBase.Instance.music[1];
            MusicBase.Instance.GetComponent<AudioSource>().Play();

            SetupGame();
        }

        public override void UpdateState()
        {
            
        }

        public override void ExitState()
        {
            
        }
        
        public void LoadLevel()
        {
            currentLevel = PlayerPrefs.GetInt("OpenLevel");
            if (currentLevel == 0)
                currentLevel = 1;
            LoadDataFromLocal(currentLevel);

        }
        public void LoadDataFromLocal(int currentLevel)
        {
            TextAsset mapText = Resources.Load("Levels/" + currentLevel) as TextAsset;
            if (mapText == null)
            {
                mapText = Resources.Load("Levels/" + currentLevel) as TextAsset;
            }
            ProcessGameDataFromString(mapText.text);
        }
        void ProcessGameDataFromString(string mapText)
        {
            var levelManager = LevelManager.Instance;
        string[] lines = mapText.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

        int mapLine = 0;
        foreach (string line in lines)
        {
            //check if line is game mode line
            if (line.StartsWith("MODE"))
            {
                //Replace GM to get mode number, 
                string modeString = line.Replace("MODE", string.Empty).Trim();
                //then parse it to interger
                LevelManager.Instance.target = (Target)int.Parse(modeString);
                //Assign game mode
            }
            else if (line.StartsWith("SIZE "))
            {
                string blocksString = line.Replace("SIZE", string.Empty).Trim();
                string[] sizes = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                levelManager.maxCols = int.Parse(sizes[0]);
                levelManager.maxRows = int.Parse(sizes[1]);
                squaresArray = new Square[levelManager.maxCols * levelManager.maxRows];
                levelSquaresFile = new SquareBlocks[levelManager.maxRows * levelManager.maxCols];
                for (int i = 0; i < levelSquaresFile.Length; i++)
                {

                    SquareBlocks sqBlocks = new SquareBlocks();
                    sqBlocks.Block = SquareTypes.EMPTY;
                    sqBlocks.Obstacle = SquareTypes.NONE;

                    levelSquaresFile[i] = sqBlocks;
                }
            }
            else if (line.StartsWith("LIMIT"))
            {
                string blocksString = line.Replace("LIMIT", string.Empty).Trim();
                string[] sizes = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                levelManager.limitType = (LIMIT)int.Parse(sizes[0]);
                levelManager.limit = int.Parse(sizes[1]);
            }
            else if (line.StartsWith("COLOR LIMIT "))
            {
                string blocksString = line.Replace("COLOR LIMIT", string.Empty).Trim();
                levelManager.colorLimit = int.Parse(blocksString);
            }
            
            else if (line.StartsWith("STARS"))
            {
                string blocksString = line.Replace("STARS", string.Empty).Trim();
                string[] blocksNumbers = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                star1 = int.Parse(blocksNumbers[0]);
                star2 = int.Parse(blocksNumbers[1]);
                star3 = int.Parse(blocksNumbers[2]);
                if (ProgressBarScript.Instance != null)//2.1.2
                    ProgressBarScript.Instance.InitBar();//2.1.2
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
                    if (levelManager.target == Target.INGREDIENT)
                        ingrTarget[i] = (Ingredients)int.Parse(blocksNumbers[i]);
                    else if (levelManager.target == Target.COLLECT)
                        collectItems[i] = (CollectItems)int.Parse(blocksNumbers[i]);

                }
            }
            else
            {
                string[] st = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < st.Length; i++)
                {
                    levelSquaresFile[mapLine * levelManager.maxCols + i].Block = (SquareTypes)int.Parse(st[i][0].ToString());
                    levelSquaresFile[mapLine * levelManager.maxCols + i].Obstacle = (SquareTypes)int.Parse(st[i][1].ToString());
                }
                mapLine++;
            }

            var tar = Resources.Load<TargetLevel>("Targets/Level" + currentLevel);
            levelManager.targetObject = new TargetObject[tar.targets.Length];
            for (var index = 0; index < tar.targets.Count; index++)
            {
                var tarTarget = tar.targets[index];
                levelManager.targetObject[index] = tarTarget.DeepCopy();
            }
        }
        //levelLoaded = true;
    }
        
        private void SetupGame()
        {
            var levelManager = LevelManager.Instance;
            levelManager.ActivatedBoost = null;
            Score = 0;
            stars = 0;


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

            levelManager.TargetBlocks = 0;
            levelManager.EnableMap(false);


            gameField.transform.position = Vector3.zero;
            firstSquarePosition = gameField.transform.position;

            squaresArray = new Square[levelManager.maxCols *levelManager.maxRows];
            LoadLevel();
            for (int row = 0; row < levelManager.maxRows; row++)
            {
                for (int col = 0; col < levelManager.maxCols; col++)
                {
                    if (levelSquaresFile[row * levelManager.maxCols + col].Block == SquareTypes.BLOCK)
                        levelManager.TargetBlocks++;
                    else if (levelSquaresFile[row * levelManager.maxCols + col].Block == SquareTypes.DOUBLEBLOCK)
                        levelManager.TargetBlocks += 2;
                }
            }
            
            GameObject.Find("Canvas").transform.Find("PrePlay").gameObject.SetActive(true);
            if (levelManager.limitType == LIMIT.MOVES)
            {
                levelManager.InGameBoosts[0].gameObject.SetActive(true);
                levelManager.InGameBoosts[1].gameObject.SetActive(false);
            }
            else
            {
                levelManager.InGameBoosts[0].gameObject.SetActive(false);
                levelManager.InGameBoosts[1].gameObject.SetActive(true);

            }
            //TODO Change State & Invoke Event
            GameEvents.OnEnterGame?.Invoke();
        }
        
    }
}