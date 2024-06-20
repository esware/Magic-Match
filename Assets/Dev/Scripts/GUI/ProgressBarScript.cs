﻿
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Dev.Scripts.GUI
{
    public class ProgressBarScript : MonoBehaviour 
    {
        Image slider;
        public static ProgressBarScript Instance;
        float maxWidth;
        public GameObject[] stars;
        void OnEnable () {
            Instance = this;
            slider = GetComponent<Image> ();
            maxWidth = 1;
            InitBar ();
        }

        public void InitBar () 
        {
            ResetBar ();
            PrepareStars ();

        }

        public void UpdateDisplay (float x) {
            slider.fillAmount = maxWidth * x;
            if (maxWidth * x >= maxWidth) {
                slider.fillAmount = maxWidth;

                //	ResetBar();
            }
        }

        public void AddValue (float x) {
            UpdateDisplay (slider.fillAmount * 100 / maxWidth / 100 + x);
        }
        
        public bool IsFull () {
            if (slider.fillAmount >= maxWidth) { 
                ResetBar ();
                return true;
            } else
                return false;
        }

        public void ResetBar () {
            UpdateDisplay (0.0f);
        }

        void PrepareStars () {
            if (GameManager.Instance != null) {
                float width = GetComponent<RectTransform> ().rect.width;
                stars [0].transform.localPosition = new Vector3 (GameManager.Instance.star1 * 100 / GameManager.Instance.star3 * width / 100 - (width / 2f), stars [0].transform.localPosition.y, 0);
                stars [1].transform.localPosition = new Vector3 (GameManager.Instance.star2 * 100 / GameManager.Instance.star3 * width / 100 - (width / 2f), stars [1].transform.localPosition.y, 0);
                stars [0].transform.GetChild (0).gameObject.SetActive (false);
                stars [1].transform.GetChild (0).gameObject.SetActive (false);
                stars [2].transform.GetChild (0).gameObject.SetActive (false);
            }
        }

    }

}