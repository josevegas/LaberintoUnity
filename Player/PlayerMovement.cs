using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 5f;
    public LayerMask wallLayer;
    
    private Vector2 targetPosition;
    private bool isMoving = false;
    private Vector2 input;
    
    void Update()
    {
        if (isMoving) return;
        
        // Input con GetAxisRaw para respuesta instantánea (estilo retro)
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        
        if (input != Vector2.zero)
        {
            // Prevenir movimiento diagonal
            if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
                input.y = 0;
            else
                input.x = 0;
                
            Vector2 nextPos = (Vector2)transform.position + input;
            
            if (!Physics2D.OverlapCircle(nextPos, 0.1f, wallLayer))
            {
                targetPosition = nextPos;
                isMoving = true;
            }
        }
    }
    
    void FixedUpdate()
    {
        if (isMoving)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
            
            if (Vector2.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition;
                isMoving = false;
            }
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Collectible"))
        {
            other.GetComponent<Collectible>()?.Collect();
        }
        else if (other.CompareTag("Exit"))
        {
            GameManager.Instance.CompleteLevel();
        }
    }
}