using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dev.Scripts.Levels
{
    public class MapController : MonoBehaviour
    {
        #region Events

        public static event EventHandler<LevelReachedEventArgs> LevelSelected;
        public static event EventHandler<LevelReachedEventArgs> LevelReached;

        #endregion
        public static MapController instance;
        private static IMapProgressManager _mapProgressManager = new PlayerPrefsMapProgressManager();

        #region Properties

        public bool isGenerated;
        public LevelMapNode levelNodePrefab;
        public Transform characterPrefab;
        public WaypointsMover waypointsMover;
        public LevelMapNode characterLevel;
        
        public bool starsEnabled;

        public bool scrollingEnabled;
        public MapCamera mapCamera;

        public bool isClickEnabled;
        public bool isConfirmationEnabled;
        #endregion

        private void Awake()
        {
            instance = this;
        }

        private void OnDestroy()
        {
            instance = null;
        }

        private void OnEnable()
        {
            if (isGenerated)
            {
                Reset();
            }
        }

        public List<LevelMapNode> GetLevelMapItems() =>
            FindObjectsOfType<LevelMapNode>().OrderBy(item => item.number).ToList();

        public void Reset()
        {
            UpdateMapLevels();
            PlaceCharacterToLastUnlockedLevel();
            SetCameraToCharacter();
        }

        private void UpdateMapLevels()
        {
            /*foreach (var levelMapItem in GetLevelMapItems())
            {
                levelMapItem.UpdateState(_mapProgressManager.LoadLevelStarsCount(levelMapItem.number),
                    IsLevelLocked(levelMapItem.number));
            }*/
        }

        private void PlaceCharacterToLastUnlockedLevel()
        {
            var lastUnlockedLevel = GetLevelMapItems().Where(l => !l.isLocked).Select(l => l.number).Max();
            //TeleportToLevelInternal(lastUnlockedLevel, true);
        }

        private void SetCameraToCharacter()
        {
            /*MapCamera mapCamera = FindObjectOfType<MapCamera>();
            if (mapCamera!=null)
            {
                mapCamera.SetPosition(waypointsMover.transform.position);
            }*/
        }

        #region Static API

        public static void CompleteLevel(int levelIndex)
        {
            CompleteLevelInternal(levelIndex, 1);
        }

        public static void CompleteLevel(int levelIndex, int starsCount)
        {
            CompleteLevelInternal(levelIndex, starsCount);
        }

        internal static void OnLevelSelected(int levelIndex)
        {
            if (LevelSelected !=null && !IsLevelLocked(levelIndex))
            {
                LevelSelected(instance, new LevelReachedEventArgs(levelIndex));
            }

            if (!instance.isConfirmationEnabled)
            {
                GoToLevel(levelIndex);
            }
        }

        public static void GoToLevel(int levelIndex)
        {
            instance.WalkToLevelInternal (levelIndex);
        }

        public static bool IsLevelLocked(int levelIndex) =>
            levelIndex > 1 && _mapProgressManager.LoadLevelStarsCount(levelIndex - 1) == 0;

        public static void ClearAllProgress () {
            instance.ClearAllProgressInternal ();
        }
        
        public static bool IsStarsEnabled () {
            return instance.starsEnabled;
        }

        public static bool GetIsClickEnabled () {
            return instance.isClickEnabled;
        }

        public static bool GetIsConfirmationEnabled () {
            return instance.isConfirmationEnabled;
        }
        #endregion

        private static void CompleteLevelInternal(int levelIndex, int starsCount)
        {
            if (IsLevelLocked(levelIndex))
            {
                Debug.Log($"Cant complete locked level {levelIndex}");
            }
            else if (starsCount<1 || starsCount>3)
            {
                Debug.Log($"Cant complete level . Invalid stars count {levelIndex}");
            }
            else
            {
                int currentStarsCount = _mapProgressManager.LoadLevelStarsCount(levelIndex);
                int maxStarsCount = Mathf.Max(currentStarsCount, starsCount);
                _mapProgressManager.SaveLevelStarsCount(levelIndex,maxStarsCount);

                if (instance!=null)
                {
                    instance.UpdateMapLevels();
                }
            }
        }

        private void WalkToLevelInternal(int levelIndex)
        {
            LevelMapNode levelMapNode = GetLevel(levelIndex);
            if (levelMapNode.isLocked)
            {
                Debug.Log($"Cant go to locked level number{levelIndex}");
            }
            else
            {
                waypointsMover.Move(characterLevel.pathTransform,levelMapNode.pathTransform, () =>
                {
                    RaiseLevelReached(levelIndex);
                    characterLevel = levelMapNode;
                });
            }
        }

        private void RaiseLevelReached(int levelIndex)
        {
            LevelMapNode levelMapNode = GetLevel(levelIndex);
            if (!string.IsNullOrEmpty(levelMapNode.sceneName))
            {
                SceneManager.LoadScene(levelMapNode.sceneName);
            }

            if (LevelReached !=null)
            {
                LevelReached(this, new LevelReachedEventArgs(levelIndex));
            }
        }

        public LevelMapNode GetLevel(int levelIndex) =>
            GetLevelMapItems().SingleOrDefault(item => item.number == levelIndex);

        private void ClearAllProgressInternal()
        {
            foreach (var levelMapItem in GetLevelMapItems())
            {
                _mapProgressManager.ClearLevelProgress(levelMapItem.number);
            }
        }

    }
}