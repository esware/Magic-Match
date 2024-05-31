using System.Collections.Generic;
using GameStates;
using UnityEngine;

namespace Dev.Scripts.Targets
{
    public class TargetsGroup : MonoBehaviour
    {
        public TargetIcon prefab;
        private int _levelNum;
        private readonly List<TargetIcon> _targetIcons = new List<TargetIcon>();
        private void OnEnable()
        {
            _levelNum = PlayerPrefs.GetInt("OpenLevel");
            var targetObj = Resources.Load<TargetLevel>("Targets/Level" + _levelNum);
            foreach (var target in targetObj.targets)
            {
                Debug.Log(target);
                var obj = Instantiate<TargetIcon>(prefab, transform);
                obj.SetTarget(target);
                _targetIcons.Add(obj);
                target.guiObj = obj;
            }
        }

        private void OnDisable()
        {
            foreach (var child in _targetIcons)
            {
                Destroy(child.gameObject);
            }
            _targetIcons.Clear();
        }
    }
}