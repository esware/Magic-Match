using System;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

namespace Dev.Scripts.Levels
{
    public class LevelMapNode : MonoBehaviour
    {
        #region Variables

        #region Private Variables

        private Vector3 _originalScale;
        private bool _isScaled;
        private float _scaleValue;

        #endregion

        #region Public Variables

        public int number;
        public int starsCount;
        public bool isLocked;
        public Transform lockedObject;
        public Transform pathTransform;
        public GameObject levelScene;
        public string sceneName;
        public Transform[] stars;

        #endregion
        

        #endregion

        private void Awake()
        {
            _originalScale = transform.localScale;
        }

        #region Enable click

        public void OnMouseEnter()
        {
            // TODO if clicked level item play over scale animation 
            
        }

        public void OnMouseDown()
        {
            // TODO item clickScale animation play 
        }

        public void OnMouseExit()
        {
            //TODO item scale reset 
            /*if (LevelsMap.GetIsClickEnabled())
                ResetScale();*/
        }
        public void OnDisable()
        {
            //TODO item scale reset 
            /*if (LevelsMap.GetIsClickEnabled())
                ResetScale();*/
            
        }

        public void OnMouseUpAsButton()
        {
            //TODO item scale reset 
            /*if (LevelsMap.GetIsClickEnabled())
            {
                ResetScale();
                LevelsMap.OnLevelSelected(Number);
            }*/
        }
        #endregion
        
        private void Scale()
        {
            transform.localScale = _originalScale * _scaleValue;
            _isScaled = true;
        }
        private void ResetScale()
        {
            if (_isScaled)
                transform.localScale = _originalScale;
        }

        public void UpdateState(int starsCount,bool isLocked)
        {
            this.starsCount = starsCount;
            UpdateStars(starsCount);
            this.isLocked = isLocked;
            lockedObject.gameObject.SetActive(isLocked);
        }

        public void UpdateStars(int starsCount)
        {
            if (starsCount<1)
                return;

            int starCounter = 0;
            foreach (var star in stars)
            {
                starCounter++;
                star.gameObject.SetActive(starCounter<=starsCount);
            }
        }
    }
}