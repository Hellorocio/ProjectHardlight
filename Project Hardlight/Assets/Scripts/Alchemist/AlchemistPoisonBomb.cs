using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlchemistPoisonBomb : Ability
{
    public float baseEffectRadius;
    public BuffObj attackDebuff;

    public GameObject poisonBlastPrefab;

    private bool targeting;

    public void Start()
    {
        targeting = false;
    }
    public override bool StartTargeting()
    {
        //targeting = true;
        return true;
    }

    public override void StopTargeting()
    {
        targeting = false;
    }

    public override bool DoAbility()
    {
        Debug.Log("Poison bomb casted");

        // Hit enemies
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(selectedPosition, GetRadius());
        foreach (Collider2D collider in hitColliders)
        {

            Fighter tmp = collider.gameObject.GetComponent<Fighter>();
            GenericMeleeMonster tmp2 = collider.gameObject.GetComponent<GenericMeleeMonster>();
            GenericRangedMonster tmp3 = collider.gameObject.GetComponent<GenericRangedMonster>();

            if (tmp != null)
            {
                if (tmp.team == CombatInfo.Team.Enemy)
                {
                    tmp.TakeDamage(GetDamage());
                    //tmp.AddTimedBuff(attackDebuff);
                }
            }

            if (tmp2 != null)
            {
                tmp2.TakeDamage(GetDamage());
                //tmp2.AddTimedBuff(attackDebuff);
            }

            if (tmp3 != null)
            {
                tmp3.TakeDamage(GetDamage());
                //tmp3.AddTimedBuff(attackDebuff);
            }
            //Fighter hitFighter = collider.gameObject.GetComponent<Fighter>();
            //if (hitFighter != null)
            //{
            //    if (hitFighter.team == CombatInfo.Team.Enemy)
            //    {
            //        hitFighter.TakeDamage(GetDamage());

            //        //add debuff
            //        hitFighter.AddTimedBuff(attackDebuff);
            //    }
            //}
        }

        //display boom!
        GameObject boom = Instantiate(poisonBlastPrefab);
        Vector3 boomPos = selectedPosition;
        boomPos.z = 2;
        boom.transform.position = boomPos;

        return true;
    }

    public float GetRadius()
    {
        return baseEffectRadius;
    }
}
