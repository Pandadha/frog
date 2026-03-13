using UnityEngine;
using System;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Lives")]
    [SerializeField] private int startLives = 3;
    [SerializeField] private int maxLives = 9;

    [Header("Damage Immunity")]
    [SerializeField] private float buttImmunitySeconds = 1.0f;

    [Header("Knockback")]
    [SerializeField] private bool enableKnockback = true;
    [SerializeField] private float knockbackForce = 8f;
    [SerializeField] private float knockbackUpwardBias = 0.3f; // 0 = purely away, 1 = purely up

    [Header("Immunity Visual")]
    [SerializeField] private bool enableBlinking = true;
    [SerializeField] private float blinkInterval = 0.1f;  // How fast to blink
    [SerializeField] private SpriteRenderer[] spritesToBlink; // All sprites to blink together


    public int Lives { get; private set; }
    public event Action<int, int> OnLivesChanged;

    float immuneUntil;
    public bool IsImmune => Time.time < immuneUntil;

    private Rigidbody2D rb;
    private Coroutine blinkCoroutine;
    private Vector2 lastDamageSource; // Track where damage came from

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Auto-find all SpriteRenderers if not assigned
        if (spritesToBlink == null || spritesToBlink.Length == 0)
            spritesToBlink = GetComponentsInChildren<SpriteRenderer>();

        Lives = Mathf.Clamp(startLives, 0, maxLives);
        OnLivesChanged?.Invoke(Lives, 0);
    }

    public void TakeDamage(int amount = 1)
    {
        if (amount <= 0 || Lives <= 0) return;
        if (IsImmune) return;

 

        int prev = Lives;
        Lives = Mathf.Max(0, Lives - amount);
        OnLivesChanged?.Invoke(Lives, Lives - prev);

        immuneUntil = Time.time + buttImmunitySeconds;

        // Apply knockback
        if (enableKnockback && rb != null)
        {
            ApplyKnockback();
        }

        // Start blinking effect
        if (enableBlinking && Lives > 0)
        {
            if (blinkCoroutine != null)
                StopCoroutine(blinkCoroutine);
            blinkCoroutine = StartCoroutine(BlinkWhileImmune());
        }

        // Respawn when dead
        if (Lives <= 0)
        {
            // Stop blinking if dead
            if (blinkCoroutine != null)
            {
                StopCoroutine(blinkCoroutine);
                SetSpritesVisible(true);
            }

            CheckpointManager.Instance.RespawnPlayer(this);
        }
    }

    // Overload to accept damage source position
    public void TakeDamage(int amount, Vector2 damageSourcePos)
    {
        lastDamageSource = damageSourcePos;
        TakeDamage(amount);
    }
    public void Respawn()
    {
        Lives = startLives;
        immuneUntil = Time.time + 1f;  // brief immunity on respawn so you don't instantly die again
        OnLivesChanged?.Invoke(Lives, 0);

        if (blinkCoroutine != null)
            StopCoroutine(blinkCoroutine);

        blinkCoroutine = StartCoroutine(BlinkWhileImmune());
    }
    public void HealToFull()
    {
        Lives = startLives; // or however you store max health
        OnLivesChanged?.Invoke(Lives, 0);
    }
    void ApplyKnockback()
    {
        Vector2 knockbackDir;

        // If we know where damage came from, push away from it
        if (lastDamageSource != Vector2.zero)
        {
            knockbackDir = ((Vector2)transform.position - lastDamageSource).normalized;
        }
        else
        {
            // Default: push backward based on player's facing direction
            knockbackDir = new Vector2(-Mathf.Sign(transform.localScale.x), 0f);
        }

        // Add upward bias to make knockback feel better
        knockbackDir = new Vector2(
            knockbackDir.x * (1f - knockbackUpwardBias),
            knockbackDir.y + knockbackUpwardBias
        ).normalized;

        // Apply the knockback force
        rb.velocity = new Vector2(rb.velocity.x * 0.5f, 0f); // Reset some momentum
        rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);

        // Reset damage source for next hit
        lastDamageSource = Vector2.zero;
    }

    IEnumerator BlinkWhileImmune()
    {
        if (spritesToBlink == null || spritesToBlink.Length == 0) yield break;

        while (IsImmune)
        {
            // Toggle all sprites at once
            bool visible = !spritesToBlink[0].enabled;
            SetSpritesVisible(visible);

            yield return new WaitForSeconds(blinkInterval);
        }

        // Ensure all sprites are visible when immunity ends
        SetSpritesVisible(true);
    }

    void SetSpritesVisible(bool visible)
    {
        if (spritesToBlink == null) return;

        foreach (var sprite in spritesToBlink)
        {
            if (sprite != null)
                sprite.enabled = visible;
        }
    }

 

    public void AddLife(int amount = 1)
    {
        if (amount <= 0 || Lives >= maxLives) return;
        int prev = Lives;
        Lives = Mathf.Min(maxLives, Lives + amount);
        OnLivesChanged?.Invoke(Lives, Lives - prev);
    }
}