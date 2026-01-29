using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizManager : MonoBehaviour
{
    [Header("Pontuação")]
    public int xpMaximo = 50;  // Acerto de primeira
    public int xpRecuperacao = 20; // Acerto depois do Blinky

    [Header("Referências")]
    public QuizQuestion[] questions;
    public DialogueManager dialogueManager;
    
    [Header("UI - Pergunta")]
    public GameObject quizButtonsPanel;
    public Button[] answerButtons;

    [Header("UI - Tela de Erro (Fundo Rosa)")]
    public GameObject errorPanel; // O painel rosa que cobre tudo
    public TextMeshProUGUI errorText; // "Você errou..."
    public Button callBlinkyButton; // "Pedir ajuda (20 XP)"
    public Button giveUpButton; // "Próxima Pergunta (0 XP)"
    
    [Header("UI - Botão de Retorno")]
    public Button returnToQuizButton; // Botão "Tentar Novamente" após a explicação

    // Estado Interno
    private int currentIndex = 0;
    private int currentXpValue = 0; // Quanto vale a pergunta AGORA
    private bool isRetrying = false; // Se estamos na segunda chance

    void Start()
    {
        LoadQuestion(0);
    }

    public void LoadQuestion(int index)
    {
        if (index >= questions.Length) return; // Fim do Quiz

        currentIndex = index;
        
        // Configuração Inicial da Pergunta
        currentXpValue = xpMaximo; // Começa valendo 50
        isRetrying = false;

        // Limpa UI
        errorPanel.SetActive(false);
        returnToQuizButton.gameObject.SetActive(false);
        
        // Mostra Pergunta
        ShowQuestionUI();
    }

    void ShowQuestionUI()
    {
        quizButtonsPanel.SetActive(true);
        
        // Manda o texto para a Ana falar
        ShowDialogue("Ana", questions[currentIndex].questionText, true);

        // Configura botões de resposta
        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (i < questions[currentIndex].answers.Length)
            {
                answerButtons[i].gameObject.SetActive(true);
                answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = questions[currentIndex].answers[i];
                
                int idx = i;
                answerButtons[i].onClick.RemoveAllListeners();
                answerButtons[i].onClick.AddListener(() => OnAnswerSelected(idx));
            }
            else
            {
                answerButtons[i].gameObject.SetActive(false);
            }
        }
    }

    // --- LÓGICA DE RESPOSTA ---

    void OnAnswerSelected(int index)
    {
        if (index == questions[currentIndex].correctAnswerIndex)
        {
            // ACERTOU!
            Debug.Log("Ganhou " + currentXpValue + " XP!");
            
            // Se quiser, Ana pode parabenizar antes de ir pro próximo
            // LoadQuestion(currentIndex + 1); // Avança direto
            ShowFeedbackAndAdvance(true);
        }
        else
        {
            // ERROU!
            if (!isRetrying)
            {
                // Se é a primeira vez que erra, vai para a tela do Blinky
                ShowErrorScreen();
            }
            else
            {
                // Se já estava tentando de novo e errou (difícil acontecer se o Blinky der a resposta), perde tudo.
                Debug.Log("Errou de novo. 0 XP.");
                LoadQuestion(currentIndex + 1);
            }
        }
    }

    void ShowFeedbackAndAdvance(bool acertou)
    {
         // Exemplo simples: Avança logo
         LoadQuestion(currentIndex + 1);
    }

    // --- TELA DE ERRO / BLINKY ---

    void ShowErrorScreen()
    {
        quizButtonsPanel.SetActive(false); // Esconde botões de resposta
        errorPanel.SetActive(true); // Mostra tela rosa
        
        // Configura texto da Ana no Painel de Erro
        errorText.text = "Infelizmente você errou!\nMas não se preocupe, posso te ensinar novamente valendo " + xpRecuperacao + " XP.";
        
        // Configura botões do painel rosa
        callBlinkyButton.onClick.RemoveAllListeners();
        callBlinkyButton.onClick.AddListener(AcceptBlinkyHelp);

        giveUpButton.onClick.RemoveAllListeners();
        giveUpButton.onClick.AddListener(GiveUpQuestion);
    }

    public void AcceptBlinkyHelp()
    {
        // Jogador aceitou ajuda
        currentXpValue = xpRecuperacao; // Agora vale 20
        isRetrying = true;
        
        errorPanel.SetActive(false); // Some painel de erro
        
        // Blinky aparece e explica
        // Assumindo que isCenterCharacter existe no seu sistema de diálogo, senão use isLeft=false
        ShowDialogue("Blinky", questions[currentIndex].blinkyExplanation, false);
        
        // Ativa o botão para voltar a responder depois de ler
        returnToQuizButton.gameObject.SetActive(true);
    }

    public void GiveUpQuestion()
    {
        // Desistiu, ganha 0 e vai pro próximo
        LoadQuestion(currentIndex + 1);
    }

    // Chamado pelo botão "Tentar Novamente" (ReturnButton)
    public void ReturnToAnswer()
    {
        returnToQuizButton.gameObject.SetActive(false);
        ShowQuestionUI(); // Mostra os botões de resposta de novo
    }

    // Auxiliar
    void ShowDialogue(string name, string text, bool isLeft)
    {
        DialogueLine line = new DialogueLine();
        line.characterName = name;
        line.sentence = text;
        line.isLeftCharacter = isLeft;
        
        Dialogue d = new Dialogue();
        d.lines = new DialogueLine[] { line };
        dialogueManager.StartDialogue(d);
    }
}