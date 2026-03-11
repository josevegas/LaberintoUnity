using UnityEngine;

/// <summary>
/// Trampa estática que daña o mata al jugador al tocarla.
/// </summary>
public class Trap : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private bool instantKill = true;
    [SerializeField] private int damage = 1;
    [SerializeField] private ParticleSystem trapEffect;
    [SerializeField] private AudioClip trapSound;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TriggerTrap(other.gameObject);
        }
    }

    private void TriggerTrap(GameObject player)
    {
        // Efectos
        if (trapEffect != null)
            Instantiate(trapEffect, transform.position, Quaternion.identity);
        
        if (trapSound != null && AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(trapSound);

        // Aplicar daño o muerte
        if (instantKill)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            playerController?.Die();
        }
        else
        {
            // Sistema de vida con daño parcial (requiere PlayerController modificado)
            // player.GetComponent<PlayerHealth>()?.TakeDamage(damage);
        }
        
        Debug.Log("¡Trampa activada!");
    }
}