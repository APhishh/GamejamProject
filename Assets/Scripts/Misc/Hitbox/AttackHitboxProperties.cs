using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class AttackHitboxProperties : MonoBehaviour
{
    [SerializeField] float Damage;

    public float getDamage()
    {
        return Damage;
    }

}
