using UnityEngine;
using UnityEngine.UI; 
using System.Collections.Generic;

public class NPCDialogue : MonoBehaviour
{
    [Header("Identificação de Save")]
    public string npcSaveID = "Ana_Fase1"; 

    [Header("Dialogue Content")]
    public List<DialogueLine> lines; 

    [Header("Quiz Content (O que faremos depois)")]
    // public List<QuizQuestion> quizQuestions; // Deixe comentado ou apague por enquanto se der erro

    [Header("UI Reference")]
    public QuizManager dialogueManager; 

    [Header("Interaction Menu")]
    public GameObject interactionPanel; 
    public Button talkButton;           
    public Button quizButton;           

    [Header("Progresso do Jogador")]
    public int xpRequiredForQuiz = 6;   // <-- NOVO: Quantidade de XP necessária para liberar!
    public bool isQuizUnlocked = false; 

    [Header("XP System")]
    public int xpForTalking = 1;      
    private bool hasGivenXP = false;  

    private void Start()
    {
        if (interactionPanel != null) 
        {
            interactionPanel.SetActive(false);
        }

        if (PlayerPrefs.GetInt(npcSaveID + "_HasGivenXP", 0) == 1)
        {
            hasGivenXP = true;
        }
    }

    // 1. Quando a Ame ENTRA na área da Ana
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // --- A MÁGICA DO DESBLOQUEIO ---
            // A Ana pergunta ao XPManager se a Ame já tem 6 ou mais de XP
            if (XPManager.instance != null && XPManager.instance.currentXP >= xpRequiredForQuiz)
            {
                isQuizUnlocked = true; // Libera o Quiz!
            }

            // Liga o menu de botões
            if (interactionPanel != null)
            {
                interactionPanel.SetActive(true);
                
                // Define se o botão do Quiz pode ser clicado (brilhante) ou não (apagado)
                if (quizButton != null)
                {
                    quizButton.interactable = isQuizUnlocked; 
                }
            }
        }
    }

    // 2. Quando a Ame SAI da área da Ana
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (interactionPanel != null)
            {
                interactionPanel.SetActive(false);
            }
        }
    }

    // --- FUNÇÕES DOS BOTÕES ---
    public void OnTalkClicked()
    {
        interactionPanel.SetActive(false); 

        if (dialogueManager != null && !dialogueManager.gameObject.activeSelf)
        {
            dialogueManager.StartDialogue(lines, this); 
        }
    }

    public void OnQuizClicked()
    {
        if (!isQuizUnlocked) return; 

        interactionPanel.SetActive(false); 
        
        // Por enquanto, apenas avisa no console que funcionou!
        Debug.Log("O botão do Quiz foi clicado! A Ame tem " + XPManager.instance.currentXP + " de XP."); 
    }

    public void OnDialogueFinished()
    {
        if (!hasGivenXP)
        {
            XPManager.instance.AddXP(xpForTalking);
            hasGivenXP = true;
            
            PlayerPrefs.SetInt(npcSaveID + "_HasGivenXP", 1);
            PlayerPrefs.Save();
        }
    }

    // Função para o botão "X" do Menu de Interação
    public void CloseInteraction()
    {
        if (interactionPanel != null)
        {
            interactionPanel.SetActive(false);
        }
    }
}