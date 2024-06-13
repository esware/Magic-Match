using System.Collections.Generic;
using UnityEngine;

namespace MapScripts.Scripts
{
    public class Path : MonoBehaviour
    {
        public List<Transform> waypoints = new List<Transform>();
        
        public bool isCurved;
        
        public Color gizmosColor = Color.white;
        
        public float gizmosRadius = 1.0f;

        public void OnDrawGizmos()
        {
            Gizmos.color = gizmosColor;
            for (int i = 0; i < waypoints.Count; i++)
            {
                Gizmos.DrawSphere(waypoints[i].transform.position, gizmosRadius);
                if (i < waypoints.Count - 1)
                    DrawPart(i);
            }
        }

        private void DrawPart(int ind)
        {
            if (isCurved)
            {
                Vector2[] devidedPoints = GetDivededPoints(ind);
                for (int i = 0; i < devidedPoints.Length - 1; i++)
                    Gizmos.DrawLine(devidedPoints[i], devidedPoints[i + 1]);
            }
            else
            {
                Gizmos.DrawLine(waypoints[ind].position, waypoints[(ind + 1) % waypoints.Count].position);
            }
        }

        private Vector2[] GetDivededPoints(int ind)
        {
            Vector2[] points = new Vector2[11];
            int pointInd = 0;
            int[] indexes = GetSplinePointIndexes(ind, true);
            Vector2 a = waypoints[indexes[0]].transform.position;
            Vector2 b = waypoints[indexes[1]].transform.position;
            Vector2 c = waypoints[indexes[2]].transform.position;
            Vector2 d = waypoints[indexes[3]].transform.position;
            for (float t = 0; t <= 1.001f; t += 0.1f)
                points[pointInd++] = SplineCurve.GetPoint(a, b, c, d, t);
            return points;
        }

        public int[] GetSplinePointIndexes(int baseInd, bool isForwardDirection)
        {
            int dInd = isForwardDirection ? 1 : -1;
            return new int[]
            {
                Mathf.Clamp(baseInd - dInd, 0, waypoints.Count - 1),
                baseInd,
                Mathf.Clamp(baseInd + dInd, 0, waypoints.Count - 1),
                Mathf.Clamp(baseInd + 2*dInd, 0, waypoints.Count - 1)
            };
        }

        public float GetLength()
        {
            float length = 0;
            for (int i = 0; i < waypoints.Count; i++)
            {
                Vector2 p1 = waypoints[i].transform.position;
                Vector2 p2 = waypoints[(i + 1) % waypoints.Count].transform.position;
                length += Vector2.Distance(p1, p2);
            }
            return length;
        }

        public float GetLength(int startInd)
        {
            return Vector2.Distance(
                waypoints[startInd].transform.position,
                waypoints[(startInd + 1) % waypoints.Count].transform.position);
        }

    }
}
