using UnityEngine;

public class RoomTransition : MonoBehaviour
{
    [Header("This Room's Bounds")]
    public Vector2 roomSize = new Vector2(20, 12);
    public Vector2 roomCenter;
    public Color roomBackgroundColor = Color.clear;

    // NEW: DoorTransition will call this to get the bounds for this room
    public Bounds GetBounds()
    {
        return new Bounds(
            new Vector3(roomCenter.x, roomCenter.y, 0f),
            new Vector3(roomSize.x, roomSize.y, 0f)
        );
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(roomCenter, new Vector3(roomSize.x, roomSize.y, 0));
    }
    public void ApplyAsCurrentRoom(CameraFollow cam)
    {
        Camera.main.backgroundColor = roomBackgroundColor;
        cam.SetRoomBounds(GetBounds());
        cam.SnapNow();
    }

}
