using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 1f;//0.125f;
    public Vector3 offset = new Vector3(0, 0, -10);

 

    [Header("Room Bounds")]
    public float boundsPadding = 1f;
    public float edgeMargin = 0.2f;
    private Bounds currentRoomBounds;
    private bool useRoomBounds = false;


    private Vector3 GetClamped(Vector3 desired)
    {
        if (!useRoomBounds) return desired;

        Camera cam = Camera.main;
        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;

        float minX = currentRoomBounds.min.x + halfW + edgeMargin;
        float maxX = currentRoomBounds.max.x - halfW - edgeMargin;
        float minY = currentRoomBounds.min.y + halfH + edgeMargin;
        float maxY = currentRoomBounds.max.y - halfH - edgeMargin;

        // If room smaller than camera view: lock to center on that axis
        if (minX > maxX) desired.x = currentRoomBounds.center.x;
        else desired.x = Mathf.Clamp(desired.x, minX, maxX);

        if (minY > maxY) desired.y = currentRoomBounds.center.y;
        else desired.y = Mathf.Clamp(desired.y, minY, maxY);

        return desired;
    }

    void LateUpdate()
    {
        if (target == null) return;
  
            // Normal following with room bounds
            Vector3 desiredPosition = target.position + offset;
            desiredPosition = GetClamped(desiredPosition);
            transform.position = desiredPosition;

        
    }
    public void SetRoomBounds(Bounds newBounds)
    {
        currentRoomBounds = newBounds;
        useRoomBounds = true;
    }
    public void SnapNow()
    {
        if (target == null) return;

        Vector3 desired = target.position + offset;
        desired = GetClamped(desired);

        transform.position = new Vector3(desired.x, desired.y, transform.position.z);
    }

}