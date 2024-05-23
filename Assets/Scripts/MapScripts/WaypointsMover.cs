using System;
using UnityEngine;

namespace MapScripts.Scripts
{
    public class WaypointsMover : MonoBehaviour
{
    private int _nextInd;
    private int _finishInd;
    private Action _finishedAction;

    private SplineCurve _splineCurve;
    private float _splineT;
    private float _partTime;
    private Vector3 _precisePosition;
    private bool _isRunning;
    private bool _isForwardDirection;
    
    public Path path;

    [HideInInspector]
    public float Speed;

    public void Start()
    {
        if (path.isCurved)
        {
            _splineCurve = new SplineCurve();
            UpdateCurvePoints();
        }
    }

    public void Move(Transform from, Transform to, Action finishedAction)
    {
        if (_isRunning)
            return;

        _finishedAction = finishedAction;
        _nextInd = path.waypoints.IndexOf(from);
        _finishInd = path.waypoints.IndexOf(to);
        _isForwardDirection = _finishInd > _nextInd;
        transform.position = from.position;
        _isRunning = true;
        TakeNextWaypoint();
    }

    public void Update()
    {
        if (_isRunning)
        {
            if (path.isCurved)
                UpdateCurved();
            else
                UpdateLinear();
        }
    }

    private void TakeNextWaypoint()
    {
        if (_nextInd == _finishInd)
        {
            _isRunning = false;
            _finishedAction();
        }
        else
        {
            _nextInd += _isForwardDirection ? 1 : -1;
        }

        if (path.isCurved)
            UpdateCurvePoints();
    }

    #region Linear
    private void UpdateLinear()
    {
        Transform waypoint = path.waypoints[_nextInd];
        Vector3 direction = (waypoint.position - transform.position).normalized;
        Vector3 nextPosition = transform.position + direction * Speed * Time.deltaTime;
        if (Vector3.Distance(transform.position, waypoint.position) <=
            Vector3.Distance(transform.position, nextPosition))
        {
            transform.position = waypoint.position;
            TakeNextWaypoint();
        }
        else
        {
            transform.position = nextPosition;
        }
    }

    #endregion

    #region Curved
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
        int[] indexes = path.GetSplinePointIndexes((_nextInd - dInd + path.waypoints.Count) % path.waypoints.Count, _isForwardDirection);
        _splineCurve.ApplyPoints(
            path.waypoints[indexes[0]].transform.position,
            path.waypoints[indexes[1]].transform.position,
            path.waypoints[indexes[2]].transform.position,
            path.waypoints[indexes[3]].transform.position);
        _partTime = GetPartPassTime(_nextInd);
    }

    private float GetPartPassTime(int targetInd)
    {
        int dInd = _isForwardDirection ? 1 : 0;
        return path.GetLength((targetInd - dInd + path.waypoints.Count) % path.waypoints.Count) / Speed;
    }

    #endregion

}
}