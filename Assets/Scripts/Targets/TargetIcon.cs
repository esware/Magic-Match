using System.Linq;
using Dev.Scripts.Targets;
using GameStates;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TargetIcon : MonoBehaviour
    {
        public Image imageRenderer;
        public TextMeshProUGUI textObj;
        public int count;
        public Sprite[] checkUncheck;
        public Image checkObject;
        private TargetObject _targetObject;

        public TargetObject TargetObj
        {
            get
            {
                if (_targetObject == null)
                {
                    if (GameManager.Instance != null)
                        _targetObject = GameManager.Instance.targetObject.First(i => i.icon.name == imageRenderer.sprite.name);
                }
                return _targetObject;
            }
            set => _targetObject = value;
        }

        public void SetTarget(TargetObject t)
        {
            checkObject.gameObject.SetActive(false);
            imageRenderer.sprite = t.icon;
            count = t.targetCount;
            textObj.text = t.targetCount.ToString();
            if (t.type == Target.SCORE)
            {
                count = GameManager.Instance.star1;
                textObj.text = "1";
            }
            else if(t.type == Target.BLOCKS)
            {
                textObj.GetComponent<TargetText>().TextUpdate = GetCount;
            }
            else if(t.type == Target.INGREDIENT)
            {
                textObj.GetComponent<TargetText>().TextUpdate = GetCount;
            }
            else if(t.type == Target.COLLECT)
            {
                textObj.GetComponent<TargetText>().TextUpdate = GetCount;
            }
        }

        private void Update()
        {
            if(TargetObj?.Done() ?? false) SetCheck();
            else if(GameManager.Instance.GetState<PreFailed>() || GameManager.Instance.GetState<GameOver>()) SetFailed();
            else if((!TargetObj?.Done() ?? false) && checkObject.gameObject.activeSelf) SetContinue();
        }

        void SetCheck()
        {
            checkObject.sprite = checkUncheck[0];
            checkObject.gameObject.SetActive(true);
            textObj.gameObject.SetActive(false);
        }
        
        void SetFailed()
        {
            checkObject.sprite = checkUncheck[1];
            checkObject.gameObject.SetActive(true);
            textObj.gameObject.SetActive(false);
        }
        
        void SetContinue()
        {
            checkObject.gameObject.SetActive(false);
            textObj.gameObject.SetActive(true);
        }
        
        string GetBlocks() => GameManager.Instance.TargetBlocks.ToString();

        string GetCount() => TargetObj?.GetCount().ToString();

        int GetScoreTarget() => count - GameManager.Score;
    }