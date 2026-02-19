using UnityEngine;
using System.Collections.Generic;

public class NPCDialogue : MonoBehaviour
{
    [Header("NPC/Object Texts")]
    public List<DialogueLine> introLines;
    
    [Header("NPC Quiz (Leave empty if only conversation)")]
    public List<QuestionData> quizQuestions;
    public int xpToPass = 100;

    [Header("Drag QuizSystem here")]
    public QuizManager quizManager;

    // 1. Ativa por aproximação (Trigger)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartConversation();
        }
    }

    // 2. Ativa por clique (Mouse)
    private void OnMouseDown()
    {
        StartConversation();
    }

    void StartConversation()
    {
        // Se a tela não estiver aberta, manda os dados para ela e abre!
        if (quizManager != null && !quizManager.gameObject.activeSelf)
        {
            quizManager.StartConversation(introLines, quizQuestions, xpToPass);
        }
    }
}