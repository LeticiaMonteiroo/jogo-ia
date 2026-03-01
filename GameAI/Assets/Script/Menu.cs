using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    
    public void GoToPhase1()
    {
        SceneManager.LoadScene("Phase_1");
    }

    
    public void GoToOptions()
    {
        SceneManager.LoadScene("Options");
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
    
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Saiu do jogo");
    }
}