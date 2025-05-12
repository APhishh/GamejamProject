using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCharacter : MonoBehaviour
{
    [SerializeField] Transform playerTR;
    [SerializeField] float smoothTime = 0.3f; // Smoothing time
    [SerializeField] Vector2 offset = new Vector2(0.33f, 0.33f);
    [SerializeField] Vector2 deadzone = new Vector2(2f, 2f); // Size of deadzone
    [SerializeField] float maxSpeed = 30f; // Maximum camera speed

    private Camera mainCamera;
    private Vector2 velocity = Vector2.zero;
    private Vector3 lastTargetPosition;

    private void Start()
    {
        mainCamera = Camera.main;
        if (playerTR != null)
        {
            lastTargetPosition = playerTR.position;
        }
    }

    void Update()
    {
        if (playerTR == null) return;

        Vector3 targetPosition = CalculateTargetPosition();
        Vector3 currentPosition = transform.position;

        // Check if player is outside deadzone
        float deltaX = Mathf.Abs(targetPosition.x - currentPosition.x);
        float deltaY = Mathf.Abs(targetPosition.y - currentPosition.y);

        if (deltaX > deadzone.x || deltaY > deadzone.y)
        {
            // Apply exponential smoothing using SmoothDamp
            Vector3 smoothedPosition = new Vector3(
                Mathf.SmoothDamp(currentPosition.x, targetPosition.x, ref velocity.x, smoothTime, maxSpeed),
                Mathf.SmoothDamp(currentPosition.y, targetPosition.y, ref velocity.y, smoothTime, maxSpeed),
                currentPosition.z
            );

            transform.position = smoothedPosition;
        }

        lastTargetPosition = playerTR.position;
    }

    private Vector3 CalculateTargetPosition()
    {
        // Calculate look-ahead offset based on player's direction
        float playerVelocityX = (playerTR.position.x - lastTargetPosition.x) / Time.deltaTime;
        float lookAheadX = Mathf.Sign(playerVelocityX) * Mathf.Abs(offset.x);

        return new Vector3(
            playerTR.position.x + lookAheadX * mainCamera.orthographicSize * mainCamera.aspect,
            playerTR.position.y + offset.y * mainCamera.orthographicSize,
            transform.position.z
        );
    }
}
