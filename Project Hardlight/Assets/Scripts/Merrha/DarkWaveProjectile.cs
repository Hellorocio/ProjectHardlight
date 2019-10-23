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
    }
}
