using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    
    // Referências que você precisa arrastar no Inspector
    public VirtualJoystick joystick; 
    public Animator animator; 

    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector2 lastDirection; // Guarda a última direção que a Ame olhou

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Proteção caso você esqueça de arrastar o Animator no Inspector
        if (animator == null) 
            animator = GetComponent<Animator>();

        // Garante que a Ame comece o jogo olhando para a frente/baixo (evita bugs no primeiro frame)
        lastDirection = new Vector2(0, -1);
        animator.SetFloat("Horizontal", lastDirection.x);
        animator.SetFloat("Vertical", lastDirection.y);
    }

    void Update()
    {
        // 1. Recebe os valores do Joystick
        movement.x = joystick.Horizontal; 
        movement.y = joystick.Vertical;

        // 2. Calcula a velocidade exata (vai de 0 a 1 com o joystick)
        float speed = movement.magnitude;

        // 3. Envia a Velocidade para o Animator (Ativa a troca entre Parado e Andando)
        animator.SetFloat("Speed", speed);

        // 4. Envia a Direção APENAS se estiver empurrando o joystick (speed maior que 0.01)
        if (speed > 0.01f)
        {
            // O .normalized garante que o valor seja "forte" o suficiente para a BlendTree não bugar no meio do caminho
            lastDirection = movement.normalized;
            
            animator.SetFloat("Horizontal", lastDirection.x);
            animator.SetFloat("Vertical", lastDirection.y);
        }
    }

    void FixedUpdate()
    {
        // 5. Move a personagem fisicamente
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}