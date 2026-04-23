using UnityEngine;
using UnityEngine.UI; // Necessário para o Button
using TMPro;

public class ComputerInteraction : MonoBehaviour
{
    [Header("Save Identification")]
    public string objectSaveID = "Computer_Phase1"; 

    [Header("Visual Configuration")]
    public GameObject glowObject; 

    [Header("UI Screens Configuration")]
    public GameObject computerPanel; 
    public GameObject menuScreen;    
    public GameObject detailScreen;  

    [Header("UI Text References")]
    public TextMeshProUGUI titleTextUI; 
    public TextMeshProUGUI bodyTextUI;  

    [Header("Botão Universal")]
    public Button btnClose; // <-- O nosso botão X!

    [Header("Topic 1 Content")]
    public string title1 = "Reconhecimento de fala";
    [TextArea(3, 6)] public string body1 = "Digite a explicação aqui...";

    [Header("Topic 2 Content")]
    public string title2 = "Reconhecimento de imagem";
    [TextArea(3, 6)] public string body2 = "Digite a explicação aqui...";

    [Header("Topic 3 Content")]
    public string title3 = "Classificação de emails";
    [TextArea(3, 6)] public string body3 = "Digite a explicação aqui...";

    [Header("Distance Zones")]
    public float distanceToOpen = 1.5f; 

    [Header("XP Configuration")]
    public int xpAmount = 2; 

    public int topics = 2;
    private bool hasGivenXP = false;

    // --- VARIABLES TO TRACK READING ---
    private bool readTopic1 = false;
    private bool readTopic2 = false;
    private bool readTopic3 = false;

    private Transform playerTransform; 
    private bool hasOpenedThisTime = false; 

    void Start()
    {
        if (glowObject != null) glowObject.SetActive(false);
        if (computerPanel != null) computerPanel.SetActive(false);

        if (PlayerPrefs.GetInt(objectSaveID + "_HasGivenXP", 0) == 1)
        {
            hasGivenXP = true;
            readTopic1 = true;
            readTopic2 = true;
            readTopic3 = true;
        }
    }

    void Update()
    {
        if (playerTransform != null && !hasOpenedThisTime && computerPanel != null)
        {
            float currentDistance = Vector2.Distance(transform.position, playerTransform.position);

            if (currentDistance <= distanceToOpen)
            {
                // --- A MÁGICA DO BOTÃO UNIVERSAL ---
                if (btnClose != null)
                {
                    // 1. Faz o botão esquecer qualquer objeto que ele fechou antes
                    btnClose.onClick.RemoveAllListeners(); 
                    
                    // 2. Avisa o botão que agora ele deve conversar com ESTE PC aqui
                    btnClose.onClick.AddListener(CloseComputerAndReward); 
                }
                // -----------------------------------

                OpenComputer();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (glowObject != null) glowObject.SetActive(true);
            playerTransform = collision.transform; 
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (glowObject != null) glowObject.SetActive(false);
            if (computerPanel != null) computerPanel.SetActive(false);
            
            playerTransform = null; 
            hasOpenedThisTime = false; 
        }
    }

    private void OpenComputer()
    {
        computerPanel.SetActive(true);
        
        menuScreen.SetActive(true);
        if(detailScreen != null) detailScreen.SetActive(false);

        hasOpenedThisTime = true; 
    }

    // --- FUNCTIONS FOR THE 3 MENU BUTTONS ---
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
        
        menuScreen.SetActive(false);
        detailScreen.SetActive(true);
    }

    public void BackToMenu()
    {
        detailScreen.SetActive(false);
        menuScreen.SetActive(true);
    }

    public void CloseComputerAndReward()
    {
        computerPanel.SetActive(false); 

        if (topics < 3){
            readTopic3 = true;
        }

        if (!hasGivenXP && readTopic1 && readTopic2 && readTopic3)
        {
            XPManager.instance.AddXP(xpAmount);
            hasGivenXP = true;
            PlayerPrefs.SetInt(objectSaveID + "_HasGivenXP", 1);
            PlayerPrefs.Save();
            Debug.Log("Leu os 3 tópicos! 2 XP adicionados.");
        }
    }
}