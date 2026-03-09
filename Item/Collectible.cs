using UnityEngine;

public abstract class Collectible : MonoBehaviour
{
    [Header("Visual 8-Bit")]
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    
    [Header("Efectos")]
    public AudioClip collectSound;
    public GameObject particleEffect;
    
    public abstract void Collect();
    
    protected void DestroyWithEffect()
    {
        if (collectSound) AudioSource.PlayClipAtPoint(collectSound, transform.position);
        if (particleEffect) Instantiate(particleEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}

// Moneda.cs
public class Coin : Collectible
{
    public int value = 10;
    
    public override void Collect()
    {
        GameManager.Instance.AddCoins(value);
        UIManager.Instance.ShowFloatingText($"+{value}", transform.position, Color.yellow);
        DestroyWithEffect();
    }
}

// Llave.cs
public class Key : Collectible
{
    public override void Collect()
    {
        GameManager.Instance.AddKey();
        UIManager.Instance.ShowFloatingText("KEY!", transform.position, Color.cyan);
        DestroyWithEffect();
    }
}

// PowerUp.cs
public class PowerUp : Collectible
{
    public enum Type { Speed, Vision, Shield }
    public Type powerUpType;
    
    public override void Collect()
    {
        GameManager.Instance.ActivatePowerUp(powerUpType);
        DestroyWithEffect();
    }
}