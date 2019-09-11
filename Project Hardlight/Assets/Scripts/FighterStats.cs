using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Battle/FighterStats", order = 1)]
public class FighterStats : ScriptableObject
{
    public int maxHealth = 100;
    public int maxMana = 10;
    public float movementSpeed = 1.0f;
}
