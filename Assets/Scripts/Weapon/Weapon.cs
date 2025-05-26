using UnityEngine;

public enum WeaponType
{
    Magic,
    Melee
}

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class Weapon : ScriptableObject
{
    [Header("Config")]
    public string Name;
    public Sprite Icon;
    public WeaponType WeaponType;
    public float Damage;
    public float RequiredMana;
    public Projectile ProjectilePrefab;
    public float KnockbackForce = 1f;
}
