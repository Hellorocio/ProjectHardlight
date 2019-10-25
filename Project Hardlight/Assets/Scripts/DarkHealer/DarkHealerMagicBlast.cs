using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkHealerMagicBlast : Ability
{
    public float baseEffectRange;

    public GameObject rangeIndicatorPrefab;
    public GameObject rangeIndicator;

    public GameObject attackTargetUnit;
    public GameObject magicBlastPrefab;

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
        rangeIndicator.transform.localScale *= GetRange();

        return true;
    }

    public override void StopTargeting()
    {
        targeting = false;
        Destroy(rangeIndicator);
    }

    public override bool DoAbility()
    {
        if (selectedTarget != null && Vector2.Distance(selectedTarget.transform.position, gameObject.transform.position) < GetRange())
        {
            MonsterAI monster = selectedTarget.GetComponent<MonsterAI>();
            Fighter tmp = selectedTarget.GetComponent<Fighter>();
          

            if (monster != null)
            {
                GameObject blast = Instantiate(magicBlastPrefab);
                blast.transform.localPosition = transform.position;

                //blast.GetComponent<ProjectileMovement>().SetTarget(gameObject, selectedTarget);
                blast.GetComponent<DarkHealerProjectile>().dmg = GetDamage();
                return true;
            }

            if (tmp != null)
            {
                GameObject blast = Instantiate(magicBlastPrefab);
                blast.transform.localPosition = transform.position;

                //blast.GetComponent<ProjectileMovement>().SetTarget(gameObject, selectedTarget);
                blast.GetComponent<DarkHealerProjectile>().dmg = GetDamage();
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
