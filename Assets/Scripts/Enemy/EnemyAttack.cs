using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] GameObject hitbox;
    [SerializeField] private Transform player;
    [SerializeField] private bool detected;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            
        }
    }

    IEnumerator Despawn(GameObject obj)
    {
        yield return new WaitForSeconds(0.1f);
        Destroy(obj);
    }
}
