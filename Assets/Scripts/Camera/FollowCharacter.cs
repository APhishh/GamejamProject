using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCharacter : MonoBehaviour
{
    [SerializeField] Transform playerTR;
    [SerializeField] float followSpeed;
    [SerializeField] Vector2 offset = new Vector2(0.33f, 0.33f); // Offset for the two-thirds rule (x = horizontal, y = vertical)

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (playerTR != null)
        {
            // Calculate the target position based on the two-thirds rule
            Vector3 targetPosition = new Vector3(
                playerTR.position.x + offset.x * mainCamera.orthographicSize * mainCamera.aspect,
                playerTR.position.y + offset.y * mainCamera.orthographicSize,
                transform.position.z
            );

            // Smoothly move the camera towards the target position
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
    }
}
