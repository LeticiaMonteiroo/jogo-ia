using UnityEngine;
using UnityEngine.UI; 

public class ObjectInteraction : MonoBehaviour
{
    [Header("Identificação de Save")]
    public string objectSaveID = "Objeto_Fase1"; 

    [Header("Configuração Visual")]
    public GameObject glowObject; 

    [Header("Configuração da UI (O Painel)")]
    public GameObject panelToOpen; 
    public Image uiImagePlaceholder; 
    public Sprite itemSprite;        
    public Button closeButton; // <-- ELE VOLTOU! Precisamos dele para o botão virar "Universal"

    [Header("Zonas de Distância")]
    public float distanciaParaAbrir = 1.5f; 

    [Header("Configuração de XP")]
    public int xpAmount = 1;
    private bool hasGivenXP = false;

    private Transform playerTransform; 
    private bool hasOpenedThisTime = false; 

    void Start()
    {
        if (glowObject != null) glowObject.SetActive(false);
        if (panelToOpen != null) panelToOpen.SetActive(false);

        if (PlayerPrefs.GetInt(objectSaveID + "_HasGivenXP", 0) == 1)
        {
            hasGivenXP = true;
        }
    }

    void Update()
    {
        if (playerTransform != null && !hasOpenedThisTime && panelToOpen != null)
        {
            float distanciaAtual = Vector2.Distance(transform.position, playerTransform.position);

            if (distanciaAtual <= distanciaParaAbrir)
            {
                // Injeta a imagem
                if (uiImagePlaceholder != null && itemSprite != null)
                {
                    uiImagePlaceholder.sprite = itemSprite;
                    uiImagePlaceholder.SetNativeSize(); 
                }

                // --- A MÁGICA DO BOTÃO UNIVERSAL ---
                if (closeButton != null)
                {
                    // 1. Faz o botão esquecer qualquer objeto que ele fechou antes
                    closeButton.onClick.RemoveAllListeners(); 
                    
                    // 2. Avisa o botão que agora ele deve conversar com ESTE objeto aqui
                    closeButton.onClick.AddListener(ClosePanelAndReward); 
                }
                // -----------------------------------

                panelToOpen.SetActive(true);
                hasOpenedThisTime = true; 
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
            if (panelToOpen != null) panelToOpen.SetActive(false);
            
            playerTransform = null; 
            hasOpenedThisTime = false; 
        }
    }

    public void ClosePanelAndReward()
    {
        panelToOpen.SetActive(false); 

        if (!hasGivenXP)
        {
            XPManager.instance.AddXP(xpAmount);
            hasGivenXP = true;
            PlayerPrefs.SetInt(objectSaveID + "_HasGivenXP", 1);
            PlayerPrefs.Save();
        }
    }
}