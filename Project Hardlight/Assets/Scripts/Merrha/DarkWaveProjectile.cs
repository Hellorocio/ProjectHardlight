using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkWaveProjectile : MonoBehaviour
{
    // Set by Intialize
    public int damageAmount;
    public int healAmount;
    public int damageIncrease;
    public int healTrailAmount;
    
    // Fix if u want
    public GameObject healTrailPrefab;
    public float healTrailDropFreq = 0.3f;
    public float healTrailDuration = 10.0f;

    // Don't set
    [Header("donut touch")]
    private bool initialized = false;
    public HashSet<Attackable> affectedAttackables;
    public Vector3 startPosition;
    public float maxDistance;

    private void Update()
    {
        if (initialized)
        {
            // Destroy after distance
            if (Vector2.Distance(transform.position, startPosition) > maxDistance)
            {
                Destroy(gameObject);
            }
        }
    }

    public void Initialize(Vector3 startPos, int damageAmt, int healAmt, float maxDistance, int damageIncreasedPerHit, int healTrailHPS)
    {
        startPosition = startPos;
        affectedAttackables = new HashSet<Attackable>();
        damageAmount = damageAmt;
        healAmount = healAmt;
        damageIncrease = damageIncreasedPerHit;
        this.maxDistance = maxDistance;
        healTrailAmount = healTrailHPS;
        initialized = true;
        
        // If has heal trail amount, drop every few seconds 
        if (healTrailAmount > 0)
        {
            StartCoroutine(DropHealTrail());
        }
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
                hitAttackable.TakeDamage(damageAmount + damageIncrease*affectedAttackables.Count);
            }
            affectedAttackables.Add(hitAttackable);
        }
    }

    IEnumerator DropHealTrail()
    {
        while (true)
        {
            GameObject trailBit = Instantiate(healTrailPrefab);
            trailBit.transform.position = new Vector2(transform.position.x, transform.position.y);
            trailBit.GetComponent<HealSpot>().duration = healTrailDuration;
            trailBit.GetComponent<HealPerSecondBuff>().healPerTick = healTrailAmount;
            yield return new WaitForSeconds(healTrailDropFreq);
        }
    }
}
