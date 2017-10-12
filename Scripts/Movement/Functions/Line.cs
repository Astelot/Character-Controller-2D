using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line {

    private Vector2 _pointA, _pointB;
    private float b, m;

    public Vector2 PointA {
        get { return _pointA; }
        set {
            if(value != _pointA) {
                _pointA = value;
                RecalculateFunction();
            }
        }
    }

    public Vector2 PointB {
        get { return _pointB; }
        set {
            if(value != _pointB) {
                _pointB = value;
                RecalculateFunction();
            }
        }
    }

    public float getIntercept() {
        return b;
    }

    public float getSlope() {
        return m;
    }

    public Line() {
        this._pointA = Vector2.zero;
        this._pointB = Vector2.zero;

        RecalculateFunction();
    }

    public Line(Vector2 pointB) {
        this._pointA = Vector2.zero;
        this._pointB = pointB;

        RecalculateFunction();
    }

    public Line(Vector2 pointA, Vector2 pointB) {
        this._pointA = pointA;
        this._pointB = pointB;

        RecalculateFunction();
    }

    public void RecalculateFunction() {
        if (_pointA != Vector2.zero && _pointB != Vector2.zero) {
            m = (_pointB.y - _pointA.y) / (_pointB.x - _pointA.x);
            b = m * (-_pointA.x) + _pointA.y;
        }
        else {
            m = 0;
            b = _pointA.y;
        }
    }
}
