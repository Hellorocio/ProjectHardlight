﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageCurseAbility : Ability
{
    public float baseEffectRange;

    public GameObject rangeIndicatorPrefab;
    public GameObject rangeIndicator;
    public BuffObj attackDebuff;

    public GameObject attackTargetUnit;
    public GameObject lightPrisonPrefab;

    private bool targeting;
   

    private void Update()
    {
        if (targeting)
        {
            // Update positions
            rangeIndicator.transform.position = gameObject.transform.position;
        }
    }


    public override bool StartTargeting()
    {
        targeting = true;

        rangeIndicator = Instantiate(rangeIndicatorPrefab);
        rangeIndicator.name = "Range";
        rangeIndicator.transform.localScale *= GetRange();

        return true;
    }

    public override void StopTargeting()
    {
        Destroy(rangeIndicator);
        targeting = false;
    }

    public override bool DoAbility()
    {
        if (selectedTarget != null && Vector2.Distance(selectedTarget.transform.position, gameObject.transform.position) < GetRange())
        {
            Fighter hitFighter = selectedTarget.gameObject.GetComponent<Fighter>();
            if (hitFighter != null && hitFighter.team == CombatInfo.Team.Enemy)
            {
                Debug.Log("Mage Light Prison");

                hitFighter.TakeDamage(GetDamage());
                //display light prison
                GameObject lightPrison = Instantiate(lightPrisonPrefab, selectedTarget.transform);
                lightPrison.transform.localPosition = Vector3.zero;
                lightPrison.transform.localScale = Vector3.one;
                return true;
            }

               
            MonsterAI monster = selectedTarget.gameObject.GetComponent<MonsterAI>();
            if (monster != null)
            {
                monster.TakeDamage(GetDamage());
                //display light prison
                GameObject lightPrison = Instantiate(lightPrisonPrefab, selectedTarget.transform);
                lightPrison.transform.localPosition = Vector3.zero;
                lightPrison.transform.localScale = Vector3.one;
                return true;
            }

           
        }
        return false;

    }

    public float GetRange()
    {
        return baseEffectRange;
    }
}
