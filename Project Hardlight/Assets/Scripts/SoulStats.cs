using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Battle/SoulStats", order = 1)]
public class SoulStats : ScriptableObject
{
    //attribute amount
    public int moonlight;
    public int starlight;
    public int sunlight;

    //percent stat buff
    public float healthBoost;
    public float movementSpeedBoost;
    public float attackSpeedBoost;
    public float defenseBoost;
    public float attackBoost;
    public float manaGenerationBoost;

}
