using UnityEngine;

/// <summary>
/// Puerta bloqueada que requiere llave para abrirse.
/// Al abrirse, completa el nivel.
/// </summary>
public class Door : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private Sprite closedSprite;
    [SerializeField] private Sprite openSprite;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private ParticleSystem openEffect;
    
    private SpriteRenderer spriteRenderer;
    private Collider2D doorCollider;
    private bool isOpen = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        doorCollider = GetComponent<Collider2D>();
        
        if (closedSprite != null && spriteRenderer != null)
            spriteRenderer.sprite = closedSprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isOpen) return;
        
        if (other.CompareTag("Player"))
        {
            AttemptOpen();
        }
    }

    private void AttemptOpen()
    {
        if (GameManager.Instance == null) return;
        
        if (GameManager.Instance.UseKey())
        {
            OpenDoor();
        }
        else
        {
            // Feedback de puerta cerrada
            Debug.Log("Necesitas una llave para abrir esta puerta");
            // Aquí podrías mostrar un UI hint
        }
    }

    private void OpenDoor()
    {
        isOpen = true;
        
        // Cambiar sprite
        if (openSprite != null && spriteRenderer != null)
            spriteRenderer.sprite = openSprite;
        
        // Desactivar collider para permitir paso
        if (doorCollider != null)
            doorCollider.isTrigger = true; // O desactivarlo completamente
        
        // Efectos
        if (openEffect != null)
            Instantiate(openEffect, transform.position, Quaternion.identity);
        
        if (openSound != null && AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(openSound);
        
        // Notificar al GameManager
        GameManager.Instance.RegisterDoorOpened();
        
        Debug.Log("¡Puerta abierta! Nivel completado.");
    }
}