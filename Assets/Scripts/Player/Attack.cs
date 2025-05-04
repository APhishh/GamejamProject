using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Attack : MonoBehaviour
{

    [SerializeField] GameObject hitbox;

   

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;
            Vector3 dir = (mousePos - transform.position).normalized;
            float angle = Mathf.Atan2(dir.y,dir.x) * Mathf.Rad2Deg;
            GameObject newObj = Instantiate(hitbox, transform.position + new Vector3(dir.x*1,dir.y*1), Quaternion.Euler(0, 0, angle));
            StartCoroutine(Despawn(newObj));
        }
    }

    IEnumerator Despawn(GameObject obj)
    {
        yield return new WaitForSeconds(0.1f);
        Destroy(obj);
    }
}
