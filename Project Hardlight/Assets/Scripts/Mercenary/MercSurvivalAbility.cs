using System.Collections;
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
        Debug.Log("Merc's survival ability called");
        Debug.Log(selectedTarget);
        Debug.Log("Distance equation == " + (Vector2.Distance(selectedTarget.transform.position, gameObject.transform.position) < GetRange()));
        if (selectedTarget != null && Vector2.Distance(selectedTarget.transform.position, gameObject.transform.position) < GetRange())
        {
            Debug.Log("in range");
            Fighter selectedFighter = selectedTarget.GetComponent<Fighter>();
            if (selectedFighter != null)
            {
                selectedFighter.TakeDamage(GetDamage());
                // Lose mana
                selectedFighter.LoseMana(selectedFighter.manaCosts);

                // heal this fighter
                Fighter thisFighter = gameObject.GetComponent<Fighter>();
                if (thisFighter.GetHealth() <= thisFighter.fighterStats.maxHealth * 0.1f)
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
