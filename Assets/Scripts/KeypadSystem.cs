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

    [Header("Target")]
    public GameObject wallToDisable;

    [Header("Code Settings")]
    public string correctCode = "1234";

    private string currentInput = "";
    private bool isOpen = false;

    void Start()
    {
        keypadUI.SetActive(false);
        GenerateKeypad();

        exitButton.onClick.AddListener(CloseKeypad);
        UpdateDisplay();
    }

    // ❌ NO INPUT HERE ANYMORE
    void Update()
    {
        // handled by interaction script only
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
        if (isOpen) return; // extra safety

        keypadUI.SetActive(true);
        isOpen = true;

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