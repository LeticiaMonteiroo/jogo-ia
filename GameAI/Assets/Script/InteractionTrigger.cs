using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class InteractionTrigger : MonoBehaviour
{
    [Header("Proximity Settings")]
    public float interactionDistance = 1.5f; 
    private Transform playerTransform;
    private bool isInteracting = false; 

    [Header("Visuals")]
    public GameObject glowObject; 

    [Header("UI - The Interaction Button")]
    public Button interactionButton;   
    public Image innerIconImage;       
    public Sprite interactionIcon;     

    [Header("Eventos de Comunicação")]
    public UnityEvent onInteract; 

    private void Start()
    {
        if (glowObject != null) glowObject.SetActive(false);
        if (interactionButton != null) interactionButton.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (playerTransform != null && !isInteracting)
        {
            float currentDistance = Vector2.Distance(transform.position, playerTransform.position);

            if (currentDistance <= interactionDistance)
            {
                if (interactionButton != null && !interactionButton.gameObject.activeSelf)
                    ShowInteractionButton();
            }
            else
            {
                if (interactionButton != null && interactionButton.gameObject.activeSelf)
                    HideInteractionButton();
            }
        }
    }

    private void ShowInteractionButton()
    {
        if (innerIconImage != null && interactionIcon != null)
            innerIconImage.sprite = interactionIcon;

        interactionButton.onClick.RemoveAllListeners(); 
        interactionButton.onClick.AddListener(TriggerInteraction); 
        interactionButton.gameObject.SetActive(true);
    }

    private void HideInteractionButton()
    {
        if (interactionButton != null)
        {
            interactionButton.onClick.RemoveListener(TriggerInteraction);
            interactionButton.gameObject.SetActive(false);
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
            HideInteractionButton();
            playerTransform = null; 
            isInteracting = false;
        }
    }

    private void TriggerInteraction()
    {
        isInteracting = true; 
        HideInteractionButton();
        
        onInteract?.Invoke();
    }

    public void ResetInteraction()
    {
        isInteracting = false;
    }
}