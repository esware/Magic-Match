using System.Collections;
using System.Linq;
using GameStates;
using UnityEngine;

namespace Dev.Scripts.System
{
    public class AspectCamera : MonoBehaviour
    {
        public Sprite map;
        public RectTransform topPanel;
        private Rect fieldRect;
        float fieldHeight;
        private float fielgWidth;
        private float fraq;

        private Camera _mainCam;

        private void OnEnable()
        {
            GameEvents.OnMapState += UpdateAspect;
            GameEvents.OnEnterGame += UpdateAspect;

            _mainCam = GetComponent<Camera>();
        }

        private void OnDisable()
        {
            GameEvents.OnMapState -= UpdateAspect;
            GameEvents.OnEnterGame -= UpdateAspect;
        }

        void UpdateAspect()
        {
            StartCoroutine(Wait());

        }

        IEnumerator Wait()
        {
            yield return new WaitWhile(() => !GameManager.Instance);
            if (!GameManager.Instance.GetState<Map>())
            {
                yield return new WaitWhile(() => GameManager.Instance.GetItems().Count == 0);
                var items = GameManager.Instance.GetItems().Where(i => i != null).Where(i => i != null);
                float topY = items.Max(i => i.transform.position.y);
                float bottomY = items.Min(i => i.transform.position.y);
                float leftX = items.Min(i => i.transform.position.x);
                float rightX = items.Max(i => i.transform.position.x);

                fieldHeight = topY - bottomY;
                fielgWidth = rightX - leftX;
                fieldRect = new Rect(leftX, topY, fielgWidth, fieldHeight);
                fraq = (fielgWidth > fieldHeight ? fielgWidth : fieldHeight);
                int width = Screen.width;
                int height = Screen.height;
                float v = fraq / width * (height - 300);
                var h = fieldRect.width * Screen.height / Screen.width / 2 + 1.5f;
                var w = (fieldRect.height + 2.5f * 2) / 2 + 2f;
                var maxLength = Mathf.Max(h, w);
                _mainCam.orthographicSize = Mathf.Clamp(maxLength, 4, maxLength);
            }
            else
                _mainCam.orthographicSize = 8f / Screen.width * Screen.height / 2f;
        }

    }
}