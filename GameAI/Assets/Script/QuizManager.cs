using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

// Classe para estruturar as falas da introdução
[System.Serializable]
public class DialogueLine
{
    public string speaker;
    [TextArea(2, 5)]
    public string text;
}

public class QuizManager : MonoBehaviour
{
    [Header("Current Data (Do not fill manually)")]
    // Essas listas agora são preenchidas pelo NPC automaticamente
    public List<DialogueLine> introLines;
    public List<QuestionData> questions;
    public int xpToPass = 100;
    
    [Header("Visual Characters")]
    public RectTransform ameRect;    
    public RectTransform blinkyRect; 
    public Image ameImage;           
    public Image blinkyImage;
    
    [Header("Name Boxes")]
    public GameObject nameBoxLeft;   
    public TextMeshProUGUI nameTextLeft;
    public GameObject nameBoxCenter; 
    public TextMeshProUGUI nameTextCenter;

    [Header("UI - Dialogue & Buttons")]
    public GameObject dialogueBox;     
    public TextMeshProUGUI mainText;   
    public GameObject nextButton;      
    public GameObject answerButtonsGroup; 
    public Button[] answerButtons;     

    [Header("UI - Extra Panels")]
    public GameObject errorPanel;
    public GameObject resultPanel;
    public TextMeshProUGUI resultText;

    // Internal State
    private int currentQuestionIdx = 0;
    private int totalXP = 0;
    private bool hasUsedHint = false;
    private bool isBlinkyActive = false; 
    
    private int currentIntroIdx = 0;
    private bool isIntroPlaying = false;

    private Vector2 blinkyOriginalPos;

    // --- NEW: CALLED BY THE NPC ---
    public void StartConversation(List<DialogueLine> newIntroLines, List<QuestionData> newQuestions, int newXp)
    {
        // 1. Recebe os dados do NPC
        introLines = newIntroLines;
        questions = newQuestions;
        xpToPass = newXp;

        // 2. Liga a tela do Canvas
        gameObject.SetActive(true); 

        // 3. Salva a posição original do Blinky
        if (blinkyRect != null && blinkyOriginalPos == Vector2.zero)
            blinkyOriginalPos = blinkyRect.anchoredPosition;
            
        // 4. Decide se começa pela conversa ou vai direto pro Quiz
        if (introLines != null && introLines.Count > 0)
        {
            StartIntro();
        }
        else
        {
            StartQuiz(); 
        }
    }

    // --- INTRO SYSTEM ---
    private void StartIntro()
    {
        isIntroPlaying = true;
        currentIntroIdx = 0;
        
        dialogueBox.SetActive(true);
        answerButtonsGroup.SetActive(false);
        errorPanel.SetActive(false);
        resultPanel.SetActive(false);
        
        PlayNextIntroLine();
    }

    private void PlayNextIntroLine()
    {
        // Se as falas acabaram, vai pro Quiz
        if (currentIntroIdx >= introLines.Count)
        {
            isIntroPlaying = false;
            StartQuiz();
            return;
        }

        ResetCharacterPositions(); 
        nextButton.SetActive(true);
        
        DialogueLine line = introLines[currentIntroIdx];
        
        SetupSpeaker(line.speaker);
        mainText.text = line.text;

        nextButton.GetComponent<Button>().onClick.RemoveAllListeners();
        nextButton.GetComponent<Button>().onClick.AddListener(() => {
            currentIntroIdx++;
            PlayNextIntroLine();
        });
    }

    // --- QUIZ SYSTEM ---
    public void StartQuiz()
    {
        // Se NÃO houver perguntas (só conversa), fecha o diálogo e encerra
        if (questions == null || questions.Count == 0)
        {
            CloseDialogue();
            return;
        }

        currentQuestionIdx = 0;
        totalXP = 0;
        LoadQuestion();
    }

