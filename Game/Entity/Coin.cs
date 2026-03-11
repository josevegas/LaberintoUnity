using UnityEngine;

/// <summary>
/// Moneda recolectable que aumenta el puntaje.
/// </summary>
public class Coin : Item
{
    [Header("Configuración de Moneda")]
    [SerializeField] private int pointValue = 10;
    [SerializeField] private float rotationSpeed = 180f;

    private void Update()
    {
        // Animación simple de rotación para efecto visual 8-bit
        if (spriteRenderer != null)
        {
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }
    }

    protected override void ApplyEffect(GameObject player)
    {
        GameManager.Instance?.CollectCoin(pointValue);
        
        // Feedback visual flotante (opcional)
        Debug.Log($"Moneda recolectada: +{pointValue} puntos");
    }
}