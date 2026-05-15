using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class DoorController : MonoBehaviour
{
    [Header("Door Settings")]
    public float openAngle = 90f;
    public float openSpeed = 2f;
    public float interactDistance = 3f;
    public KeyCode interactKey = KeyCode.F;

    [Header("Lock Settings")]
    public bool isLocked = true;
    public string requiredItemID = "red_key";

    [Header("Axis")]
    public Vector3 rotationAxis = Vector3.up;

    [Header("Interaction Offset")]
    public Vector3 interactionOffset;

    [Header("Look Settings")]
    [Range(0f, 1f)]
    public float lookThreshold = 0.7f;

    [Header("UI Prompts")]
    public GameObject openText;
    public GameObject lockedText;

    [Header("Audio")]
    public AudioClip doorOpenSound;
    public AudioClip doorCloseSound;

    private AudioSource audioSource;

    private static DoorController currentDoor;

    private bool isOpen = false;
    private bool isAnimating = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Transform playerTransform;
    private Camera playerCamera;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        closedRotation = transform.rotation;
        openRotation = transform.rotation * Quaternion.AngleAxis(openAngle, rotationAxis);

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
            playerTransform = player.transform;

        playerCamera = Camera.main;

        if (openText != null)
            openText.SetActive(false);

        if (lockedText != null)
            lockedText.SetActive(false);
    }

    void Update()
    {
        if (playerTransform == null || playerCamera == null)
            return;

        Vector3 checkPosition = transform.position + transform.TransformDirection(interactionOffset);

        float distance = Vector3.Distance(playerTransform.position, checkPosition);
        bool isNear = distance <= interactDistance;

        Vector3 dirToDoor = (checkPosition - playerCamera.transform.position).normalized;
        float dot = Vector3.Dot(playerCamera.transform.forward, dirToDoor);
        bool isLooking = dot > lookThreshold;

        if (isNear && isLooking && !isAnimating)
        {
            currentDoor = this;

            bool hasKey = PlayerInventory.instance != null &&
                          PlayerInventory.instance.HasItem(requiredItemID);

            bool canOpen = !isLocked || hasKey;

            ShowUI(canOpen);

            if (Input.GetKeyDown(interactKey))
            {
                HandleInteraction(hasKey);
            }
        }
        else
        {
            if (currentDoor == this)
            {
                HideUI();
                currentDoor = null;
            }
        }
    }

    void HandleInteraction(bool hasKey)
    {
        if (isLocked)
        {
            if (hasKey)
            {
                isLocked = false;
            }
            else
            {
                ShowUI(false);
                return;
            }
        }

        if (!isOpen)
        {
            PlaySound(doorOpenSound);
            StartCoroutine(RotateDoor(closedRotation, openRotation));
        }
        else
        {
            PlaySound(doorCloseSound);
            StartCoroutine(RotateDoor(openRotation, closedRotation));
        }

        isOpen = !isOpen;
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    void ShowUI(bool canOpen)
    {
        if (openText != null)
            openText.SetActive(canOpen);

        if (lockedText != null)
            lockedText.SetActive(!canOpen);
    }

    void HideUI()
    {
        if (openText != null)
            openText.SetActive(false);

        if (lockedText != null)
            lockedText.SetActive(false);
    }

    IEnumerator RotateDoor(Quaternion from, Quaternion to)
    {
        isAnimating = true;

        HideUI();

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