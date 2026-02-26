using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class DialogueLine
{
    public string speaker; 
    [TextArea(2, 5)]
    public string text;    
    public Sprite characterSprite; 
}

public class QuizManager : MonoBehaviour
{
    [Header("UI - Name Box Left (Ame)")]
    public GameObject nameBoxLeft;   
    public TextMeshProUGUI nameTextLeft;
    
    [Header("UI - Name Box Right (NPCs)")]
    public GameObject nameBoxRight;       
    public TextMeshProUGUI nameTextRight; 
    
    [Header("UI - Dialogue Box")]
    public GameObject dialogueBox;     
    public TextMeshProUGUI mainText;   
    public GameObject nextButton;      
    public GameObject backButton;      // <-- NOVO: O botão de voltar!

    [Header("UI - Esconder Quiz")]
    public GameObject quizPanelToHide; 

    [Header("Personagens Visuais (Animação)")]
    public RectTransform ameRect;  
    public Image ameImage;         
    public RectTransform npcRect;  
    public Image npcImage;         

    // Variáveis Internas
    private List<DialogueLine> currentLines; 
    private int currentLineIndex = 0;        
    private NPCDialogue currentActiveNPC; 

    public void StartDialogue(List<DialogueLine> linesFromNPC, NPCDialogue npc)
    {
        currentLines = linesFromNPC;
        currentLineIndex = 0; 
        currentActiveNPC = npc; 

        gameObject.SetActive(true); 
        dialogueBox.SetActive(true);
        nextButton.SetActive(true);
        
        // Esconde o botão de voltar na primeira fala
        if (backButton != null) backButton.SetActive(false);

        if (quizPanelToHide != null) quizPanelToHide.SetActive(false);

        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (currentLineIndex < currentLines.Count)
        {
            DialogueLine currentLine = currentLines[currentLineIndex];

            // SE A AME ESTIVER FALANDO
            if (currentLine.speaker == "Ame")
            {
                if (nameBoxLeft != null) nameBoxLeft.SetActive(true);
                if (nameTextLeft != null) nameTextLeft.text = currentLine.speaker;
                if (nameBoxRight != null) nameBoxRight.SetActive(false);

                HighlightCharacter(ameRect, ameImage, true);
                HighlightCharacter(npcRect, npcImage, false);

                if (currentLine.characterSprite != null && ameImage != null)
                    ameImage.sprite = currentLine.characterSprite;
            }
            // SE O NPC ESTIVER FALANDO
            else 
            {
                if (nameBoxRight != null) nameBoxRight.SetActive(true);
                if (nameTextRight != null) nameTextRight.text = currentLine.speaker;
                if (nameBoxLeft != null) nameBoxLeft.SetActive(false);

                HighlightCharacter(ameRect, ameImage, false);
                HighlightCharacter(npcRect, npcImage, true);

                if (currentLine.characterSprite != null && npcImage != null)
                    npcImage.sprite = currentLine.characterSprite;
            }

            mainText.text = currentLine.text;
            currentLineIndex++; // Prepara para a próxima fala

            // LIGA O BOTÃO DE VOLTAR: Se já passamos da primeira fala, ele aparece!
            if (backButton != null)
            {
                backButton.SetActive(currentLineIndex > 1);
            }
        }
        else
        {
            EndDialogue();
        }
    }

    // --- NOVA FUNÇÃO: VOLTAR O TEXTO ---
    public void DisplayPreviousLine()
    {
        // Só volta se não estivermos na primeira fala
        if (currentLineIndex > 1)
        {
            // Como o 'Next' soma 1 para o futuro, subtraímos 2 para ir para o passado
            currentLineIndex -= 2; 
            
            // Manda exibir a fala novamente
            DisplayNextLine(); 
        }
    }

    private void HighlightCharacter(RectTransform rect, Image img, bool isSpeaking)
    {
        if (rect != null && img != null)
        {
            if (isSpeaking)
            {
                rect.localScale = new Vector3(1.1f, 1.1f, 1f); 
                img.color = Color.white;
                rect.SetAsLastSibling(); 
            }
            else
            {
                rect.localScale = new Vector3(0.9f, 0.9f, 1f); 
                img.color = new Color(0.6f, 0.6f, 0.6f, 1f); 
            }
        }
    }

    public void EndDialogue()
    {
        gameObject.SetActive(false); 

        if (currentActiveNPC != null)
        {
            currentActiveNPC.OnDialogueFinished();
            currentActiveNPC = null; 
        }
    }
}