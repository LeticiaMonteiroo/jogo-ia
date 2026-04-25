using UnityEngine;
using UnityEngine.UI; 
using System.Collections.Generic;
using UnityEngine.Events;

public class NPCDialogue : MonoBehaviour
{
    [Header("Identificação de Save")]
    public string npcSaveID = "Ana_Fase1"; 

    [Header("Dialogue Content (Conversa Normal)")]
    public List<DialogueLine> lines; 

    [Header("Quiz Content (O Quiz)")]
    public List<DialogueLine> preQuizLines;  
    public List<QuizQuestion> quizQuestions; 

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

    [Header("Transição de Cena")]
    public bool hasSceneTransition = false;
    public string nextSceneName = ""; 

    [Header("Communication Events")]
    public UnityEvent onCloseDialogue; 

    private void Start()
    {
        if (interactionPanel != null) interactionPanel.SetActive(false);
        if (PlayerPrefs.GetInt(npcSaveID + "_HasGivenXP", 0) == 1) hasGivenXP = true;
    }

    public void OpenInteractionMenu()
    {
        if (XPManager.instance != null && XPManager.instance.currentXP >= xpRequiredForQuiz)
        {
            isQuizUnlocked = true; 
        }

        if (interactionPanel != null)
        {
            interactionPanel.SetActive(true);
            if (quizButton != null) quizButton.interactable = isQuizUnlocked; 
        }
    }

    public void OnTalkClicked()
    {
        if (interactionPanel != null) interactionPanel.SetActive(false); 
        
        if (dialogueManager != null)
        {
            dialogueManager.StartDialogue(lines, this, hasSceneTransition, nextSceneName); 
        }
    }

    public void OnQuizClicked()
    {
        if (!isQuizUnlocked) return; 
        
        if (interactionPanel != null) interactionPanel.SetActive(false); 
        
        if (dialogueManager != null)
        {
            dialogueManager.StartQuiz(preQuizLines, quizQuestions, this); 
        }
    }

    public void OnDialogueFinished()
    {
        if (!hasGivenXP)
        {
            if (XPManager.instance != null) XPManager.instance.AddXP(xpForTalking);
            hasGivenXP = true;
            PlayerPrefs.SetInt(npcSaveID + "_HasGivenXP", 1);
            PlayerPrefs.Save();
        }
    }

    public void CloseInteraction()
    {
        if (interactionPanel != null) interactionPanel.SetActive(false);
        
        onCloseDialogue?.Invoke(); 
    }
}