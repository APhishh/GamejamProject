using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCharacter : MonoBehaviour
{
    [SerializeField] Transform playerTR;
    [SerializeField] float followSpeed;
    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(playerTR.position.x,playerTR.position.y,transform.position.z), followSpeed * Time.deltaTime);
    }
}
