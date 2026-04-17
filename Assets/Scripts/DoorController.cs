using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DoorController : MonoBehaviour
{
    [Header("Door Settings")]
    public float openAngle = 90f;
    public float openSpeed = 2f;
    public float interactDistance = 3f;
    public KeyCode interactKey = KeyCode.F;

    [Header("Axis")]
    public Vector3 rotationAxis = Vector3.up;

    [Header("Interaction Offset")]
    public Vector3 interactionOffset;

    [Header("Look Settings")]
    [Range(0f, 1f)]
    public float lookThreshold = 0.7f; // higher = more precise (0.7–0.9 is good)

    [Header("UI Prompt (Shared)")]
    public GameObject interactText;

    private static DoorController currentDoor;

    private bool isOpen = false;
    private bool isAnimating = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Transform playerTransform;
    private Camera playerCamera;

    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = transform.rotation * Quaternion.AngleAxis(openAngle, rotationAxis);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;

        playerCamera = Camera.main;

        if (interactText != null)
            interactText.SetActive(false);

        Text txt = interactText != null ? interactText.GetComponent<Text>() : null;
        if (txt != null)
            txt.text = "Press " + interactKey.ToString() + " to Open";
    }

    void Update()
    {
        if (playerTransform == null || playerCamera == null) return;

        Vector3 checkPosition = transform.position + transform.TransformDirection(interactionOffset);

        float distance = Vector3.Distance(playerTransform.position, checkPosition);
        bool isNear = distance <= interactDistance;

        // 👇 Check if player is LOOKING at the door
        Vector3 dirToDoor = (checkPosition - playerCamera.transform.position).normalized;
        float dot = Vector3.Dot(playerCamera.transform.forward, dirToDoor);
        bool isLooking = dot > lookThreshold;

        if (isNear && isLooking && !isAnimating)
        {
            currentDoor = this;

            if (interactText != null)
                interactText.SetActive(true);

            if (Input.GetKeyDown(interactKey))
            {
                if (!isOpen)
                    StartCoroutine(RotateDoor(closedRotation, openRotation));
                else
                    StartCoroutine(RotateDoor(openRotation, closedRotation));

                isOpen = !isOpen;
            }
        }
        else
        {
            if (currentDoor == this)
            {
                if (interactText != null)
                    interactText.SetActive(false);

                currentDoor = null;
            }
        }
    }

    IEnumerator RotateDoor(Quaternion from, Quaternion to)
    {
        isAnimating = true;

        if (interactText != null)
            interactText.SetActive(false);

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * openSpeed;
            transform.rotation = Quaternion.Slerp(from, to, t);
            yield return null;
        }

        transform.rotation = to;
        isAnimating = false;
    }
}