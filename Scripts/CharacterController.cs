using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour {

    public float maximumVelocity;
    public float acceleration;
    public float rotationSpeed;
    public float rotationTolerance;

    private bool isAiming = false;
    private Vector2 mousePos;
    private Rigidbody2D rbody;

    private void Start() {
        rbody = this.GetComponent<Rigidbody2D>();
    }

    private void Update() {
        isAiming = Input.GetAxis("Aim") == 1;
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void FixedUpdate() {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector2 dirInput;

        if (isAiming) {
            dirInput = (transform.right * x) + transform.up * y;
        }
        else {
            dirInput = new Vector2(x, y);
        }
        
        if(dirInput.sqrMagnitude > 1) {
            dirInput.Normalize();
        }

        RotateCharacter();
        MoveCharacter(dirInput);
    }

    private void MoveCharacter(Vector2 inputs) {

        if(rbody.velocity.magnitude < maximumVelocity) {
            Vector2 velocity = inputs * acceleration;

            if((velocity + rbody.velocity).magnitude > maximumVelocity) {
                float combMag = velocity.magnitude + rbody.velocity.magnitude;
                float desiredMag = combMag - rbody.velocity.magnitude;

                velocity = desiredMag / velocity.magnitude * velocity;
            }

            rbody.velocity += velocity;
        }
    }

    private void RotateCharacter() {
        float angleRad = Mathf.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x);
        float angle = (180 / Mathf.PI) * angleRad;

        Debug.Log("Angle:" + angle);
        Debug.Log("Rotation:" + rbody.rotation);

        if(Mathf.Abs(rbody.rotation - angle) < rotationTolerance) {
            rbody.rotation = angle;
        }

        if(rbody.rotation < angle)
            rbody.rotation += rotationSpeed * Time.fixedDeltaTime;
        else if(rbody.rotation > angle) {
            rbody.rotation -= rotationSpeed * Time.fixedDeltaTime;
        }

        if (rbody.rotation > 180)
            rbody.rotation -= 360;
        else if (rbody.rotation < -180)
            rbody.rotation += 360;
    }
}
