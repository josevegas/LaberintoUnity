using UnityEngine;

/// <summary>
/// Controlador del jugador con movimiento grid-based estilo 8-bit.
/// Incluye animaciones simples y gestión de estado.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gridSize = 1f;
    [SerializeField] private bool snapToGrid = true;
    
    [Header("Animación")]
    [SerializeField] private Sprite[] idleSprites; // Animación idle
    [SerializeField] private Sprite[] walkSprites;   // Animación caminar
    [SerializeField] private float animationSpeed = 0.15f;
    
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 movement;
    private Vector2 targetPosition;
    private bool isMoving = false;
    private int currentFrame = 0;
    private float animationTimer = 0f;
    private bool isDead = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void Update()
    {
        if (isDead || !GameManager.Instance.IsGameActive) return;
        
        HandleInput();
        HandleAnimation();
    }

    private void FixedUpdate()
    {
        if (isDead) return;
        
        Move();
    }

    private void HandleInput()
    {
        // Obtener input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        // Priorizar un eje (movimiento estilo grid)
        if (Mathf.Abs(horizontal) > 0.1f)
        {
            movement = new Vector2(horizontal, 0);
        }
        else if (Mathf.Abs(vertical) > 0.1f)
        {
            movement = new Vector2(0, vertical);
        }
        else
        {
            movement = Vector2.zero;
        }
        
        // Flip sprite según dirección
        if (movement.x > 0)
            spriteRenderer.flipX = false;
        else if (movement.x < 0)
            spriteRenderer.flipX = true;
    }

    private void Move()
    {
        if (movement != Vector2.zero)
        {
            isMoving = true;
            Vector2 newPosition = rb.position + movement * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(newPosition);
        }
        else
        {
            isMoving = false;
            
            // Snap a grid cuando se detiene
            if (snapToGrid)
            {
                Vector2 snappedPos = new Vector2(
                    Mathf.Round(rb.position.x / gridSize) * gridSize,
                    Mathf.Round(rb.position.y / gridSize) * gridSize
                );
                rb.position = snappedPos;
            }
        }
    }

    private void HandleAnimation()
    {
        animationTimer += Time.deltaTime;
        
        if (animationTimer >= animationSpeed)
        {
            animationTimer = 0f;
            currentFrame++;
            
            Sprite[] currentAnimation = isMoving ? walkSprites : idleSprites;
            
            if (currentAnimation != null && currentAnimation.Length > 0)
            {
                currentFrame %= currentAnimation.Length;
                spriteRenderer.sprite = currentAnimation[currentFrame];
            }
        }
    }

    /// <summary>
    /// Llamado por trampas o peligros
    /// </summary>
    public void Die()
    {
        if (isDead) return;
        
        isDead = true;
        movement = Vector2.zero;
        
        // Efecto de muerte
        spriteRenderer.color = Color.red;
        
        // Notificar al GameManager
        GameManager.Instance?.PlayerDied();
    }

    /// <summary>
    /// Respawn en posición específica
    /// </summary>
    public void Respawn(Vector2 position)
    {
        transform.position = position;
        rb.position = position;
        isDead = false;
        spriteRenderer.color = Color.white;
        movement = Vector2.zero;
    }

    public void ResetState()
    {
        isDead = false;
        isMoving = false;
        movement = Vector2.zero;
        spriteRenderer.color = Color.white;
    }
}