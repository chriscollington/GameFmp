using UnityEngine;

public class GearController : MonoBehaviour
{
    [Header("Keyboard Input")]
    public KeyCode toggleKey = KeyCode.E;

    [Header("Interaction")]
    public Transform player;
    public Transform lever;
    public float activationDistance = 3f;

    [Header("Gear Settings")]
    public Transform[] gears;
    public float rotationSpeed = 100f;
    public Vector3 rotationAxis = Vector3.up; // Set this in Inspector

    private bool isRotating = false;

    void Update()
    {
        bool isNearLever = Vector3.Distance(player.position, lever.position) <= activationDistance;

        // Keyboard input ONLY when near
        if (isNearLever && Input.GetKeyDown(toggleKey))
        {
            ToggleLever();
        }

        // Rotate gears
        if (isRotating)
        {
            foreach (Transform gear in gears)
            {
                if (gear != null)
                {
                    gear.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
                }
            }
        }
    }

    public void ToggleLever()
    {
        isRotating = !isRotating;
    }

    public bool IsPlayerNear()
    {
        return Vector3.Distance(player.position, lever.position) <= activationDistance;
    }
}