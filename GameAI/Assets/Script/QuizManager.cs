using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class DialogueLine
{
    public string speaker; 
    [TextArea(2, 5)] public string text;    
    public Sprite characterSprite; 
}

[System.Serializable]
public class QuizQuestion
{
    [TextArea(2, 4)] public string questionText;
    public string optionA;
    public string optionB;
    public string optionC;
    public int correctAnswer; 
    [TextArea(2, 4)] public string blinkyHint; 
}

public class QuizManager : MonoBehaviour
{
    [Header("UI - Caixas de Nome")]
    public GameObject nameBoxLeft;   public TextMeshProUGUI nameTextLeft;
    public GameObject nameBoxRight;  public TextMeshProUGUI nameTextRight; 
    public GameObject nameBoxCenter; public TextMeshProUGUI nameTextCenter; 

    [Header("UI - Dialogue Box (Caixa Rosa)")]
    public GameObject dialogueBox;     
    public TextMeshProUGUI mainText;   
    public GameObject nextButton;      
    public GameObject backButton;      

    [Header("UI - Quiz Panels")]
    public GameObject quizPanelToHide;    
    public TextMeshProUGUI quizQuestionText; 
    public GameObject answerButtonsPanel; 
    public TextMeshProUGUI textOptionA;
    public TextMeshProUGUI textOptionB;
    public TextMeshProUGUI textOptionC;
    public GameObject errorPanel;         

    [Header("Personagens Visuais")]
    public RectTransform ameRect;  public Image ameImage;         
    public RectTransform npcRect;  public Image npcImage; 
    public RectTransform blinkyRect; public Image blinkyImage; 

    private List<DialogueLine> currentLines; 
    private int currentLineIndex = 0;        
    private NPCDialogue currentActiveNPC; 
    
    private bool isQuizMode = false;
    private bool isStartingQuiz = false; 
    private List<QuizQuestion> currentQuiz;
    private int currentQuestionIndex = 0;
    private bool usedBlinkyOnCurrent = false;
    public int totalScore = 0; 
    
    // A MÁQUINA DE ESTADOS COMPLETA
    private enum QuizState { None, Question, WrongAna, AskHelp, AmeAsking, BlinkyExplaining, CorrectAna }
    private QuizState quizState = QuizState.None;

    public void StartDialogue(List<DialogueLine> linesFromNPC, NPCDialogue npc)
    {
        isQuizMode = false; 
        currentLines = linesFromNPC;
        currentLineIndex = 0; 
        currentActiveNPC = npc; 

        gameObject.SetActive(true); 
        
        if (dialogueBox != null) dialogueBox.SetActive(true);
        if (quizPanelToHide != null) quizPanelToHide.SetActive(false);
        
        if (backButton != null) backButton.SetActive(false);
        if (nextButton != null) nextButton.SetActive(true);

        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (currentLineIndex < currentLines.Count)
        {
            DialogueLine currentLine = currentLines[currentLineIndex];
            SetQuizSpeaker(currentLine.speaker);
            if (mainText != null) mainText.text = currentLine.text;
            
            currentLineIndex++; 
            if (backButton != null) backButton.SetActive(currentLineIndex > 1);
        }
        else
        {
            EndDialogue();
        }
    }

    public void DisplayPreviousLine()
    {
        if (isQuizMode) return; 
        if (currentLineIndex > 1) { currentLineIndex -= 2; DisplayNextLine(); }
    }

    public void EndDialogue()
    {
        // Puxa o quiz se a introdução acabou
        if (isStartingQuiz)
        {
            isStartingQuiz = false;
            StartQuizLoop();
            return; 
        }

        gameObject.SetActive(false); 
        if (currentActiveNPC != null)
        {
            currentActiveNPC.OnDialogueFinished();
            currentActiveNPC = null; 
        }
    }

    public void StartQuiz(List<DialogueLine> introDialogue, List<QuizQuestion> questions, NPCDialogue npc)
    {
        currentQuiz = questions;
        currentActiveNPC = npc;
        totalScore = 0;

        if (introDialogue != null && introDialogue.Count > 0)
        {
            isStartingQuiz = true; 
            StartDialogue(introDialogue, npc); 
        }
        else
        {
            StartQuizLoop(); 
        }
    }

    private void StartQuizLoop()
    {
        isQuizMode = true;
        currentQuestionIndex = 0;
        gameObject.SetActive(true);
        ShowQuestion();
    }

    private void ShowQuestion()
    {
        quizState = QuizState.Question;
        usedBlinkyOnCurrent = false;

        if (dialogueBox != null) dialogueBox.SetActive(false);
        if (quizPanelToHide != null) quizPanelToHide.SetActive(true);

        if (quizQuestionText != null) quizQuestionText.gameObject.SetActive(true);
        if (answerButtonsPanel != null) answerButtonsPanel.SetActive(true);
        if (errorPanel != null) errorPanel.SetActive(false);

        if (currentQuestionIndex < currentQuiz.Count)
        {
            QuizQuestion q = currentQuiz[currentQuestionIndex];
            SetQuizSpeaker("Ana");
            
            if (quizQuestionText != null) quizQuestionText.text = q.questionText;
            if(textOptionA != null) textOptionA.text = q.optionA;
            if(textOptionB != null) textOptionB.text = q.optionB;
            if(textOptionC != null) textOptionC.text = q.optionC;
        }
        else
        {
            EndDialogue(); // Fecha tudo quando o Quiz acaba
        }
    }

