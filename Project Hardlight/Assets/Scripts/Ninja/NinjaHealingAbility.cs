using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaHealingAbility : Ability
{
    public float baseEffectRange;
    public int baseDamage;

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
        rangeIndicator.transform.localScale *= 2 * GetRange();

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
            Fighter selectedFighter = selectedTarget.GetComponent<Fighter>();
            if (selectedFighter != null && selectedFighter.team == CombatInfo.Team.Enemy)
            {
                Debug.Log("Ninja projectile + heal");

                //create projectiles
                Vector3 blastPos = transform.position;
                GameObject blast = Instantiate(ninjaProjecilePrefab);
                blast.transform.localPosition = blastPos;

                blast.GetComponent<ProjectileMovement>().SetTarget(gameObject, selectedTarget);
                blast.GetComponent<NinjaHealingProjectile>().dmg = GetDamage();

                blastPos.y += 2;
                GameObject blast2 = Instantiate(ninjaProjecilePrefab);
                blast2.transform.localPosition = blastPos;
                
                
                blast2.GetComponent<ProjectileMovement>().SetTarget(gameObject, selectedTarget);
                blast2.GetComponent<NinjaHealingProjectile>().dmg = GetDamage();

                blastPos.y -= 4;
                GameObject blast3 = Instantiate(ninjaProjecilePrefab);
                blast3.transform.localPosition = blastPos;


                blast3.GetComponent<ProjectileMovement>().SetTarget(gameObject, selectedTarget);
                blast3.GetComponent<NinjaHealingProjectile>().dmg = GetDamage();
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
