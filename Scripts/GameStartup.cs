using UnityEngine;

public class GameStartup : MonoBehaviour
{
    [Header("Optional: Override spawn point (leave null to use player's current position)")]
    public Transform spawnPoint;

    void Start()
    {
        CameraFollow cam = Camera.main.GetComponent<CameraFollow>();
        Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("GameStartup: No GameObject tagged 'Player' found!");
            return;
        }

        // If a spawn point is set, teleport the player there first
        if (spawnPoint != null)
            player.position = spawnPoint.position;

        // Find whichever RoomTransition contains the player's current position
        RoomTransition startRoom = FindRoomContainingPoint(player.position);

        if (startRoom != null)
        {
            startRoom.ApplyAsCurrentRoom(cam);
            Debug.Log($"GameStartup: Starting in room '{startRoom.gameObject.name}'");
        }
        else
        {
            Debug.LogWarning("GameStartup: Player is not inside any RoomTransition bounds. Camera bounds won't be set.");
        }
    }

    RoomTransition FindRoomContainingPoint(Vector3 point)
    {
        foreach (RoomTransition room in FindObjectsByType<RoomTransition>(FindObjectsSortMode.None))
        {
            if (room.GetBounds().Contains(point))
                return room;
        }
        return null;
    }
}