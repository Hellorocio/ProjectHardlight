using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaHealingAbility : Ability
{

    public GameObject rangeIndicatorPrefab;
    public GameObject rangeIndicator;

    public GameObject attackTargetUnit;
    public GameObject ninjaProjecilePrefab;

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
        if (Vector2.Distance(transform.position, selectedPosition) < GetRange())
        {
            //do something different based on allight values
            Soul soul = GetComponent<Soul>();
            if (soul != null)
            {
                foreach (AllightAttribute allight in soul.allightAttributes)
                {
                    switch (allight.allightType)
                    {
                        case AllightType.SUNLIGHT:
                            //remember to scale based on allight.baseValue
                            break;
                        case AllightType.MOONLIGHT:
                            break;
                        case AllightType.STARLIGHT:
                            break;
                        default:
                            break;
                    }
                }
            }
            
            // Create projectile
            GameObject shuriken = Instantiate(ninjaProjecilePrefab);
            shuriken.transform.position = transform.position;
            shuriken.GetComponent<NinjaHealingProjectile>().Initialize(GetDamage(), GetComponent<Fighter>());
            shuriken.GetComponent<ProjectileMovement>().SetTarget(selectedPosition);
            shuriken.GetComponent<ProjectileMovement>().StartMovement();

            return true;
        }        
        Debug.Log("out of range");
        return false;

    }

    public override float GetRange()
    {
        return this.baseEffectRange;
    }
}
