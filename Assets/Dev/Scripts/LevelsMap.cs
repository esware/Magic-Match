using UnityEngine;

namespace Dev.Scripts
{
    public class LevelsMap:MonoBehaviour
    {
        private static LevelsMap _instance;
        private static IMapProgressManager _mapProgressManager = new PlayerPrefsMapProgressManager();
        public static LevelsMap Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<LevelsMap>();
                    if (_instance == null)
                    {
                        var go = new GameObject("LevelsMap");
                        _instance = go.AddComponent<LevelsMap>();
                    }
                }

                return _instance;
            }
        }

        public bool isGenerated;
        public MapLevel mapLevelPrefab;
        public Transform characterPrefab;
        public int count = 10;
        
        public WaypointsMover waypointsMover;
        public MapLevel characterLevel;




    }
    
}