using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeypadProximityInteract : MonoBehaviour
{
    [Header("References")]
    public KeypadSystem keypadSystem;

    [Header("Interaction")]
    public float interactDistance = 3f;
    public KeyCode interactKey = KeyCode.E;

    [Header("Interaction Offset")]
    public Vector3 interactionOffset;

    [Header("Look Settings")]
    [Range(0f, 1f)]
    public float lookThreshold = 0.7f;

    [Header("UI Prompt")]
    public GameObject interactText;

    private Transform playerTransform;
    private Camera playerCamera;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;

        playerCamera = Camera.main;

        if (interactText != null)
            interactText.SetActive(false);

        // Set text automatically
        Text txt = interactText != null ? interactText.GetComponent<Text>() : null;
        if (txt != null)
            txt.text = "Press " + interactKey + " to Use Keypad";

        TMP_Text tmp = interactText != null ? interactText.GetComponent<TMP_Text>() : null;
        if (tmp != null)
            tmp.text = "Press " + interactKey + " to Use Keypad";
    }

    void Update()
    {
        if (playerTransform == null || playerCamera == null || keypadSystem == null)
            return;

        // 👇 SAME as your door system
        Vector3 checkPosition = transform.position + transform.TransformDirection(interactionOffset);

        float distance = Vector3.Distance(playerTransform.position, checkPosition);
        bool isNear = distance <= interactDistance;

        // 👇 SAME look logic as door
        Vector3 dirToKeypad = (checkPosition - playerCamera.transform.position).normalized;
        float dot = Vector3.Dot(playerCamera.transform.forward, dirToKeypad);
        bool isLooking = dot > lookThreshold;

        if (isNear && isLooking && !keypadSystem.IsOpen())
        {
            if (interactText != null)
                interactText.SetActive(true);

            if (Input.GetKeyDown(interactKey))
            {
                keypadSystem.OpenKeypad();
            }
        }
        else
        {
            if (interactText != null)
                interactText.SetActive(false);
        }
    }
}