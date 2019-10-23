using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkWaveProjectile : MonoBehaviour
{
    // Set by SetEffectNumbers
    public int damageAmount;
    public int healAmount;

    // Don't set
    public HashSet<Fighter> affectedFighters;

    public void SetEffectNumbers(int damageAmt, int healAmt)
    {
        affectedFighters = new HashSet<Fighter>();
        damageAmount = damageAmt;
        healAmount = healAmt;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("trigger enter");
        Fighter hitFighter = other.gameObject.GetComponent<Fighter>();
        if (hitFighter != null && !affectedFighters.Contains(hitFighter))
        {
            if (hitFighter.team == CombatInfo.Team.Hero)
            {
                Debug.Log("heal " + healAmount);

                hitFighter.Heal(healAmount);
            }
            else if (hitFighter.team == CombatInfo.Team.Enemy)
            {
                Debug.Log("damage " + damageAmount);

                hitFighter.TakeDamage(damageAmount);
            }

            affectedFighters.Add(hitFighter);
        }
    }
}
