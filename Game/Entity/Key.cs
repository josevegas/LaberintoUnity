using UnityEngine;

/// <summary>
/// Llave necesaria para abrir puertas.
/// Solo puede haber una en el inventario a la vez.
/// </summary>
public class Key : Item
{
    [Header("Configuración de Llave")]
    [SerializeField] private Color glowColor = Color.yellow;
    
    private Light keyLight; // Opcional: efecto de brillo

    protected override void Awake()
    {
        base.Awake();
        
        // Efecto visual pulsante
        if (spriteRenderer != null)
        {
            spriteRenderer.color = glowColor;
        }
    }

    protected override bool CanCollect(GameObject player)
    {
        // Verificar si ya tiene llave (opcional: permitir múltiples)
        return true;
    }

    protected override void ApplyEffect(GameObject player)
    {
        GameManager.Instance?.CollectKey();
        Debug.Log("¡Llave recolectada!");
    }
}