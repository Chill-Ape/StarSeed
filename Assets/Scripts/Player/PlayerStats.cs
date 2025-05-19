using UnityEngine;


[CreateAssetMenu(fileName = "PlayerStats", menuName = "Player Stats")]
public class PlayerStats : ScriptableObject
{
  [Header("Config")]
  public int Level;

  public float Health;
  public float MaxHealth;

}