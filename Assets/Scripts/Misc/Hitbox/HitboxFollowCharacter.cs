using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxFollowCharacter : MonoBehaviour
{
  [SerializeField] Transform character;
  [SerializeField] Vector2 dir;
    // Update is called once per frame
    void Update()
    {
        if (character != null)
        {
            transform.position = character.position + new Vector3(dir.x*1,dir.y*1);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Set(Transform charac, Vector2 dir)
    {
        character =charac;
        this.dir = dir;
    }
}
