using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MercSurvivalAbility : Ability
{
    public float baseEffectRange;

    public GameObject rangeIndicatorPrefab;
    public GameObject rangeIndicator;

    public GameObject attackTargetUnit;

    private bool targeting;

    public void Start()
    {
        abilityName = "Survive";
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
        rangeIndicator.transform.localScale *=  GetRange();

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
            Debug.Log("Merc survival cast");
            Fighter selectedFighter = selectedTarget.GetComponent<Fighter>();
            if (selectedFighter != null && selectedFighter.team == CombatInfo.Team.Enemy)
            {
                selectedFighter.TakeDamage(GetDamage());

                // heal this fighter
                Fighter thisFighter = gameObject.GetComponent<Fighter>();
                if (thisFighter.GetHealth() <= thisFighter.GetMaxHealth() * 0.5f)
                {
                    thisFighter.Heal(GetDamage());
                }

                return true;
            }
            GenericMeleeMonster tmp2 = selectedTarget.GetComponent<GenericMeleeMonster>();
            GenericRangedMonster tmp3 = selectedTarget.GetComponent<GenericRangedMonster>();


            if (tmp2 != null)
            {
                tmp2.TakeDamage(GetDamage());
                Fighter thisFighter = gameObject.GetComponent<Fighter>();
                if (thisFighter.GetHealth() <= thisFighter.GetMaxHealth() * 0.5f)
                {
                    thisFighter.Heal(GetDamage());
                }

                return true;
            }

            if (tmp3 != null)
            {
                tmp3.TakeDamage(GetDamage());
                Fighter thisFighter = gameObject.GetComponent<Fighter>();
                if (thisFighter.GetHealth() <= thisFighter.GetMaxHealth() * 0.5f)
                {
                    thisFighter.Heal(GetDamage());
                }

                return true;
            }
        }
        return false;

    }

    public float GetRange()
    {
        return baseEffectRange;
    }

    public override float GetDamage()
    {
        Fighter thisFighter = gameObject.GetComponent<Fighter>();
        float dmg = thisFighter.GetDamage(baseDamage);
        if (thisFighter.GetHealth() <= thisFighter.GetMaxHealth() * 0.5f)
        {
            return dmg * 3;
        }
        return dmg;
    }
}
