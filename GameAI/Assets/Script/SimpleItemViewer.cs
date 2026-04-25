using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;

public class SimpleItemViewer : MonoBehaviour
{
    [Header("Save & XP Settings")]
    public string objectSaveID = "Objeto_Fase1"; 
    public int xpAmount = 1;
    private bool hasGivenXP = false;

    [Header("UI - The Panel & Content")]
    public GameObject targetPanel; 
    public Image itemImagePlaceholder; 
    public Sprite itemSprite;          
    public Button closeButton; 

    [Header("Eventos de Comunicação")]
    public UnityEvent onClosePanel;

    private void Start()
    {
        if (targetPanel != null) targetPanel.SetActive(false);

        if (PlayerPrefs.GetInt(objectSaveID + "_HasGivenXP", 0) == 1)
        {
            hasGivenXP = true;
        }
    }

    public void OpenViewer()
    {
        if (itemImagePlaceholder != null && itemSprite != null)
        {
            itemImagePlaceholder.sprite = itemSprite;
            itemImagePlaceholder.SetNativeSize(); 
        }

        if (targetPanel != null) targetPanel.SetActive(true);
        
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners(); 
            closeButton.onClick.AddListener(CloseViewerAndReward); 
        }

    }

    public void CloseViewerAndReward()
    {
        if (targetPanel != null) targetPanel.SetActive(false); 

        if (!hasGivenXP)
        {
            if (XPManager.instance != null) XPManager.instance.AddXP(xpAmount);
            hasGivenXP = true;
            PlayerPrefs.SetInt(objectSaveID + "_HasGivenXP", 1);
            PlayerPrefs.Save();
        }

        onClosePanel?.Invoke(); 
    }

}