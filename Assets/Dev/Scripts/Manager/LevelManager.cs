using System.Collections.Generic;
using UnityEngine;

namespace Dev.Scripts.Manager
{
    public class SquareBlocks
    {
        public SquareTypes block;
        public SquareTypes obstacle;

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
    
    public class LevelManager:MonoBehaviour
    {
        
        [Header("Singleton instances")]
        public static LevelManager THIS;
        public static LevelManager Instance;

        [Header("Prefabs")]
        public GameObject itemPrefab;
        public GameObject squarePrefab;

        [Header("Sprites")]
        public Sprite squareSprite;
        public Sprite squareSprite1;
        public Sprite outline1;
        public Sprite outline2;
        public Sprite outline3;

        [Header("Block prefabs")]
        public GameObject blockPrefab;
        public GameObject wireBlockPrefab;
        public GameObject solidBlockPrefab;
        public GameObject undesroyableBlockPrefab;
        public GameObject thrivingBlockPrefab;

        [Header("Game field")]
        public Transform GameField;

        [Header("In-app purchases")]
        public bool enableInApps;

        [Header("Grid size")]
        public int maxRows = 9;
        public int maxCols = 9;

        [Header("Square size")]
        public float squareWidth = 1.2f;
        public float squareHeight = 1.2f;

        [Header("First square position")]
        public Vector2 firstSquarePosition;

        [Header("Squares array")]
        public Square[] squaresArray;

        [Header("Combined items")]
        List<List<Item>> combinedItems = new List<List<Item>>();

        [Header("Last dragged and switched items")]
        public Item lastDraggedItem;
        public Item lastSwitchedItem;

        [Header("Items to destroy")]
        public List<Item> destroyAnyway = new List<Item>();

        [Header("Score popup")]
        public GameObject popupScore;

        [Header("Scores")]
        public int scoreForItem = 10;
        public int scoreForBlock = 100;
        public int scoreForWireBlock = 100;
        public int scoreForSolidBlock = 100;
        public int scoreForThrivingBlock = 100;

        [Header("Game limit")]
        public LIMIT limitType;
        public int Limit = 30;

        [Header("Target score")]
        public int TargetScore = 1000;

        [Header("Current level")]
        public int currentLevel = 1;

        [Header("Failed cost")]
        public int FailedCost;

        [Header("Extra failed moves and seconds")]
        public int ExtraFailedMoves = 5;
        public int ExtraFailedSecs = 30;

        [Header("In-app purchase IDs")]
        public string[] InAppIDs;

        [Header("Google license key")]
        public string GoogleLicenseKey;

        [Header("Line renderer")]
        LineRenderer line;

        [Header("Thriving block destroyed flag")]
        public bool thrivingBlockDestroyed;

        [Header("New combines")]
        List<List<Item>> newCombines;

        [Header("Drag blocked flag")]
        private bool dragBlocked;
        

    }
}