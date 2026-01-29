using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public string scena;
    public string secondScena;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void StartGame()
    {
        SceneManager.LoadScene(scena);
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene(secondScena);
    }
    public void QuitGame()
    {
        //Unity
       // UnityEditor.EditorApplication.isPlaying = false;
       Application.Quit();

    }
}
