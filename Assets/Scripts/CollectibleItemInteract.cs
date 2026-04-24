using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CollectibleItemInteract : MonoBehaviour
{
    [Header("Item Settings")]
    public string itemID = "red_key";
    public KeyCode interactKey = KeyCode.E;

    [Header("Interaction")]
    public float interactDistance = 3f;
    public Vector3 interactionOffset;
    [Range(0f, 1f)] public float lookThreshold = 0.7f;

    [Header("UI")]
    public GameObject interactText;
    public GameObject pickupText;

    private Transform player;
    private Camera cam;

    private bool collected;

    private Renderer[] renderers;
    private Collider[] colliders;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        cam = Camera.main;

        if (interactText != null) interactText.SetActive(false);
        if (pickupText != null) pickupText.SetActive(false);

        renderers = GetComponentsInChildren<Renderer>();
        colliders = GetComponentsInChildren<Collider>();
    }

    void Update()
    {
        if (collected || player == null || cam == null) return;

        Vector3 checkPos = transform.position + transform.TransformDirection(interactionOffset);

        float dist = Vector3.Distance(player.position, checkPos);
        bool near = dist <= interactDistance;

        Vector3 dir = (checkPos - cam.transform.position).normalized;
        bool looking = Vector3.Dot(cam.transform.forward, dir) > lookThreshold;

        if (near && looking)
        {
            if (interactText != null)
                interactText.SetActive(true);

            if (Input.GetKeyDown(interactKey))
                Collect();
        }
        else
        {
            if (interactText != null)
                interactText.SetActive(false);
        }
    }

    void Collect()
    {
        collected = true;

        // inventory
        PlayerInventory.instance?.AddItem(itemID);

        if (interactText != null)
            interactText.SetActive(false);

        //  instant visual hide (no lag)
        SetVisible(false);

        // UI runs separately
        if (pickupText != null)
            StartCoroutine(PickupRoutine());
        else
            Destroy(gameObject);
    }

    void SetVisible(bool state)
    {
        foreach (var r in renderers)
            r.enabled = state;

        foreach (var c in colliders)
            c.enabled = state;
    }

    IEnumerator PickupRoutine()
    {
        pickupText.SetActive(true);

        yield return new WaitForSeconds(2f);

        pickupText.SetActive(false);

        //  wait 1 frame before destroy (prevents spike)
        yield return null;

        Destroy(gameObject);
    }
}