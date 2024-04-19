using System;
using UnityEngine;
using Object = UnityEngine.Object;
using Vector2 = System.Numerics.Vector2;

namespace Dev.Scripts
{
    public class MapLevel:MonoBehaviour
    {
        private Vector3 _originalScale;
        private bool _isScaled;
        
        public float overScale = 1.05f;
        public float clickScale = 0.95f;
        public int number;
        public bool isLocked;
        public Transform @lock;
        public Transform pathPivot;
        public Object levelScene;
        public string sceneName;
        public int starsCount;
        public Transform starsHoster;
        public Transform star1;
        public Transform star2;
        public Transform star3;

        private void Awake()
        {
            _originalScale = transform.localScale;
        }

        public void UpdateState(int starsCount,bool isLocked)
        {
            this.starsCount = starsCount;
            this.isLocked = isLocked;
            UpdateStars(starsCount);
            @lock.gameObject.SetActive(isLocked);
        }
        public void UpdateStars(int starsCount)
        {
            star1.gameObject.SetActive(starsCount >= 1);
            star2.gameObject.SetActive(starsCount >= 2);
            star3.gameObject.SetActive(starsCount >= 3);
        }
    }
}