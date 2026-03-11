using UnityEngine;

/// <summary>
/// Clase base abstracta para todos los ítems interactuables.
/// Implementa el patrón Template Method para comportamiento común.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public abstract class Item : MonoBehaviour
{
    [Header("Configuración Visual")]
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected ParticleSystem pickupEffect;
    [SerializeField] protected AudioClip pickupSound;
    
    protected Collider2D itemCollider;
    protected bool isCollected = false;

    protected virtual void Awake()
    {
        itemCollider = GetComponent<Collider2D>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Asegurar que el collider es trigger para ítems
        itemCollider.isTrigger = true;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected) return;
        
        if (other.CompareTag("Player"))
        {
            OnCollect(other.gameObject);
        }
    }

    /// <summary>
    /// Método principal llamado al recolectar. Implementa el flujo común.
    /// </summary>
    protected virtual void OnCollect(GameObject player)
    {
        if (!CanCollect(player)) return;
        
        isCollected = true;
        
        // Efectos visuales y sonoros
        PlayEffects();
        
        // Lógica específica del ítem
        ApplyEffect(player);
        
        // Destruir o desactivar
        DestroyItem();
    }

    /// <summary>
    /// Condición personalizable para permitir o bloquear recolección
    /// </summary>
    protected virtual bool CanCollect(GameObject player) => true;

    /// <summary>
    /// Efecto específico del ítem - implementado por clases hijas
    /// </summary>
    protected abstract void ApplyEffect(GameObject player);

    protected virtual void PlayEffects()
    {
        if (pickupEffect != null)
            Instantiate(pickupEffect, transform.position, Quaternion.identity);
        
        if (pickupSound != null && AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(pickupSound);
    }

    protected virtual void DestroyItem()
    {
        // Animación de desaparición opcional antes de destruir
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }
        
        itemCollider.enabled = false;
        Destroy(gameObject, 0.1f);
    }
}