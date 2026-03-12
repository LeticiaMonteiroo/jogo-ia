using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [Header("UI do Tutorial")]
    public GameObject painelTutorial; // O painel inteiro do tutorial
    public Image imagemMostrador;     // A imagem que vai ficar trocando no meio da tela

    [Header("Imagens do Tutorial")]
    public Sprite[] slidesTutorial;   // A lista onde você vai colocar suas 5 imagens

    private int slideAtual = 0;       // Guarda em qual número da lista estamos

    private void Start()
    {
        // Garante que o tutorial comece fechado
        if (painelTutorial != null) 
            painelTutorial.SetActive(false);
    }

    // Função para o botão "Tutorial" do seu menu abrir a tela
    public void AbrirTutorial()
    {
        if (slidesTutorial.Length == 0) return; // Segurança: se não tiver imagem, não faz nada

        slideAtual = 0; // Sempre começa da primeira imagem
        imagemMostrador.sprite = slidesTutorial[slideAtual];
        painelTutorial.SetActive(true);
    }

    // Função para o botão "Próximo" (Seta ou botão de avançar)
    public void ProximoSlide()
    {
        slideAtual++; // Avança um número

        // Verifica se ainda tem imagens na lista
        if (slideAtual < slidesTutorial.Length)
        {
            imagemMostrador.sprite = slidesTutorial[slideAtual];
        }
        else
        {
            // Se passou da última imagem, o tutorial acaba e a tela fecha!
            FecharTutorial();
        }
    }

    // Função para fechar a tela a qualquer momento (Botão X)
    public void FecharTutorial()
    {
        if (painelTutorial != null) 
            painelTutorial.SetActive(false);
    }
}