using System.Collections;
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
        if (selectedTarget != null && Vector2.Distance(selectedTarget.transform.position, gameObject.transform.position) < GetRange())
        {
            Debug.Log("in range");
            Fighter selectedFighter = selectedTarget.GetComponent<Fighter>();
            if (selectedFighter != null)
            {
                Debug.Log("pew pew");

                selectedFighter.TakeDamage(GetDamage());
                //add debuff
                selectedFighter.AddTimedBuff(attackDebuff);
                // Lose mana
                selectedFighter.LoseMana(selectedFighter.manaCosts);

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
        return baseDamage;
    }
}
