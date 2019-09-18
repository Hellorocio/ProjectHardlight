﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlchemistHeal : Ability
{
    public float baseEffectRadius;
    public int baseHealAmt;
    public BuffObj healBuff;

    public GameObject healyCircle;

    private bool targeting;

    public void Start()
    {
        targeting = false;
    }
    public override bool StartTargeting()
    {
        //targeting = true;
        return true;
    }

    public override void StopTargeting()
    {
        targeting = false;
    }

    public override bool DoAbility()
    {
        Debug.Log("Alchemist heal casted");

        // Hit enemies
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(selectedPosition, GetRadius());
        foreach (Collider2D collider in hitColliders)
        {
            Fighter hitFighter = collider.gameObject.GetComponent<Fighter>();
            if (hitFighter != null)
            {
                if (hitFighter.team == CombatInfo.Team.Hero)
                {
                    hitFighter.Heal(GetHealAmt());

                    //add buff
                    hitFighter.AddTimedBuff(healBuff);
                }
            }
        }

        //display heal circle
        GameObject circle = Instantiate(healyCircle);
        Vector3 circlePos = selectedPosition;
        circlePos.z = 2;
        circle.transform.position = circlePos;

        return true;
    }

    public float GetRadius()
    {
        return baseEffectRadius;
    }

    public float GetHealAmt()
    {
        return baseHealAmt;
    }
}
