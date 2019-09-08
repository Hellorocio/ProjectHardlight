using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Battle/BasicAttackStats", order = 1)]
public class BasicAttackStats : ScriptableObject
{
    public float range = 2.0f;
    public float attackSpeed = 1.0f;
    public int damage = 100;
}
