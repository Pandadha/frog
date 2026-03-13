using UnityEngine;
using UnityEngine.UI;

public class LifeDotsUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Image[] dots;

    [Header("Blink")]
    [SerializeField] private float blinkDuration = 0.35f;
    [SerializeField] private float blinkHz = 18f;

    [SerializeField] private Color activeColor = Color.white;
    [SerializeField] private Color emptyColor = new Color(0.3f, 0.3f, 0.3f, 1f); // gray
    // internal blink state (no coroutines, no allocations)
    private int blinkingIndex = -1;
    private float blinkTimer = 0f;

    private void OnEnable()
    {
        if (playerHealth != null)
            playerHealth.OnLivesChanged += HandleLivesChanged;

        // initialize visuals
        if (playerHealth != null)
            RefreshAll(playerHealth.Lives);
    }

    private void OnDisable()
    {
        if (playerHealth != null)
            playerHealth.OnLivesChanged -= HandleLivesChanged;
    }

    private void Update()
    {
        if (blinkingIndex < 0) return;

        blinkTimer -= Time.unscaledDeltaTime;

        // toggle alpha for blink effect
        float t = Time.unscaledTime * blinkHz;
        bool on = (Mathf.FloorToInt(t) % 2) == 0;

        SetDotAlpha(blinkingIndex, on ? 1f : 0.15f);

        if (blinkTimer <= 0f)
        {
            // end blink, hide dot
            dots[blinkingIndex].color = emptyColor;
            SetDotAlpha(blinkingIndex, 1f);

            blinkingIndex = -1;
        }
    }

    private void HandleLivesChanged(int newLives, int delta)
    {
        if (delta < 0)
        {
            // Blink the highest dot that was removed
            // e.g. lives 4→2: removedIndex should be 3 (the 4th dot), not 2
            int removedIndex = newLives + Mathf.Abs(delta) - 1;
            StartBlink(removedIndex);
            // Hide any dots that were silently removed below the blinked one
            for (int i = newLives; i < removedIndex && i < dots.Length; i++)
                dots[i].color = emptyColor;
        }
        else
        {
            RefreshAll(newLives);
        }
    }

    private void RefreshAll(int lives)
    {
        if (blinkingIndex >= 0 && blinkingIndex < dots.Length)
        {
            dots[blinkingIndex].color = activeColor;
            SetDotAlpha(blinkingIndex, 1f);
            blinkingIndex = -1;  // <-- stops Update() from finishing the blink
            blinkTimer = 0f;
        }
        lives = Mathf.Clamp(lives, 0, dots.Length);
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].enabled = true;                                    // always visible
            dots[i].color = (i < lives) ? activeColor : emptyColor;   // white or gray
            SetDotAlpha(i, 1f);
        }
        if (blinkingIndex >= lives)
            blinkingIndex = -1;
    }

    private void StartBlink(int index)
    {
        if (index < 0 || index >= dots.Length) return;

        // make sure it’s visible while blinking
        dots[index].enabled = true;
        blinkTimer = blinkDuration;
        blinkingIndex = index;
    }

    private void SetDotAlpha(int index, float a)
    {
        var c = dots[index].color;
        c.a = a;
        dots[index].color = c;
    }
}
