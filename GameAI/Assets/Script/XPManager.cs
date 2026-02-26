using UnityEngine;
using UnityEngine.UI;

public class XPManager : MonoBehaviour
{
    public static XPManager instance; 

    [Header("XP Bar UI")]
    public Image xpBarImage;
    public Sprite[] xpBarSprites; 

    [Header("Progress")]
    public int currentXP = 0;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // 1. LER O BLOQUINHO DE NOTAS QUANDO O JOGO ABRE
        // Ele procura por "PlayerTotalXP". Se não achar nada (primeira vez jogando), ele carrega 0.
        currentXP = PlayerPrefs.GetInt("PlayerTotalXP", 0); 
        
        // 2. Atualiza a imagem da barra logo de cara
        UpdateXPUI();
    }

    public void AddXP(int amount)
    {
        currentXP += amount;

        // 3. ANOTAR NO BLOQUINHO SEMPRE QUE GANHAR XP
        PlayerPrefs.SetInt("PlayerTotalXP", currentXP);
        PlayerPrefs.Save(); // Força a Unity a salvar no HD/Celular na mesma hora!

        UpdateXPUI();
    }

    // Separei a lógica de atualizar a imagem para podermos chamar no Start e no AddXP
    private void UpdateXPUI()
    {
        if (currentXP >= xpBarSprites.Length)
        {
            currentXP = xpBarSprites.Length - 1; // Trava no máximo
        }

        if (xpBarImage != null && xpBarSprites.Length > 0)
        {
            xpBarImage.sprite = xpBarSprites[currentXP];
        }
    }

    // --- TRUQUE DE MESTRE PARA TESTES ---
    // Você pode clicar com o botão direito no componente XPManager lá no Inspector e escolher "Reset Save Data" para zerar o jogo!
    [ContextMenu("Reset Save Data")]
    public void ResetSaveData()
    {
        PlayerPrefs.DeleteAll(); // Apaga todo o bloquinho de notas
        currentXP = 0;
        UpdateXPUI();
        Debug.Log("Save apagado com sucesso!");
    }
}