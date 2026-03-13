using UnityEngine;

public class Checkpoint : Interactable
{
    [Header("Respawn")]
    [Tooltip("Where the player spawns. Defaults to this object's position if left empty.")]
    public Transform respawnAnchor;
    [Header("Room")]
    public RoomTransition room;
    [Header("Visual")]
    public SpriteRenderer visual;
    public Color inactiveColor = new Color(0.4f, 0.4f, 0.4f, 1f);
    public Color activeColor = new Color(0.3f, 1f, 0.6f, 1f);


    public Vector2 RespawnPosition => respawnAnchor != null
        ? (Vector2)respawnAnchor.position
        : (Vector2)transform.position;

    void Start()
    {
        if (visual != null)
            visual.color = inactiveColor;
    }

    protected override void OnInteract()
    {
        CheckpointManager.Instance.SetCheckpoint(this);
    }

    public void SetActive(bool isActive)
    {
        if (visual != null)
            visual.color = isActive ? activeColor : inactiveColor;
    
    }
}