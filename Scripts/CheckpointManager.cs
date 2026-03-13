using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;
    private RoomTransition activeRoom;
    [Header("Default Spawn")]
    public RoomTransition defaultRoom;
    public Vector2 defaultRespawnPoint;
    public Vector2 RespawnPoint { get; private set; }
    public Checkpoint ActiveCheckpoint { get; private set; }
    public ParticleSystem checkpointParticles;

    void Awake()
    {
        // Singleton — persists across scenes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        // Use default values if no checkpoint has been set yet
        if (ActiveCheckpoint == null)
        {
            RespawnPoint = defaultRespawnPoint;
            activeRoom = defaultRoom;
        }
    }
    public void SetCheckpoint(Checkpoint checkpoint)
    {
        // Deactivate previous checkpoint visual
        if (ActiveCheckpoint != null && ActiveCheckpoint != checkpoint)
            ActiveCheckpoint.SetActive(false);

        ActiveCheckpoint = checkpoint;
        RespawnPoint = checkpoint.RespawnPosition;
        activeRoom = checkpoint.room;
        checkpoint.SetActive(true);

        // Move and play shared particles
        if (checkpointParticles != null)
        {
            checkpointParticles.transform.position = checkpoint.transform.position;
            checkpointParticles.Play();
        }
        // Heal player on checkpoint touch
        PlayerHealth player = FindObjectOfType<PlayerHealth>();
        if (player != null)
            player.HealToFull();

    }

    public void RespawnPlayer(PlayerHealth player)
    {
        player.transform.position = RespawnPoint;
        player.Respawn();
        if (activeRoom != null)              
            activeRoom.ApplyAsCurrentRoom(Camera.main.GetComponent<CameraFollow>());
    }
}