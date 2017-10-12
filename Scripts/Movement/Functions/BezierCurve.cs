using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurve {

    private List<Vector2> points = new List<Vector2>();

    public Vector2 StartPoint {
        get { return points[0]; }
        set { points[0] = value; }
    }

    public Vector2 EndPoint {
        get { return points[points.Count - 1]; }
        set { points[points.Count - 1] = value; }
    }

    public BezierCurve(Vector2 startPoint, Vector2 endPoint, params Vector2[] controlPoints) {
        points.Add(startPoint);

        foreach (Vector2 p in controlPoints) {
            if (p == null || points.Count >= 3)
                break;

            points.Add(p);
        }
        points.Add(endPoint);
    }

    public Vector2 CalculatePoint(float t) {
        float u = 1 - t;
        int n = points.Count;
        int degree = n - 1;

        Vector2 result = points[0] * Mathf.Pow(u, degree);
        float m;

        for(int i = 1; i < n; i++) {
            if (i != 0 && i != n - 1) {
                m = degree;
            }
            else m = 1;

            result += m * points[i] * Mathf.Pow(u, degree - i) * Mathf.Pow(t, i);
        }
        return result;
    }
}
