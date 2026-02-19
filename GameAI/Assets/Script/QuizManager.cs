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
    public Sprite characterSprite; // <-- NOVO: A imagem da expressão para esta fala!
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

    public void StartDialogue(List<DialogueLine> linesFromNPC)
    {
        currentLines = linesFromNPC;
        currentLineIndex = 0; 

        gameObject.SetActive(true); 
        dialogueBox.SetActive(true);
        nextButton.SetActive(true);

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

                // TROCA A IMAGEM DA AME SE TIVER UMA NOVA NESSA FALA
                if (currentLine.characterSprite != null && ameImage != null)
                {
                    ameImage.sprite = currentLine.characterSprite;
                }
            }
            // SE O NPC ESTIVER FALANDO
            else 
            {
                if (nameBoxRight != null) nameBoxRight.SetActive(true);
                if (nameTextRight != null) nameTextRight.text = currentLine.speaker;
                if (nameBoxLeft != null) nameBoxLeft.SetActive(false);

                HighlightCharacter(ameRect, ameImage, false);
                HighlightCharacter(npcRect, npcImage, true);

                // TROCA A IMAGEM DO NPC SE TIVER UMA NOVA NESSA FALA
                if (currentLine.characterSprite != null && npcImage != null)
                {
                    npcImage.sprite = currentLine.characterSprite;
                }
            }

            mainText.text = currentLine.text;
            currentLineIndex++;
        }
        else
        {
            EndDialogue();
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

    private void EndDialogue()
    {
        gameObject.SetActive(false); 
    }
}