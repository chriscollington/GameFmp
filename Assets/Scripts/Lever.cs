using UnityEngine;
using UnityEngine.UI;

public class Lever : MonoBehaviour
{
    [Header("References")]
    public GearController gearController;
    public Transform player;
    public Camera playerCamera;

    [Header("Interaction Distances")]
    public float textDistance = 3f;        // when text shows
    public float interactDistance = 5f;    // when key works (bigger)

    [Header("Input")]
    public KeyCode interactKey = KeyCode.E;

    [Header("Look Settings")]
    [Range(0f, 1f)]
    public float lookThreshold = 0.7f;

    [Header("UI Prompt")]
    public GameObject interactText;

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (playerCamera == null)
            playerCamera = Camera.main;

        if (interactText != null)
        {
            interactText.SetActive(false);

            Text txt = interactText.GetComponent<Text>();
            if (txt != null)
                txt.text = "Press " + interactKey.ToString() + " to Activate";
        }
    }

    void Update()
    {
        if (player == null || playerCamera == null || gearController == null)
            return;

        float distance = Vector3.Distance(player.position, transform.position);

        bool isNearText = distance <= textDistance;
        bool isNearInteract = distance <= interactDistance;

        // Check if looking at lever
        Vector3 dirToLever = (transform.position - playerCamera.transform.position).normalized;
        float dot = Vector3.Dot(playerCamera.transform.forward, dirToLever);
        bool isLooking = dot > lookThreshold;

        // Show text ONLY when close
        if (isNearText && isLooking)
        {
            if (interactText != null)
                interactText.SetActive(true);
        }
        else
        {
            if (interactText != null)
                interactText.SetActive(false);
        }

        // Allow interaction from further away
        if (isNearInteract && isLooking && Input.GetKeyDown(interactKey))
        {
            gearController.ToggleLever();
        }
    }
}