   public void OnNextClicked()
    {
        if (!isQuizMode) { DisplayNextLine(); return; }

        if (quizState == QuizState.CorrectAna)
        {
            currentQuestionIndex++; // Acertou, vai pra próxima pergunta
            ShowQuestion();
        }
        else if (quizState == QuizState.WrongAna)
        {
            // ERROU! Passo 2: O jogador clicou na seta, então a Ana oferece ajuda
            quizState = QuizState.AskHelp;
            SetQuizSpeaker("Ana");
            
            // --- AS DUAS MUDANÇAS AQUI ---
            // 1. Esvazia o texto principal para ele não sobrepor o texto do seu ErrorPanel
            if (mainText != null) mainText.text = ""; 
            // 2. Esconde a setinha de Voltar (BackButton)
            if (backButton != null) backButton.SetActive(false); 
            
            if (nextButton != null) nextButton.SetActive(false); // Esconde a setinha de avançar
            
            // Prepara a tela: Liga o seu ErrorPanel e esconde o resto
            if (quizPanelToHide != null) quizPanelToHide.SetActive(true);
            if (quizQuestionText != null) quizQuestionText.gameObject.SetActive(false); 
            if (answerButtonsPanel != null) answerButtonsPanel.SetActive(false);        
            if (errorPanel != null) errorPanel.SetActive(true);                         
        }
        else if (quizState == QuizState.AmeAsking)
        {
            quizState = QuizState.BlinkyExplaining;
            SetQuizSpeaker("Blinky");
            mainText.text = currentQuiz[currentQuestionIndex].blinkyHint;
        }
        else if (quizState == QuizState.BlinkyExplaining)
        {
            ShowQuestion(); 
            usedBlinkyOnCurrent = true;
        }
    }

    public void OnAnswerSelected(int optionIndex)
    {
        if (answerButtonsPanel != null) answerButtonsPanel.SetActive(false);

        if (optionIndex == currentQuiz[currentQuestionIndex].correctAnswer)
        {
            // ACERTOU! 
            int xpGanho = usedBlinkyOnCurrent ? 20 : 50;
            totalScore += xpGanho; 
            XPManager.instance.AddXP(1); 
            
            quizState = QuizState.CorrectAna;
            if (quizPanelToHide != null) quizPanelToHide.SetActive(false);
            if (dialogueBox != null) dialogueBox.SetActive(true);
            
            SetQuizSpeaker("Ana");
            mainText.text = "Isso mesmo! Você ganhou " + xpGanho + " XP!";
            if (nextButton != null) nextButton.SetActive(true);
        }
        else
        {
            // ERROU! Passo 1: Esconde o Quiz, liga a caixa rosa com o aviso da Ana
            quizState = QuizState.WrongAna;
            
            if (quizPanelToHide != null) quizPanelToHide.SetActive(false);
            if (dialogueBox != null) dialogueBox.SetActive(true);
            
            SetQuizSpeaker("Ana");
            mainText.text = "Poxa, infelizmente você errou, mas não se preocupe!";
            
            if (nextButton != null) nextButton.SetActive(true); // Mostra a setinha
        }
    }
    public void OnCallBlinky()
    {
        if (errorPanel != null) errorPanel.SetActive(false);
        quizState = QuizState.AmeAsking;
        
        if (quizPanelToHide != null) quizPanelToHide.SetActive(false);
        if (dialogueBox != null) dialogueBox.SetActive(true);

        SetQuizSpeaker("Ame");
        mainText.text = "Quero ajuda do Blinky Bunny, por favor!";
        if (nextButton != null) nextButton.SetActive(true);
    }

    public void OnSkipQuestion()
    {
        if (errorPanel != null) errorPanel.SetActive(false);
        currentQuestionIndex++;
        ShowQuestion();
    }

    private void SetQuizSpeaker(string speaker)
    {
        if (nameBoxLeft != null) nameBoxLeft.SetActive(false);
        if (nameBoxRight != null) nameBoxRight.SetActive(false);
        if (nameBoxCenter != null) nameBoxCenter.SetActive(false);

        HighlightCharacter(ameRect, ameImage, false);
        HighlightCharacter(npcRect, npcImage, false);
        HighlightCharacter(blinkyRect, blinkyImage, false);

        if (speaker == "Ame")
        {
            if (nameBoxLeft != null) nameBoxLeft.SetActive(true);
            if (nameTextLeft != null) nameTextLeft.text = speaker;
            HighlightCharacter(ameRect, ameImage, true);
        }
        else if (speaker == "Blinky")
        {
            if (nameBoxCenter != null) nameBoxCenter.SetActive(true);
            if (nameTextCenter != null) nameTextCenter.text = speaker;
            HighlightCharacter(blinkyRect, blinkyImage, true);
        }
        else 
        {
            if (nameBoxRight != null) nameBoxRight.SetActive(true);
            if (nameTextRight != null) nameTextRight.text = speaker;
            HighlightCharacter(npcRect, npcImage, true);
        }
    }

    private void HighlightCharacter(RectTransform rect, Image img, bool isSpeaking)
    {
        if (rect != null && img != null)
        {
            if (isSpeaking)
            {
                rect.localScale = new Vector3(1.1f, 1.1f, 1f); img.color = Color.white; rect.SetAsLastSibling(); 
            }
            else
            {
                rect.localScale = new Vector3(0.9f, 0.9f, 1f); img.color = new Color(0.6f, 0.6f, 0.6f, 1f); 
            }
        }
    }
}