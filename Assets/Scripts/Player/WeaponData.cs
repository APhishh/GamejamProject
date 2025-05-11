using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Game/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public Sprite weaponIcon;
    public float damage;
}
