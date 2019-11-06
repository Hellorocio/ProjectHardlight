﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaHealingProjectile : MonoBehaviour
{
    public float dmg = 0;
    GameObject target;
    Fighter source;

    private void Start()
    {
        /*
        target = GetComponent<ProjectileMovement>().target;
        source = GetComponent<ProjectileMovement>().source.GetComponent<Fighter>();
        */
    }

    public void Initialize(float damage, Fighter source)
    {
        dmg = damage;
        this.source = source;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("proj hit");
        Attackable hitAttackable = other.GetComponent<Attackable>();

        if (hitAttackable != null)
        {
            if (hitAttackable.team == CombatInfo.Team.Enemy)
            {
                // Deal damage
                hitAttackable.TakeDamage(dmg);

                //heal ninja
                source.GetComponent<Attackable>().Heal(dmg);
                Destroy(gameObject);
            }

        }
        
    }
}