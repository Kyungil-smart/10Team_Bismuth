using UnityEngine;

[CreateAssetMenu(fileName = "Player Data", menuName = "Bismuth/Player Database", order = 0)]
public class PlayerSO : ScriptableObject
{
    [SerializeField] private int level;
    [SerializeField] private int gold;
    [SerializeField] private int maxBaseHealth;

    public int MaxBaseHealth { get => maxBaseHealth; set => maxBaseHealth = value; }
    public int Gold { get => gold; set => gold = value; }
    public int Level { get => level; set => level = value; }
}
