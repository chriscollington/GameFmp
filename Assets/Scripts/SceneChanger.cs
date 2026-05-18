using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public string sceneToLoad;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Release and show mouse cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Load new scene
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}