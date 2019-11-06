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
    public HashSet<Attackable> affectedAttackables;
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
        affectedAttackables = new HashSet<Attackable>();
        damageAmount = damageAmt;
        healAmount = healAmt;
        this.maxDistance = maxDistance;
        initialized = true;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Attackable hitAttackable = other.gameObject.GetComponent<Attackable>();
        if (hitAttackable != null && !affectedAttackables.Contains(hitAttackable))
        {
            if (hitAttackable.team == CombatInfo.Team.Hero)
            {
                hitAttackable.Heal(healAmount);
            }
            else if (hitAttackable.team == CombatInfo.Team.Enemy)
            {
                hitAttackable.TakeDamage(damageAmount);
            }
            affectedAttackables.Add(hitAttackable);
        }

    }
}