    private void LoadQuestion()
    {
        if (currentQuestionIdx >= questions.Count) 
        { 
            ShowResults(); 
            return; 
        }

        hasUsedHint = false;
        isBlinkyActive = false;

        ResetCharacterPositions();
        
        dialogueBox.SetActive(true);
        answerButtonsGroup.SetActive(false); 
        nextButton.SetActive(true);          
        errorPanel.SetActive(false);

        SetupSpeaker("Ame"); 
        mainText.text = questions[currentQuestionIdx].questionText;
        
        nextButton.GetComponent<Button>().onClick.RemoveAllListeners();
        nextButton.GetComponent<Button>().onClick.AddListener(ShowAnswers);
    }

    public void ShowAnswers()
    {
        if (isBlinkyActive)
        {
            LoadQuestion(); 
            return;
        }

        nextButton.SetActive(false); 
        answerButtonsGroup.SetActive(true); 

        QuestionData q = questions[currentQuestionIdx];
        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (i < q.options.Length)
            {
                answerButtons[i].gameObject.SetActive(true);
                answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = q.options[i];
                int idx = i;
                answerButtons[i].onClick.RemoveAllListeners();
                answerButtons[i].onClick.AddListener(() => CheckAnswer(idx));
            }
            else 
            {
                answerButtons[i].gameObject.SetActive(false);
            }
        }
    }

    // --- HINT SYSTEM ---
    public void AcceptHelp() 
    {
        hasUsedHint = true;
        isBlinkyActive = true;
        errorPanel.SetActive(false);
        dialogueBox.SetActive(true);
        answerButtonsGroup.SetActive(false); 
        nextButton.SetActive(true); 

        MoveBlinkyToCenter();
        SetupSpeaker("Blinky");
        mainText.text = questions[currentQuestionIdx].blinkyHint;
    }

    private void MoveBlinkyToCenter()
    {
        if(ameImage != null) ameImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        if(blinkyImage != null) blinkyImage.color = Color.white;
        
        if (blinkyRect != null)
        {
            blinkyRect.anchoredPosition = new Vector2(0, blinkyOriginalPos.y); 
            blinkyRect.localScale = Vector3.one * 1.2f; 
            blinkyRect.SetAsLastSibling();
        }
    }

    private void ResetCharacterPositions()
    {
        if(ameImage != null) ameImage.color = Color.white;
        if(blinkyImage != null) blinkyImage.color = new Color(0.5f, 0.5f, 0.5f, 1f); 
        
        if (blinkyRect != null)
        {
            blinkyRect.anchoredPosition = blinkyOriginalPos;
            blinkyRect.localScale = Vector3.one * 0.9f;
        }
    }

    private void SetupSpeaker(string speaker)
    {
        if (speaker == "Blinky")
        {
            nameBoxLeft.SetActive(false);
            nameBoxCenter.SetActive(true); 
            nameTextCenter.text = "Blinky Bunny";
        }
        else
        {
            nameBoxLeft.SetActive(true); 
            nameBoxCenter.SetActive(false);
            nameTextLeft.text = speaker; 
        }
    }

    private void CheckAnswer(int idx)
    {
        if (idx == questions[currentQuestionIdx].correctIndex)
        {
            totalXP += hasUsedHint ? 20 : 50;
            currentQuestionIdx++;
            LoadQuestion();
        }
        else
        {
            if (!hasUsedHint) ShowErrorPanel();
            else { currentQuestionIdx++; LoadQuestion(); } 
        }
    }

    private void ShowErrorPanel()
    {
        dialogueBox.SetActive(false); 
        answerButtonsGroup.SetActive(false); 
        errorPanel.SetActive(true);
    }
    
    public void DeclineHelp() 
    { 
        currentQuestionIdx++; 
        LoadQuestion(); 
    }

    private void ShowResults()
    {
        dialogueBox.SetActive(false);
        resultPanel.SetActive(true);
        resultText.text = "XP Final: " + totalXP;
    }

    // --- CLOSE DIALOGUE ---
    public void CloseDialogue()
    {
        gameObject.SetActive(false);
    }
}