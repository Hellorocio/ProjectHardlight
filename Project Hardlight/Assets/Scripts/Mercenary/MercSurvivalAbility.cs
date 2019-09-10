﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MercSurvivalAbility : Ability
{
    public float baseEffectRange;
    public int baseDamage;

    public GameObject rangeIndicatorPrefab;
    public GameObject rangeIndicator;

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
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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
            //Check for a single enemy at click position (check small range)
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(selectedPosition, 0.5f);
            foreach (Collider2D collider in hitColliders)
            {
                Fighter hitFighter = collider.gameObject.GetComponent<Fighter>();
                if (hitFighter != null)
                {
                    if (hitFighter.team == CombatInfo.Team.Enemy)
                    {
                        hitFighter.TakeDamage(GetDamage());

                        // Lose mana
                        Fighter fighter = gameObject.GetComponent<Fighter>();
                        fighter.LoseMana(fighter.manaCosts);

                        //heal this fighter
                        Fighter thisFighter = gameObject.GetComponent<Fighter>();
                        if (thisFighter.GetHealth() <= thisFighter.fighterStats.maxHealth * 0.1f)
                        {
                            thisFighter.Heal(GetDamage());
                        }

                        return true;
                    }
                }
            }

            //if we get here then there weren't any enemies right where the player clicked
            Debug.Log("Merc: Selected area does not have an enemy");

            return false;
        }

        Debug.Log("Merc: Selelected area is not in range");
        return false;

    }

    public float GetRange()
    {
        return baseEffectRange;
    }

    public float GetDamage()
    {
        Fighter thisFighter = gameObject.GetComponent<Fighter>();
        if (thisFighter.GetHealth() <= thisFighter.fighterStats.maxHealth * 0.1f)
        {
            return baseDamage * 3;
        }
        return baseDamage;
    }
}