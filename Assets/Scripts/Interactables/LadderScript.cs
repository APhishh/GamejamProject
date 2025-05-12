using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderScript : MonoBehaviour
{
    [SerializeField]private GameObject player;
    [SerializeField]private float startPoint,endPoint;
    [SerializeField]private GameObject firstLadder,lastLadder;
    
    void Start()
    {
        Renderer firstRenderer = firstLadder.GetComponent<Renderer>();
        Renderer lastRenderer = lastLadder.GetComponent<Renderer>();
        endPoint = lastRenderer.bounds.max.y;
        startPoint = firstRenderer.bounds.min.y;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
       if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
       {
            player = collision.gameObject;
       } 
    }

    void OnTriggerExit2D(Collider2D collision)
    {
       if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
       {
            player = null;
       } 
    }

    void Update()
    {

        
        if(player != null)
        {
            PlayerMovement playerMovementScript = player.GetComponent<PlayerMovement>();
            Rigidbody2D playerRB = player.GetComponent<Rigidbody2D>();

            if(playerMovementScript.getClimbing())
            {
                Vector2 playerPos = player.transform.position; 
                player.transform.position = Vector2.MoveTowards(playerPos, new Vector2(transform.position.x, Mathf.Clamp(playerPos.y,startPoint,endPoint)),5*Time.deltaTime);
            }
            
            if (Input.GetKeyDown(KeyCode.W) && playerMovementScript.getClimbing() == false)
            {   
                playerMovementScript.setClimbing(true);
            }

            if(Input.GetAxis("Vertical") > 0 && player.transform.position.y >= endPoint && playerMovementScript.getClimbing())
            {
                playerRB.velocity = new Vector2(0,0);
            }
        }
    }


}
