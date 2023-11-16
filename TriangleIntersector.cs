using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TriangleIntersector
{
    public static bool findIntersectionSegmentBetweenTriangles(List<Vector3> triangleA, List<Vector3> triangleB, out Vector3 start, out Vector3 end) {
        start = Vector3.zero; end = Vector3.zero;

        float[] distancesA = new float[3];
        float[] distancesB = new float[3];

        Plane planeA = new Plane(triangleA[0], triangleA[1], triangleA[2]);
        Plane planeB = new Plane(triangleB[0], triangleB[1], triangleB[2]);

        distancesA[0] = planeB.GetDistanceToPoint(triangleA[0]); distancesA[1] = planeB.GetDistanceToPoint(triangleA[1]); distancesA[2] = planeB.GetDistanceToPoint(triangleA[2]);
        distancesB[0] = planeA.GetDistanceToPoint(triangleB[0]); distancesB[1] = planeA.GetDistanceToPoint(triangleB[1]); distancesB[2] = planeA.GetDistanceToPoint(triangleB[2]);

        if ((Mathf.Sign(distancesA[0]) == Mathf.Sign(distancesA[1]) && Mathf.Sign(distancesA[0]) == Mathf.Sign(distancesA[2]) && Mathf.Sign(distancesA[1]) == Mathf.Sign(distancesA[2])) ||
            (Mathf.Sign(distancesB[0]) == Mathf.Sign(distancesB[1]) && Mathf.Sign(distancesB[0]) == Mathf.Sign(distancesB[2]) && Mathf.Sign(distancesB[1]) == Mathf.Sign(distancesB[2])) ||
            (planeA.normal == planeB.normal))
        {
            return false;
        }


        PlaneIntersector.getIntersectionLineOfPlanes(triangleA[0], planeA.normal, triangleB[0], planeB.normal, out Vector3 point, out Vector3 direction);

        float[] interval1 = getInterval(triangleA, distancesA, point, direction);
        float[] interval2 = getInterval(triangleB, distancesB, point, direction);

        if (interval1[0] <= interval2[1] && interval2[0] <= interval1[1]){
            float[] points = interval1.Concat(interval2).ToArray();
            Array.Sort(points);

            start = point + points[1] * direction;
            end = point + points[2] * direction;
            return true;
        }

        return false;
    }


    static float[] getInterval(List<Vector3> triangle, float[] distances, Vector3 point, Vector3 direction)
    {
        float[] intervall = new float[2];

        int a = 0, b = 0, c = 0;
        if (Mathf.Sign(distances[1]) == Mathf.Sign(distances[2])) { a = 1; b = 0; c = 2; Debug.Log("1, 2 | " + a + " " + b + " " + c); }
        if (Mathf.Sign(distances[0]) == Mathf.Sign(distances[2])) { a = 2; b = 1; c = 0; Debug.Log("0, 2 | " + a + " " + b + " " + c); }
        if (Mathf.Sign(distances[0]) == Mathf.Sign(distances[1])) { a = 0; b = 2; c = 1; Debug.Log("0, 1 | " + a + " " + b + " " + c); }

        float p0, p1;

        p0 = Vector3.Dot(direction, triangle[a] - point);
        p1 = Vector3.Dot(direction, triangle[b] - point);
        intervall[0] = p0 + (p1 - p0) * distances[a] / (distances[a] - distances[b]);

        p0 = Vector3.Dot(direction, triangle[b] - point);
        p1 = Vector3.Dot(direction, triangle[c] - point);
        intervall[1] = p0 + (p1 - p0) * distances[b] / (distances[b] - distances[c]);

        Array.Sort(intervall);
        return intervall;
    }

}
