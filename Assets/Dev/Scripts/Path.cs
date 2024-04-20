using System;
using System.Collections;
using System.Collections.Generic;
using Dev.Scripts;
using UnityEngine;

public class Path : MonoBehaviour
{
    public List<Transform> waypoints = new List<Transform>();
    
    [HideInInspector] public bool isCurved;
    [HideInInspector] public Color gizmosColor = Color.white;
    [HideInInspector] public float gizmosRadius = 10.0f;
    public void OnDrawGizmos()
    {
        Gizmos.color = gizmosColor;
        for (int i = 0; i < waypoints.Count; i++)
        {
            Gizmos.DrawSphere(waypoints[i].transform.position,gizmosRadius);
            if (i<waypoints.Count-1)
                DrawPart(i);
        }
    }

    private void DrawPart(int ind)
    {
        if (isCurved)
        {
            Vector2[] dividedPoints = GetDividedPoints(ind);
            for (int i = 0; i < dividedPoints.Length; i++)
            {
                Gizmos.DrawLine(dividedPoints[i],dividedPoints[i + 1]);
            }
        }
        else
        {
            Gizmos.DrawLine(waypoints[ind].position,waypoints[(ind+1) % waypoints.Count].position);
        }
    }

    private Vector2[] GetDividedPoints(int ind)
    {
        var points = new Vector2[11];
        var pointInd = 0;
        var indexes = GetSplinePointIndexes(ind, true);
        Vector2 a = waypoints[indexes[0]].transform.position;
        Vector2 b = waypoints[indexes[1]].transform.position;
        Vector2 c = waypoints[indexes[2]].transform.position;
        Vector2 d = waypoints[indexes[3]].transform.position;
        for (float i = 0; i < 1.001f; i+=0.1f)
        {
            points[pointInd++] = SplineCurve.GetPoint(a,b,c,d,i);
        }

        return points;
    }

    public int[] GetSplinePointIndexes(int baseInd, bool isForwardDirection)
    {
        var dInd = isForwardDirection ? 1 : -1;
        return new int[]
        {
            Mathf.Clamp(baseInd - dInd, 0, waypoints.Count - 1),
            baseInd,
            Mathf.Clamp(baseInd + dInd, 0, waypoints.Count - 1),
            Mathf.Clamp(baseInd + dInd * 2, 0, waypoints.Count - 1)
        };
    }

    public float GetLenght(int startIndex)
    {
        return Vector2.Distance(waypoints[startIndex].position, waypoints[(startIndex + 1) % waypoints.Count].position);
    }
}
