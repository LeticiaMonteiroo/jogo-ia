using UnityEngine;

public class TriggerInteract : MonoBehaviour
{
    [Header("Aba que este NPC vai abrir")]
    public GameObject telaParaAbrir; // Arraste o QuizSystem aqui

    // A Unity chama essa função automaticamente quando a Ame ENTRA na área do NPC
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica se quem bateu no NPC tem a Tag "Player" (A Ame)
        if (collision.CompareTag("Player"))
        {
            // Liga a tela do Quiz
            if (telaParaAbrir != null && !telaParaAbrir.activeSelf)
            {
                telaParaAbrir.SetActive(true);
            }
        }
    }
}