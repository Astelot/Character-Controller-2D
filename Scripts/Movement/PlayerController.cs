using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float acceleration = 0.5f;
    public VelocityConstraint velocityConstraint;

    public AnimationCurve curveanimtest;
    private bool freeLook;
    private Rigidbody2D rbody;
    public Line velocityLine;

    void Start() {
        
    }

    void Awake() {
        velocityLine = new Line();
        velocityConstraint = new VelocityConstraint() {
            line = velocityLine
        };
        rbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() {
        Vector2 axisX = Input.GetAxis("Horizontal") * transform.right;
        Vector2 axisY = Input.GetAxis("Vertical") * transform.up;
        Vector2 axis = axisX + axisY;

    }
}
