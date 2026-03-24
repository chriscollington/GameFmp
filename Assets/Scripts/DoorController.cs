using System.Collections;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Door Settings")]
    public float openAngle = 90f;        // How many degrees to swing open
    public float openSpeed = 2f;         // Speed of the swing
    public float interactDistance = 3f;  // How close the player needs to be
    public KeyCode interactKey = KeyCode.F;

    [Header("Axis")]
    public Vector3 rotationAxis = Vector3.up; // Rotate around Y axis

    private bool isOpen = false;
    private bool isAnimating = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Transform playerTransform;

    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = transform.rotation * Quaternion.AngleAxis(openAngle, rotationAxis);

        // Find player - tag your player GameObject as "Player"
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;
    }

    void Update()
    {
        if (isAnimating) return;

        // Check distance to player
        if (playerTransform != null)
        {
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            if (distance > interactDistance) return;
        }

        if (Input.GetKeyDown(interactKey))
        {
            if (!isOpen)
                StartCoroutine(RotateDoor(closedRotation, openRotation));
            else
                StartCoroutine(RotateDoor(openRotation, closedRotation));

            isOpen = !isOpen;
        }
    }

    IEnumerator RotateDoor(Quaternion from, Quaternion to)
    {
        isAnimating = true;
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