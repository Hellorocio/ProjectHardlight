using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageCurseAbility : Ability
{

    public GameObject rangeIndicatorPrefab;
    public Buff defenseDebuff;

    public GameObject lightPrisonPrefab;

    private bool targeting;
    
    [Header("Donut Touch")]
    public GameObject rangeIndicator;
    public GameObject attackTargetUnit;
    
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
            Attackable attackable = selectedTarget.gameObject.GetComponent<Attackable>();
            if (hitFighter != null && hitFighter.team == CombatInfo.Team.Enemy)
            {
                Debug.Log("Mage Light Prison");

                // Deal damage and add debuff
                hitFighter.TakeDamage(GetDamage());
                attackable.AddBuff(defenseDebuff);

                //display light prison
                GameObject lightPrison = Instantiate(lightPrisonPrefab, selectedTarget.transform);
                lightPrison.transform.localPosition = Vector3.zero;
                lightPrison.transform.localScale = Vector3.one;

                return true;
            }


        }
        return false;

    }

    public override float GetRange()
    {
        return this.baseEffectRange;
    }
}
