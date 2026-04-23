using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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
    public GameObject nameBoxLeft; public TextMeshProUGUI nameTextLeft;
    public GameObject nameBoxRight; public TextMeshProUGUI nameTextRight;
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
    public RectTransform ameRect; public Image ameImage;
    public RectTransform npcRect; public Image npcImage;
    public RectTransform blinkyRect; public Image blinkyImage;

    private List<DialogueLine> currentLines;
    private int currentLineIndex = 0;
    private NPCDialogue currentActiveNPC;

    private bool isQuizMode = false;
    private bool isStartingQuiz = false;
    private List<QuizQuestion> currentQuiz;
    private int currentQuestionIndex = 0;

    // Variáveis de controle do Quiz
    private bool usedBlinkyOnCurrent = false;
    private int currentQuestionAttempts = 0;
    public int totalScore = 0;

    // Máquina de estados
    private enum QuizState { None, Question, WrongAna, AskHelp, AmeAsking, BlinkyExplaining, CorrectAna, FailedAna, EndSummary, EndResult }
    private QuizState quizState = QuizState.None;

    public GameObject changeSceneButton;
    private bool hasSceneTransition = false;
    private string nextSceneName = "";

    public void StartDialogue(List<DialogueLine> linesFromNPC, NPCDialogue npc, bool sceneTransition = false, string nextScene = "")
    {
        isQuizMode = false;
        currentLines = linesFromNPC;
        currentLineIndex = 0;
        currentActiveNPC = npc;

        hasSceneTransition = sceneTransition;
        nextSceneName = nextScene;

        gameObject.SetActive(true);

        if (dialogueBox != null) dialogueBox.SetActive(true);
        if (quizPanelToHide != null) quizPanelToHide.SetActive(false);

        if (backButton != null) backButton.SetActive(false);
        if (nextButton != null) nextButton.SetActive(true);

        if (changeSceneButton != null)
        {
            changeSceneButton.SetActive(false);
        }

        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (currentLineIndex < currentLines.Count)
        {
            DialogueLine currentLine = currentLines[currentLineIndex];
            SetQuizSpeaker(currentLine.speaker, currentLine.characterSprite);
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
        if (hasSceneTransition && changeSceneButton != null)
        {
            changeSceneButton.SetActive(true);
            if (mainText != null) mainText.text = "";

            if (nextButton != null) nextButton.SetActive(false);
        }
        else
        {
            if (dialogueBox != null) dialogueBox.SetActive(false);
            if (changeSceneButton != null) changeSceneButton.SetActive(false);
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

    }

    public void GoToNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
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
        usedBlinkyOnCurrent = false;
        currentQuestionAttempts = 0;
        gameObject.SetActive(true);
        ShowQuestion();
    }

    private void ShowQuestion()
    {
        quizState = QuizState.Question;

        if (blinkyRect != null) blinkyRect.gameObject.SetActive(false);

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
            if (textOptionA != null) textOptionA.text = q.optionA;
            if (textOptionB != null) textOptionB.text = q.optionB;
            if (textOptionC != null) textOptionC.text = q.optionC;
        }
        else
        {
            // O QUIZ ACABOU! Mostra a tela de resumo de pontos
            quizState = QuizState.EndSummary;

            if (quizPanelToHide != null) quizPanelToHide.SetActive(false);
            if (dialogueBox != null) dialogueBox.SetActive(true);

            SetQuizSpeaker("Ana");
            if (mainText != null) mainText.text = "Chegamos ao fim do nosso quiz, Ame! Você conseguiu " + totalScore + " XP e para passar da fase precisa de 100 XP.";

            if (nextButton != null) nextButton.SetActive(true);
        }
    }

    public void OnNextClicked()
    {
        if (!isQuizMode) { DisplayNextLine(); return; }

        if (quizState == QuizState.CorrectAna || quizState == QuizState.FailedAna)
        {
            currentQuestionIndex++;
            usedBlinkyOnCurrent = false;
            currentQuestionAttempts = 0;
            ShowQuestion();
        }
        else if (quizState == QuizState.WrongAna)
        {
            quizState = QuizState.AskHelp;
            SetQuizSpeaker("Ana");
            if (mainText != null) mainText.text = "";
            if (backButton != null) backButton.SetActive(false);
            if (nextButton != null) nextButton.SetActive(false);

            if (quizPanelToHide != null) quizPanelToHide.SetActive(true);
            if (quizQuestionText != null) quizQuestionText.gameObject.SetActive(false);
            if (answerButtonsPanel != null) answerButtonsPanel.SetActive(false);
            if (errorPanel != null) errorPanel.SetActive(true);
        }
        else if (quizState == QuizState.AmeAsking)
        {
            quizState = QuizState.BlinkyExplaining;

            if (blinkyRect != null) blinkyRect.gameObject.SetActive(true);
            if (ameRect != null) ameRect.gameObject.SetActive(true);
            if (npcRect != null) npcRect.gameObject.SetActive(true);

            SetQuizSpeaker("Blinky");
            if (mainText != null) mainText.text = currentQuiz[currentQuestionIndex].blinkyHint;
            if (nextButton != null) nextButton.SetActive(true);
        }
        else if (quizState == QuizState.BlinkyExplaining)
        {
            usedBlinkyOnCurrent = true;
            if (blinkyRect != null) blinkyRect.gameObject.SetActive(false);
            ShowQuestion();
        }
        else if (quizState == QuizState.EndSummary)
        {
            quizState = QuizState.EndResult;
            SetQuizSpeaker("Ana");

            if (totalScore >= 100)
            {
                if (mainText != null) mainText.text = "Parabéns, Ame! Em breve você aprenderá mais sobre inteligência artificial e o seu impacto! Por agora pode aguardar os próximos desafios que virão.";
            }
            else
            {
                if (mainText != null) mainText.text = "Infelizmente é necessário ao menos 100 XP para que você possa ser promovida.";
            }
        }
        else if (quizState == QuizState.EndResult)
        {
            // --- ATUALIZA A BARRA VISUAL DO JOGO AQUI NO FINAL! ---
            int barrasGanhas = 0;

            // Lógica conforme você pediu:
            if (totalScore >= 100) barrasGanhas = 4;      // Passou o quiz (4 barras ou mais)
            else if (totalScore >= 70) barrasGanhas = 3;  // Conseguiu 70 XP (3 barras)
            else if (totalScore >= 50) barrasGanhas = 2;  // Conseguiu 50 XP (2 barras)
            else if (totalScore >= 20) barrasGanhas = 1;  // Conseguiu 20 XP (1 barra)

            // Se o jogador ganhou alguma barra visual, adiciona tudo de uma vez
            if (XPManager.instance != null && barrasGanhas > 0)
            {
                XPManager.instance.AddXP(barrasGanhas);
            }

            // Fecha o diálogo e encerra o jogo
            EndDialogue();
        }
    }

    public void OnAnswerSelected(int optionIndex)
    {
        if (answerButtonsPanel != null) answerButtonsPanel.SetActive(false);

        if (optionIndex == currentQuiz[currentQuestionIndex].correctAnswer)
        {
            int xpGanho = usedBlinkyOnCurrent ? 20 : 50;
            totalScore += xpGanho;

            // REMOVIDO: XPManager.instance.AddXP(1); <- A barra global não sobe mais aqui!

            quizState = QuizState.CorrectAna;
            if (quizPanelToHide != null) quizPanelToHide.SetActive(false);
            if (dialogueBox != null) dialogueBox.SetActive(true);

            SetQuizSpeaker("Ana");
            if (mainText != null) mainText.text = "Isso mesmo! Você ganhou " + xpGanho + " XP!";
            if (nextButton != null) nextButton.SetActive(true);
        }
        else
        {
            currentQuestionAttempts++;

            if (currentQuestionAttempts >= 2)
            {
                quizState = QuizState.FailedAna;
                if (quizPanelToHide != null) quizPanelToHide.SetActive(false);
                if (dialogueBox != null) dialogueBox.SetActive(true);

                SetQuizSpeaker("Ana");
                if (mainText != null) mainText.text = "Infelizmente você errou novamente. Vamos para a próxima pergunta.";
                if (nextButton != null) nextButton.SetActive(true);
            }
            else
            {
                quizState = QuizState.WrongAna;
                if (quizPanelToHide != null) quizPanelToHide.SetActive(false);
                if (dialogueBox != null) dialogueBox.SetActive(true);

                SetQuizSpeaker("Ana");
                if (mainText != null) mainText.text = "Poxa, infelizmente você errou, mas não se preocupe!";
                if (nextButton != null) nextButton.SetActive(true);
            }
        }
    }

    public void OnCallBlinky()
    {
        if (errorPanel != null) errorPanel.SetActive(false);
        quizState = QuizState.AmeAsking;

        if (quizPanelToHide != null) quizPanelToHide.SetActive(false);
        if (dialogueBox != null) dialogueBox.SetActive(true);

        SetQuizSpeaker("Ame");
        if (mainText != null) mainText.text = "Quero ajuda do Blinky Bunny, por favor!";
        if (nextButton != null) nextButton.SetActive(true);
    }

    public void OnSkipQuestion()
    {
        if (errorPanel != null) errorPanel.SetActive(false);
        currentQuestionIndex++;
        usedBlinkyOnCurrent = false;
        currentQuestionAttempts = 0;
        ShowQuestion();
    }

    private void SetQuizSpeaker(string speaker, Sprite newSprite = null)
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

            if (newSprite != null && ameImage != null) ameImage.sprite = newSprite;
        }
        else if (speaker == "Blinky")
        {
            if (nameBoxCenter != null) nameBoxCenter.SetActive(true);
            if (nameTextCenter != null) nameTextCenter.text = speaker;
            HighlightCharacter(blinkyRect, blinkyImage, true);

            if (newSprite != null && blinkyImage != null) blinkyImage.sprite = newSprite;
        }
        else
        {
            if (nameBoxRight != null) nameBoxRight.SetActive(true);
            if (nameTextRight != null) nameTextRight.text = speaker;
            HighlightCharacter(npcRect, npcImage, true);

            if (newSprite != null && npcImage != null) npcImage.sprite = newSprite;
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

    // --- FUNÇÃO PARA O BOTÃO DE FECHAR (X REDONDO) ---
    public void ForceCloseDialogue()
    {
        // 1. Reseta todas as variáveis de segurança para o próximo jogo não bugar
        isQuizMode = false;
        isStartingQuiz = false;
        quizState = QuizState.None;
        currentQuestionIndex = 0;
        totalScore = 0;
        currentQuestionAttempts = 0;

        // 2. Desliga os painéis para a tela começar limpa na próxima vez
        if (quizPanelToHide != null) quizPanelToHide.SetActive(false);
        if (dialogueBox != null) dialogueBox.SetActive(false);

        // 3. Desliga a tela inteira (Fecha o Canvas)
        gameObject.SetActive(false);

        // 4. Avisa o personagem/jogo que a interação acabou
        if (currentActiveNPC != null)
        {
            currentActiveNPC.OnDialogueFinished();
            currentActiveNPC = null;
        }

        hasSceneTransition = false;
        if (changeSceneButton != null) changeSceneButton.SetActive(false);
    }
}