﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageCurseAbility : Ability
{
    public float baseEffectRange;
    public int baseDamage;

    public GameObject rangeIndicatorPrefab;
    public GameObject rangeIndicator;
    public BuffObj attackDebuff;

    public GameObject attackTargetUnit;

    private bool targeting;

    public void Start()
    {
        targeting = false;
    }

    private void Update()
    {
        if (targeting)
        {
            // Update positions
            rangeIndicator.transform.position = gameObject.transform.position;
            Debug.Log("what");
        }
    }


    public override bool StartTargeting()
    {
        targeting = true;

        rangeIndicator = Instantiate(rangeIndicatorPrefab);
        rangeIndicator.name = "Range";
        rangeIndicator.transform.localScale *= 2 * GetRange();

        return true;
    }

    public override void StopTargeting()
    {
        Destroy(rangeIndicator);
        targeting = false;
    }

    public override bool DoAbility()
    {
        if (Vector2.Distance(selectedPosition, gameObject.transform.position) < GetRange())
        {
            Fighter selectedFighter = selectedTarget.GetComponent<Fighter>();
            if (selectedFighter != null)
            {
                selectedFighter.TakeDamage(GetDamage());
                //add debuff
                selectedFighter.AddTimedBuff(attackDebuff);
                // Lose mana
                selectedFighter.LoseMana(selectedFighter.manaCosts);

                return true;
            }
            else
            {
                return false;
            }
        }

        Debug.Log("Mage: Selelected area is not in range");
        return false;

    }

    public float GetRange()
    {
        return baseEffectRange;
    }

    public float GetDamage()
    {
        return baseDamage;
    }
}