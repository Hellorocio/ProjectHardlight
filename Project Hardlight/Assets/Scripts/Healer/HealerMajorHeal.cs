using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerMajorHeal : Ability
{
    public float baseEffectRange;
    public int baseHealAmt;

    public GameObject rangeIndicatorPrefab;
    private GameObject rangeIndicator;

    public GameObject healEffectPrefab;

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
            Debug.Log("Healer does Major Heal");
            //Check for a single enemy at click position (check small range)
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(selectedPosition, 0.5f);
            foreach (Collider2D collider in hitColliders)
            {
                Fighter hitFighter = collider.gameObject.GetComponent<Fighter>();
                if (hitFighter != null)
                {
                    if (hitFighter.team == CombatInfo.Team.Hero)
                    {
                        //heal selected ally
                        hitFighter.Heal(GetHealAmt());

                        //display heal circle
                        GameObject healEffect = Instantiate(healEffectPrefab);
                        healEffect.transform.parent = hitFighter.transform;
                        healEffect.transform.localPosition = Vector3.zero;
                        healEffect.transform.localScale = Vector3.one;

                        // Lose mana
                        Fighter fighter = gameObject.GetComponent<Fighter>();
                        fighter.LoseMana(fighter.manaCosts);

                        return true;
                    }
                }
            }

            //if we get here then there weren't any enemies right where the player clicked
            Debug.Log("Healer: Selected area does not have a hero");

            return false;
        }

        Debug.Log("Healer: Selelected area is not in range");
        return false;

    }

    public float GetRange()
    {
        return baseEffectRange;
    }

    public float GetHealAmt()
    {
        return baseHealAmt;
    }
}
