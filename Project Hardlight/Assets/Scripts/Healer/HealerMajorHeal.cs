using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerMajorHeal : Ability
{
    public int baseHealAmt;

    public GameObject rangeIndicatorPrefab;
    private GameObject rangeIndicator;

    public GameObject healEffectPrefab;

    public GameObject attackTargetUnit;

    private bool targeting;

    public void Start()
    {
        targeting = false;
        abilityName = "Major Heal";
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
        rangeIndicator.transform.localScale *=GetRange();

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
            Debug.Log("Healer does Major Heal");
            Fighter hitFighter = selectedTarget.GetComponent<Fighter>();
            if (hitFighter != null && hitFighter.GetComponent<Attackable>().team == CombatInfo.Team.Hero)
            {
                //heal selected ally
                hitFighter.GetComponent<Attackable>().Heal(GetHealAmt());

                //display heal circle
                GameObject healEffect = Instantiate(healEffectPrefab);
                healEffect.transform.parent = hitFighter.transform;
                healEffect.transform.localPosition = Vector3.zero;
                healEffect.transform.localScale = Vector3.one;

                return true;
            }

            //if we get here then there weren't any enemies right where the player clicked
            Debug.Log("Healer: Selected area does not have a hero");

            return false;
        }

        Debug.Log("Healer: Selelected area is not in range");
        return false;

    }

    public override float GetRange()
    {
        return this.baseEffectRange;
    }

    public float GetHealAmt()
    {
        return baseHealAmt;
    }
}
