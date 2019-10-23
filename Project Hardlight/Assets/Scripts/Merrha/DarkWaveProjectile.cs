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
    private bool initialized = false;
    public HashSet<Fighter> affectedFighters;
    public HashSet<GenericMeleeMonster> affectedFighters1;
    public HashSet<GenericRangedMonster> affectedFighters2;
    public Vector3 startPosition;
    public float maxDistance;

    private void Update()
    {
        if (initialized)
        {
            if (Vector2.Distance(transform.position, startPosition) > maxDistance)
            {
                Destroy(gameObject);
            }
        }
    }

    public void Initialize(Vector3 startPos, int damageAmt, int healAmt, float maxDistance)
    {
        startPosition = startPos;
        affectedFighters = new HashSet<Fighter>();
        affectedFighters1 = new HashSet<GenericMeleeMonster>();
        affectedFighters2 = new HashSet<GenericRangedMonster>();
        damageAmount = damageAmt;
        healAmount = healAmt;
        this.maxDistance = maxDistance;
        initialized = true;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Fighter hitFighter = other.gameObject.GetComponent<Fighter>();
        if (hitFighter != null && !affectedFighters.Contains(hitFighter))
        {
            if (hitFighter.team == CombatInfo.Team.Hero)
            {
                hitFighter.Heal(healAmount);
            }
            else if (hitFighter.team == CombatInfo.Team.Enemy)
            {
                hitFighter.TakeDamage(damageAmount);
            }

            affectedFighters.Add(hitFighter);
        }

        GenericMeleeMonster tmp2 = other.gameObject.GetComponent<GenericMeleeMonster>();
        GenericRangedMonster tmp3 = other.gameObject.GetComponent<GenericRangedMonster>();


        if (tmp2 != null && !affectedFighters1.Contains(tmp2))
        {
            tmp2.TakeDamage(damageAmount);
            affectedFighters1.Add(tmp2);
        }

        if (tmp3 != null && !affectedFighters2.Contains(tmp3))
        {
            tmp3.TakeDamage(damageAmount);
            affectedFighters2.Add(tmp3);
        }
    }
}
