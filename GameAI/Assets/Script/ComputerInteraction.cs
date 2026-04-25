using UnityEngine;
using UnityEngine.UI; 
using TMPro;
using UnityEngine.Events;

public class ComputerInteraction : MonoBehaviour
{
    [Header("Save Identification")]
    public string objectSaveID = "Computer_Phase1"; 

    [Header("UI Screens Configuration")]
    public GameObject computerPanel; 
    public GameObject menuScreen;    
    public GameObject detailScreen;  

    [Header("Botão de Fechar")]
    public Button closeButton; 

    [Header("UI Text References")]
    public TextMeshProUGUI titleTextUI; 
    public TextMeshProUGUI bodyTextUI;  

    [Header("Topic 1 Content")]
    public string title1 = "Reconhecimento de fala";
    [TextArea(3, 6)] public string body1 = "Digite a explicação aqui...";

    [Header("Topic 2 Content")]
    public string title2 = "Reconhecimento de imagem";
    [TextArea(3, 6)] public string body2 = "Digite a explicação aqui...";

    [Header("Topic 3 Content")]
    public string title3 = "Classificação de emails";
    [TextArea(3, 6)] public string body3 = "Digite a explicação aqui...";

    [Header("XP Configuration")]
    public int xpAmount = 2; 
    
    public int topics = 2;

    private bool hasGivenXP = false;
    private bool readTopic1 = false;
    private bool readTopic2 = false;
    private bool readTopic3 = false;

    [Header("Communication Events")]
    public UnityEvent onCloseComputer; 

    void Start()
    {
        if (computerPanel != null) computerPanel.SetActive(false);

        if (PlayerPrefs.GetInt(objectSaveID + "_HasGivenXP", 0) == 1)
        {
            hasGivenXP = true;
            readTopic1 = true;
            readTopic2 = true;
            readTopic3 = true;
        }
    }

    public void OpenComputer()
    {
        if (computerPanel != null) computerPanel.SetActive(true);
        if (menuScreen != null) menuScreen.SetActive(true);
        if (detailScreen != null) detailScreen.SetActive(false);

        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners(); 
            closeButton.onClick.AddListener(CloseComputerAndReward); 
        }
    }

    public void OpenTopic1()
    {
        readTopic1 = true; 
        ShowDetailScreen(title1, body1);
    }

    public void OpenTopic2()
    {
        readTopic2 = true; 
        ShowDetailScreen(title2, body2);
    }

    public void OpenTopic3()
    {
        readTopic3 = true; 
        ShowDetailScreen(title3, body3);
    }

    private void ShowDetailScreen(string title, string body)
    {
        if (titleTextUI != null) titleTextUI.text = title;
        if (bodyTextUI != null) bodyTextUI.text = body;
        
        if (menuScreen != null) menuScreen.SetActive(false);
        if (detailScreen != null) detailScreen.SetActive(true);
    }

    public void BackToMenu()
    {
        if (detailScreen != null) detailScreen.SetActive(false);
        if (menuScreen != null) menuScreen.SetActive(true);
    }

    public void CloseComputerAndReward()
    {
        if (computerPanel != null) computerPanel.SetActive(false); 

        if (topics < 3)
        {
            readTopic3 = true;
        }

        if (!hasGivenXP && readTopic1 && readTopic2 && readTopic3)
        {
            if (XPManager.instance != null)
            {
                XPManager.instance.AddXP(xpAmount);
            }
            hasGivenXP = true;
            PlayerPrefs.SetInt(objectSaveID + "_HasGivenXP", 1);
            PlayerPrefs.Save();
            
            Debug.Log("Leu os 3 tópicos! 2 XP adicionados.");
        }

        onCloseComputer?.Invoke(); 
    }
}