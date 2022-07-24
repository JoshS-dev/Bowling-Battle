using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// HELP FROM https://www.youtube.com/watch?v=_QajrabyTJc

public class PlayerMovement : MonoBehaviour
{
    public float mouseSensitivity = 4.5f;
    public float moveSpeed = 9f;
    public float sprintScalar = 1.50f;
    public float strafeScalar = 0.80f;
    public float backUpScalar = 0.65f;
    public float ballHeldScalar = 0.75f;
    public float gravity = -18f; // earth -9.81f
    public float jumpHeight = 2.5f; // roughly 2.5 units jumped 

    float yVelocity;
    bool isGrounded;

    float xRotation = 0f;

    CharacterController _cc;
    PlayerCondition _pc;
    PlayerBallHandler _pbh;
    
    /*
    public Transform groundCheck; // fiddly solution the tutorial used
    public float groundDistance = 0.1f;
    public LayerMask groundMask;
    */

    private void Start() {
        _cc = GetComponent<CharacterController>();
        _pc = GetComponent<PlayerCondition>();
        _pbh = GetComponent<PlayerBallHandler>();
    }
    void Update() {
        //isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask); // fiddly solution the tutorial used
        isGrounded = _cc.isGrounded; // I found this to be a lot more simple, and just as reliable out of the box
        if (isGrounded)
            yVelocity = -2f;

        if (_pc.isAlive && GameObject.Find("/SceneHandler").GetComponent<SceneHandler>().currentState != BB_Utils.GameState.Paused) {
            HorizontalMovement();
            VerticalMovement();
            MouseMovement();
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand)) {
                transform.localScale = new Vector3(1f,0.75f,1f);
            }
            else {
                transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }
        if (transform.position.y < -5f) // Fallen out of the map
            transform.position = Vector3.zero;
    }

    void HorizontalMovement() {
        ///*
        float horizontalDir = Input.GetAxis("Horizontal");
        float verticalDir = Input.GetAxis("Vertical");
        //*/
        /*
        float horizontalDir = Input.GetAxisRaw("Horizontal");
        float verticalDir = Input.GetAxisRaw("Vertical");
        */
        float xScalar = 1.0f; // side to side
        float zScalar = 1.0f; // forward and backward
        if (Input.GetKey(KeyCode.LeftShift) && _pc.canSprint) {
            xScalar *= sprintScalar;
            zScalar *= sprintScalar;
            _pc.curSprintTime -= 2f*Time.deltaTime; // doubled due to passive regeneration
        }
        if(verticalDir < 0)
            zScalar *= backUpScalar;
        xScalar *= strafeScalar;
        ///* // Option 1, going diagonal is faster
        Vector3 movement = transform.right * horizontalDir * moveSpeed * xScalar +
                           transform.up * 0f +
                           transform.forward * verticalDir * moveSpeed * zScalar;
        if (_pbh.hasBall)
            movement *= ballHeldScalar;
        movement *= Mathf.Max(Mathf.Abs(horizontalDir * xScalar), Mathf.Abs(verticalDir * zScalar)) / Mathf.Sqrt(Mathf.Pow(horizontalDir * xScalar, 2) + Mathf.Pow(verticalDir * zScalar, 2));
        _cc.Move(movement * Time.deltaTime);
        //*/ 
        /* // Option 2, player continues moving after button is unpressed
        Vector3 movement = transform.right * horizontalDir +
                           transform.up * 0f +
                           transform.forward * verticalDir;
        movement = movement.normalized;
        movement.x *= xScalar;
        movement.z *= zScalar;
        movement *= moveSpeed;
        if (_pbh.hasBall)
            movement *= ballHeldScalar;
        Debug.Log(horizontalDir+" "+verticalDir);
        _cc.Move(movement * Time.deltaTime);
        */
    }

    void VerticalMovement() {
        if (Input.GetButtonDown("Jump") && isGrounded) {
            // physics equation, initial velocity needed to jump = 
            //sqrt(height you want to reach * -2 * gravity)
            yVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        yVelocity += gravity * Time.deltaTime;

        _cc.Move(new Vector3(0f, yVelocity, 0f) * Time.deltaTime);
    }

    void MouseMovement() {
        Transform cam = transform.Find("Main Camera");
        Vector2 mouseAxes = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * mouseSensitivity;

        xRotation -= mouseAxes.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cam.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseAxes.x);
    }
}
