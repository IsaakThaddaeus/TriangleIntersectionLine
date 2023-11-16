using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

public static class TriangleIntersector
{
    public static bool findIntersectionSegmentBetweenTriangles(Vector3 a1, Vector3 b1, Vector3 c1, Vector3 a2, Vector3 b2, Vector3 c2, out Vector3 start, out Vector3 end) {
        start = Vector3.zero; end = Vector3.zero;

        Plane p1 = new Plane(a1, b1, c1);
        Plane p2 = new Plane(a2, b2, c2);

        float da1 = p2.GetDistanceToPoint(a1); 
        float db1 = p2.GetDistanceToPoint(b1); 
        float dc1 = p2.GetDistanceToPoint(c1);

        float da2 = p1.GetDistanceToPoint(a2); 
        float db2 = p1.GetDistanceToPoint(b2); 
        float dc2 = p1.GetDistanceToPoint(c2);



        if (rejectionTest(da1, db1, dc1, da2, db2, dc2, p1, p2)){
            return false;
        }

        PlaneIntersector.getIntersectionLineOfPlanes(a1, p1.normal, a2, p2.normal, out Vector3 o, out Vector3 d);
        float[] i1 = getInterval(a1, b1, c1, da1, db1, dc1, o, d);
        float[] i2 = getInterval(a2, b2, c2, da2, db2, dc2, o, d);

        if (i1[0] <= i2[1] && i2[0] <= i1[1]){
            float[] points = i1.Concat(i2).ToArray();
            Array.Sort(points);
            start = o + points[1] * d;
            end   = o + points[2] * d;
            return true;
        }

        return false;
    }


    static float[] getInterval(Vector3 a, Vector3 b, Vector3 c, float da, float db, float dc, Vector3 point, Vector3 direction)
    {
        float[] intervall = new float[2];
        Vector3 aI = Vector3.zero; float daI = 0;
        Vector3 bI = Vector3.zero; float dbI = 0;
        Vector3 cI = Vector3.zero; float dcI = 0;


        if (Mathf.Sign(db) == Mathf.Sign(dc)) { aI = b; bI = a; cI = c;
                                                daI = db; dbI = da; dcI = dc; }

        if (Mathf.Sign(da) == Mathf.Sign(dc)) { aI = c; bI = b; cI = a;
                                                daI = dc; dbI = db; dcI = da; }

        if (Mathf.Sign(da) == Mathf.Sign(db)) { aI = a; bI = c; cI = b;
                                                daI = da; dbI = dc; dcI = db; }

        float p0, p1;
        p0 = Vector3.Dot(direction, aI - point);
        p1 = Vector3.Dot(direction, bI - point);
        intervall[0] = p0 + (p1 - p0) * daI / (daI - dbI);

        p0 = Vector3.Dot(direction, bI - point);
        p1 = Vector3.Dot(direction, cI - point);
        intervall[1] = p0 + (p1 - p0) * dbI / (dbI - dcI);

        Array.Sort(intervall);
        return intervall;
    }

    static bool rejectionTest(float da1, float db1, float dc1, float da2, float db2, float dc2, Plane p1, Plane p2){
        if ((Mathf.Sign(da1) == Mathf.Sign(db1) && Mathf.Sign(da1) == Mathf.Sign(dc1) && Mathf.Sign(db1) == Mathf.Sign(dc1)) ||
            (Mathf.Sign(da2) == Mathf.Sign(db2) && Mathf.Sign(da2) == Mathf.Sign(dc2) && Mathf.Sign(db2) == Mathf.Sign(dc2)) || 
            (p1.normal == p2.normal))
            {
                return true;
            }

        return false;
    }

}
