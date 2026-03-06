using UnityEngine;
using UnityEngine.UI; 
using System.Collections.Generic;

public class NPCDialogue : MonoBehaviour
{
    [Header("Identificação de Save")]
    public string npcSaveID = "Ana_Fase1"; 

    [Header("Dialogue Content (Conversa Normal)")]
    public List<DialogueLine> lines; 

    [Header("Quiz Content (O Quiz)")]
    public List<DialogueLine> preQuizLines;  // <-- NOVO: A fala da Ana antes do Quiz
    public List<QuizQuestion> quizQuestions; // As perguntas

    [Header("UI Reference")]
    public QuizManager dialogueManager; 

    [Header("Interaction Menu")]
    public GameObject interactionPanel; 
    public Button talkButton;           
    public Button quizButton;           

    [Header("Progresso do Jogador")]
    public int xpRequiredForQuiz = 6;   
    public bool isQuizUnlocked = false; 

    [Header("XP System")]
    public int xpForTalking = 1;      
    private bool hasGivenXP = false;  

    private void Start()
    {
        if (interactionPanel != null) interactionPanel.SetActive(false);
        if (PlayerPrefs.GetInt(npcSaveID + "_HasGivenXP", 0) == 1) hasGivenXP = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (XPManager.instance != null && XPManager.instance.currentXP >= xpRequiredForQuiz)
                isQuizUnlocked = true; 

            if (interactionPanel != null)
            {
                interactionPanel.SetActive(true);
                if (quizButton != null) quizButton.interactable = isQuizUnlocked; 
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (interactionPanel != null) interactionPanel.SetActive(false);
        }
    }

    public void OnTalkClicked()
    {
        interactionPanel.SetActive(false); 
        if (dialogueManager != null)
        {
            dialogueManager.StartDialogue(lines, this); 
        }
    }

    public void OnQuizClicked()
    {
        if (!isQuizUnlocked) return; 
        interactionPanel.SetActive(false); 
        
        if (dialogueManager != null)
        {
            // MUDOU AQUI: Passamos a fala introdutória E as perguntas!
            dialogueManager.StartQuiz(preQuizLines, quizQuestions, this); 
        }
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

    public void CloseInteraction()
    {
        if (interactionPanel != null) interactionPanel.SetActive(false);
    }
}