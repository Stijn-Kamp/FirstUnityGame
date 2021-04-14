using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    // Character controller
    public CharacterController controller;

    // Camera movement
    public Transform playerCamera;
    public float c_Sensitivity = 100f;
    public float c_MaxRotation = 160;
    private float xRotation = 0f;
    private float zRotation = 0f;


    // Player movement
    public float p_Speed = 35f;
    public float p_Acceleration = 10f;
    public float p_Deceleration = 20f;

    public float p_Gravity = -30f;
    public float p_JumpHeight = 15f;
    public float p_JumpSpeed = 0.5f;
    private bool p_DoubleJump;

    private Vector3 verticalVelocity;
    private Vector3 horizontalVelocity;

    // Walljump
    Vector3 WallDirection;
    float wallSlideMultiplier = 0.5f;

    // Flips
    public float rotationSpeed = 25f;

    // Crouch/slide
    private KeyCode crouchKey = KeyCode.LeftControl;
    private Vector3 crouchTransform = new Vector3(1, .5f, 1);
    private bool isCrouching = false;
    private bool isSliding = false;

    public float crouchMultiplier = .2f;
    public float slideMultiplier = 2f;

    private float slideTime = 0.5f;

    // groundCheck
    public LayerMask groundMask;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    

    // Unity functions
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCamera();
        UpdateMovement();
    }

    void UpdateMovement()
    {
        UpdatePlayerLocation();
        if(Input.GetButtonDown("Jump")) Jump();
        if (Input.GetKeyDown(crouchKey)) StartSlide();

    }


    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.normal.y < 0.1) WallDirection = hit.normal;
        Debug.DrawRay(hit.point, hit.normal, Color.red, 1.25f);
    }


    // Player and camera movement
    void UpdatePlayerLocation()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Calculate player speed
        float speed = GetSpeed();

        // Move the player
        Vector3 newHorizontalVelocity;

        newHorizontalVelocity = transform.right * x + transform.forward * z;
        newHorizontalVelocity = Quaternion.Euler(0, playerCamera.localRotation.eulerAngles.y, 0) * newHorizontalVelocity * speed;  // Move the player in the direction of the camera


        //SLOW MOVEMENT IN AIRR
        // Calculate acceleration
        if (IsGrounded())
        {
            horizontalVelocity = newHorizontalVelocity;
        }
        else if (isSliding)
        {
            horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, newHorizontalVelocity, 0.5f);
        }
        else
        {
            horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, newHorizontalVelocity, p_JumpSpeed);
        }


        // Calculate falling speed
        if (IsGrounded() && verticalVelocity.y < 0)
        {
            verticalVelocity.y = -2f; // Prevents fallingspeed from building up
        }
        else
        {
            float fallSpeed = IsCrouching() ? 5 : 1;
            verticalVelocity.y += p_Gravity * Time.deltaTime * fallSpeed;
            //if (IsTouchingWall()) verticalVelocity.y *= wallSlideMultiplier; // slow fall speed when touching wall
        }

        // Finally move the player
        controller.Move((verticalVelocity + horizontalVelocity) * Time.deltaTime);

    }

    void UpdateCamera()
    {
        float multiplier = c_Sensitivity * Time.deltaTime;
        float mouseX = Input.GetAxis("Mouse X") * multiplier;
        float mouseY = Input.GetAxis("Mouse Y") * multiplier;

        zRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -c_MaxRotation, c_MaxRotation);

        //Move the camera
        playerCamera.localRotation = Quaternion.Euler(xRotation, zRotation, 0f);
        //transform.Rotate(Vector3.up * mouseX);
    }


    // Movement functions

    void Jump()
    {
        bool isTouchingWall = IsTouchingWall();
        bool canJump = isTouchingWall || IsGrounded();
        StopSlide();
        if (canJump || p_DoubleJump)
        {
            p_DoubleJump = canJump;
            verticalVelocity.y = Mathf.Sqrt(p_JumpHeight * -2f * p_Gravity);
            if(isTouchingWall)
            {
                horizontalVelocity = WallDirection * GetSpeed();
            }
        }

    }

    // Crouching

    void StartCrouch()
    {
        isCrouching = true;
        transform.localScale = crouchTransform;
    }

    void StopCrouch()
    {
        isCrouching = false;
        transform.localScale = new Vector3(1, 1, 1);
    }

    void StartSlide()
    {
        if (!isSliding)
        {
            isSliding = true;
            StartCrouch();
            Invoke("StopSlide", slideTime);
        }
    }
    void StopSlide()
    {
        isSliding = false;
        StopCrouch();
    }

    // Cool flippin shit

    void BackFlip()
    {
        float rotation = transform.eulerAngles.x;
        Pitch(-1);
    }

    void Pitch(int direction = 1)
    {
        transform.Rotate(rotationSpeed * Time.deltaTime * direction, 0, 0); //rotates on X axis
    }

    // Checking important shit
    bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    bool IsCrouching()
    {
        return transform.localScale == crouchTransform;
    }


    bool IsTouchingWall()
    {
        return Physics.CheckCapsule(groundCheck.position, groundCheck.position, groundDistance*3, groundMask);
    }
        
    float GetSpeed()
    {
        float speed = p_Speed;
        if (isSliding) speed *= slideMultiplier;
        else if (isCrouching) speed *= crouchMultiplier;
        return speed;
    }
}
