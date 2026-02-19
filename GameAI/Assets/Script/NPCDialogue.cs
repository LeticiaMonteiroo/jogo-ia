using UnityEngine;
using System.Collections.Generic;

public class NPCDialogue : MonoBehaviour
{
    [Header("Dialogue Content")]
    public List<DialogueLine> lines; // A lista de falas

    [Header("UI Reference")]
    public QuizManager dialogueManager; // Arraste o QuizSystem aqui

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Quando a Ame encostar, inicia o diálogo
        if (collision.CompareTag("Player"))
        {
            if (dialogueManager != null && !dialogueManager.gameObject.activeSelf)
            {
                dialogueManager.StartDialogue(lines);
            }
        }
    }
}