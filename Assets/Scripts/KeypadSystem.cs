using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeypadSystem : MonoBehaviour
{
    [Header("UI References")]
    public GameObject keypadUI;
    public Transform buttonGridParent;
    public GameObject buttonPrefab;
    public TMP_Text displayText;
    public Button exitButton;

    [Header("Interaction")]
    public float interactDistance = 3f;
    public KeyCode interactKey = KeyCode.E;

    [Header("UI Prompt (ROOT OBJECT)")]
    public GameObject interactText;

    [Header("Target")]
    public GameObject wallToDisable;

    [Header("Code Settings")]
    public string correctCode = "1234";

    private string currentInput = "";
    private bool isOpen = false;

    private Transform player;
    private Camera cam;

    void Start()
    {
        keypadUI.SetActive(false);
        GenerateKeypad();

        exitButton.onClick.AddListener(CloseKeypad);
        UpdateDisplay();

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        cam = Camera.main;

        if (interactText != null)
        {
            interactText.SetActive(false);

            // 🔥 ensure children (Text TMP) also follow state
            foreach (Transform child in interactText.transform)
                child.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (player == null || cam == null || isOpen) return;

        // 🔥 REAL INTERACTION POINT = collider center
        Collider col = GetComponent<Collider>();
        Vector3 targetPoint = col != null ? col.bounds.center : transform.position;

        float distance = Vector3.Distance(player.position, targetPoint);
        bool isNear = distance <= interactDistance;

        // 🔥 FIXED: proper raycast interaction (no angle bugs)
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        bool isLooking =
            Physics.Raycast(ray, out RaycastHit hit, interactDistance) &&
            hit.transform == transform;

        if (isNear && isLooking)
        {
            ShowInteractUI(true);

            if (Input.GetKeyDown(interactKey))
            {
                OpenKeypad();
            }
        }
        else
        {
            ShowInteractUI(false);
        }
    }

    void ShowInteractUI(bool state)
    {
        if (interactText == null) return;

        interactText.SetActive(state);

        // 🔥 fix Text not showing bug
        foreach (Transform child in interactText.transform)
            child.gameObject.SetActive(state);
    }

    void GenerateKeypad()
    {
        foreach (Transform child in buttonGridParent)
            Destroy(child.gameObject);

        for (int i = 1; i <= 9; i++)
            CreateButton(i.ToString());

        CreateButton("0");
    }

    void CreateButton(string value)
    {
        GameObject btnObj = Instantiate(buttonPrefab, buttonGridParent);
        TMP_Text txt = btnObj.GetComponentInChildren<TMP_Text>();
        Button btn = btnObj.GetComponent<Button>();

        txt.text = value;
        btn.onClick.AddListener(() => PressNumber(value));
    }

    void PressNumber(string number)
    {
        currentInput += number;
        UpdateDisplay();
        CheckCode();
    }

    void CheckCode()
    {
        if (currentInput.Length >= correctCode.Length)
        {
            if (currentInput == correctCode)
                Unlock();
            else
            {
                currentInput = "";
                UpdateDisplay();
            }
        }
    }

    void Unlock()
    {
        Debug.Log("Unlocked!");

        if (wallToDisable != null)
            wallToDisable.SetActive(false);

        CloseKeypad();
    }

    void UpdateDisplay()
    {
        displayText.text = currentInput;
    }

    public void OpenKeypad()
    {
        if (isOpen) return;

        keypadUI.SetActive(true);
        isOpen = true;

        ShowInteractUI(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    public void CloseKeypad()
    {
        keypadUI.SetActive(false);
        isOpen = false;

        currentInput = "";
        UpdateDisplay();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
    }

    public bool IsOpen()
    {
        return isOpen;
    }
}