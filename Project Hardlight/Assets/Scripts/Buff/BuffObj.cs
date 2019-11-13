using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Battle/BuffObj", order = 1)]
public class BuffObj : ScriptableObject
{
    //percent stat buff (out of 1.0)
    public float movementSpeedBoost;
    public float attackSpeedBoost;
    public float defenseBoost;
    public float attackBoost;
    public float manaGenerationBoost;

    public float timeActive;
}
