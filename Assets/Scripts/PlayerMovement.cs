using UnityEngine;

public class FPSController : MonoBehaviour
{
    [Header("References")]
    public CharacterController controller;
    public Transform playerCamera;

    [Header("Movement")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 9f;
    public float crouchSpeed = 2.5f;
    public float jumpHeight = 1.5f;
    public float gravity = -20f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 2.5f;
    public float maxLookAngle = 80f;

    [Header("Crouch")]
    public float crouchHeight = 1f;
    public float standingHeight = 2f;
    public float crouchCameraHeight = 1f;
    public float standingCameraHeight = 1.6f;

    float yVelocity;
    float xRotation = 0f;

    bool isCrouching = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        controller.height = standingHeight;
    }

    void Update()
    {
        MouseLook();
        Movement();
        HandleCrouch();
    }

    void MouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * 100f * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * 100f * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    void Movement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        bool sprinting = Input.GetKey(KeyCode.LeftShift);

        float speed = walkSpeed;

        if (isCrouching)
            speed = crouchSpeed;
        else if (sprinting)
            speed = sprintSpeed;

        Vector3 move = transform.right * x + transform.forward * z;

        if (controller.isGrounded && yVelocity < 0)
            yVelocity = -2f;

        if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded && !isCrouching)
        {
            yVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        yVelocity += gravity * Time.deltaTime;

        Vector3 velocity = Vector3.up * yVelocity;

        controller.Move((move * speed + velocity) * Time.deltaTime);
    }

    void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            isCrouching = !isCrouching;

            if (isCrouching)
            {
                controller.height = crouchHeight;

                Vector3 camPos = playerCamera.localPosition;
                camPos.y = crouchCameraHeight;
                playerCamera.localPosition = camPos;
            }
            else
            {
                controller.height = standingHeight;

                Vector3 camPos = playerCamera.localPosition;
                camPos.y = standingCameraHeight;
                playerCamera.localPosition = camPos;
            }
        }
    }
}