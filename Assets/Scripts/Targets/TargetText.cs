using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dev.Scripts.Targets
{
    public class TargetText : MonoBehaviour
    {
        public TextMeshProUGUI textObject;
        public Action<string> updateText;
        public delegate string del();
        public del TextUpdate;
        private void Update()
        {
            if (TextUpdate != null) textObject.text = TextUpdate();
        }
    }
}