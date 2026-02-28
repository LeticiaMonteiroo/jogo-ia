using UnityEngine;
using UnityEngine.SceneManagement; // Library to manage scene switching

public class BlinkyLobbyInteract : MonoBehaviour
{
    [Header("Blinky UI")]
    public GameObject blinkyLobbyPanel; // Drag the Blinky text box here
    
    [Header("Scene Configuration")]
    public string lobbySceneName = "Lobby"; // Type the EXACT name of the Lobby scene here

    private void Start()
    {
        // Ensures the panel starts hidden
        if (blinkyLobbyPanel != null) blinkyLobbyPanel.SetActive(false);
    }

    // When Ame ENTERS the area around Blinky
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (blinkyLobbyPanel != null) blinkyLobbyPanel.SetActive(true);
        }
    }

    // When Ame LEAVES the area around Blinky
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (blinkyLobbyPanel != null) blinkyLobbyPanel.SetActive(false);
        }
    }

    // --- BUTTON FUNCTIONS ---

    public void ReturnToLobby()
    {
        // Loads the Lobby scene
        SceneManager.LoadScene(lobbySceneName); 
    }

    public void ClosePanel()
    {
        // Hides the panel if she doesn't want to go now
        if (blinkyLobbyPanel != null) blinkyLobbyPanel.SetActive(false);
    }
}