using System;
using UnityEngine;

namespace Dev.Scripts
{
    public class WaypointsMover:MonoBehaviour
    {
        private int _nextIndex;
        private int _finishIndex;
        
        private Action _onFinishedAction;

        private SplineCurve _splineCurve;
        private float _splineT;
        private float _partTime;
        private Vector3 _precisePosition;
        private bool _isRunning;
        private bool _isForwardDirection;
        
        [HideInInspector] public Path path;
        [HideInInspector] public float speed;

        private void Start()
        {
            if (path.isCurved)
            {
                _splineCurve = new SplineCurve();
                UpdateCurvePoints();
            }
        }
        
        public void Move(Transform from,Transform to , Action onFinishedAction)
        {
            if (_isRunning)
                return;
            _onFinishedAction = onFinishedAction;
            _nextIndex = path.waypoints.IndexOf(from);
            _finishIndex = path.waypoints.IndexOf(to);
            _isForwardDirection = _finishIndex > _nextIndex;
            transform.position = from.position;
            _isRunning = true;
            TakeNextWaypoint();
        }

        private void Update()
        {
            if (_isRunning)
            {
                if (path.isCurved)
                {
                    UpdateCurved();
                }
                else
                {
                    UpdateLinear();
                }
            }
        }
        
        private void TakeNextWaypoint()
        {
            if (_nextIndex == _finishIndex)
            {
                _isRunning = false;
                _onFinishedAction?.Invoke();
            }
            else
            {
                _nextIndex += _isForwardDirection ? 1 : -1;
            }
            if (path.isCurved)
                UpdateCurvePoints();
        }

        private void UpdateLinear()
        {
            Transform wayPoint = path.waypoints[_nextIndex];
            Vector3 direction = (wayPoint.position - transform.position).normalized;
            Vector3 nextPosition = transform.position + direction * (speed * Time.deltaTime);

            if (Vector3.Distance(transform.position,wayPoint.position) <= 
                Vector3.Distance(transform.position,nextPosition))
            {
                transform.position = wayPoint.position;
                TakeNextWaypoint();
            }
            else
            {
                transform.position = nextPosition;
            }
        }

        private void UpdateCurved()
        {
            _splineT += Time.deltaTime / _partTime;
            if (_splineT > 1.0f)
            {
                _splineT = 0.0f;
                TakeNextWaypoint();
                UpdateCurvePoints();
            }
            else
            {
                Vector2 point = _splineCurve.GetPoint(_splineT);
                transform.position = point;
            }
        }

        private void UpdateCurvePoints()
        {
            int dInd = _isForwardDirection ? 1 : -1;
            int[] indexes = path.GetSplinePointIndexes((_nextIndex - dInd + path.waypoints.Count) % path.waypoints.Count, _isForwardDirection);
            _splineCurve.ApplyPoints(
                path.waypoints[indexes[0]].position,
                path.waypoints[indexes[1]].position,
                path.waypoints[indexes[2]].position,
                path.waypoints[indexes[3]].position);
            _partTime = GetPartPassTime(_nextIndex);
        }
        
        private float GetPartPassTime(int targetInd)
        {
            int dInd = _isForwardDirection ? 1 : 0;
            return path.GetLenght((targetInd - dInd + path.waypoints.Count) % path.waypoints.Count) / speed;
        }

    }
}