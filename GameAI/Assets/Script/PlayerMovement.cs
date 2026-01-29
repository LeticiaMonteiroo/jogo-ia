using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    
    // Referências que você precisa arrastar no Inspector
    public VirtualJoystick joystick; 
    public Animator animator; 

    private Rigidbody2D rb;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Proteção caso você esqueça de arrastar o Animator no Inspector
        if (animator == null) 
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 1. Recebe os valores do Joystick
        movement.x = joystick.Horizontal; 
        movement.y = joystick.Vertical;

        // 2. Envia a Velocidade para o Animator
        // Isso ativa as setinhas (transições) entre "Idle Tree" e "Walking Tree"
        animator.SetFloat("Speed", movement.sqrMagnitude);

        // 3. Envia a Direção APENAS se estiver se movendo
        // Se movement.sqrMagnitude for maior que 0.01, significa que o joystick está mexendo.
        if (movement.sqrMagnitude > 0.01f)
        {
            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
        }
    }

    void FixedUpdate()
    {
        // 4. Move a personagem fisicamente
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}