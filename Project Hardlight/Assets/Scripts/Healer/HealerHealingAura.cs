using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerHealingAura : Ability
{
    public float baseEffectRange;
    public float baseEffectRadius;
    public float baseHealAmt;

    public GameObject rangeIndicatorPrefab;
    public GameObject radiusIndicatorPrefab;

    public GameObject rangeIndicator;
    public GameObject radiusIndicator;

    private bool targeting;

    public void Start()
    {
        targeting = false;
    }

    public void Update()
    {
        if (targeting)
        {
            // Update positions
            rangeIndicator.transform.position = gameObject.transform.position;
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            radiusIndicator.transform.position = new Vector3(mousePos.x, mousePos.y, radiusIndicator.transform.position.z);
        }
    }

    public override bool StartTargeting()
    {
        targeting = true;

        rangeIndicator = Instantiate(rangeIndicatorPrefab);
        rangeIndicator.name = "Range";
        rangeIndicator.transform.localScale *= 2 * GetRange();

        radiusIndicator = Instantiate(radiusIndicatorPrefab);
        radiusIndicator.name = "Radius";
        radiusIndicator.transform.localScale *= 2 * GetRadius();

        return true;
    }

    public override void StopTargeting()
    {
        targeting = false;

        Destroy(rangeIndicator);
        Destroy(radiusIndicator);
    }

    public override bool DoAbility()
    {
        if (Vector2.Distance(selectedPosition, gameObject.transform.position) < GetRange())
        {
            Debug.Log("AoE heal casted");

            //heal allies
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(selectedPosition, GetRadius());
            foreach (Collider2D collider in hitColliders)
            {
                Fighter hitFighter = collider.gameObject.GetComponent<Fighter>();
                if (hitFighter != null)
                {
                    if (hitFighter.team == CombatInfo.Team.Hero)
                    {
                        hitFighter.Heal(GetHealAmt());
                    }
                }
            }

            // Lose mana
            Fighter fighter = gameObject.GetComponent<Fighter>();
            fighter.LoseMana(fighter.manaCosts);

            return true;
        }
        else
        {
            Debug.Log("AoE blast out of range");
            return false;
        }
    }

    public float GetRange()
    {
        return baseEffectRange;
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
