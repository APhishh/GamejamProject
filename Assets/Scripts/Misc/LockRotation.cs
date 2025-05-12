using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class LockRotation : MonoBehaviour
{
    quaternion lockedRotation;
    Vector2 lockedPos;
    void Start()
    {
        lockedRotation = transform.rotation;
        lockedPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
            transform.position = lockedPos;
            transform.rotation = lockedRotation;
    }
}
