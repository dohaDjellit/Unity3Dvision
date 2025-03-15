using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartManager : MonoBehaviour
{
    public void RestartApplication()
    {
        // Reload the currently active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void QuitApplication()
    {
        Debug.Log("Application is quitting..."); // For debugging purposes in the editor

        // Exit the application
        Application.Quit();
    }
}
