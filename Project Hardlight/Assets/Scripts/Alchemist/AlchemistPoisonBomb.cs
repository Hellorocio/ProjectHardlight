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
        /*
        Debug.Log("Poison bomb casted");

        // Hit enemies
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(selectedPosition, GetRadius());
        foreach (Collider2D collider in hitColliders)
        {

            Attackable attackable = collider.gameObject.GetComponent<Attackable>();

            if (attackable != null)
            {
                if (attackable.team == CombatInfo.Team.Enemy)
                {
                    attackable.TakeDamage(GetDamage());
                    //tmp.AddTimedBuff(attackDebuff);
                }
            }

            if (attackable != null)
            {
                attackable.TakeDamage(GetDamage());
                //tmp2.AddTimedBuff(attackDebuff);
            }

        }

        //display boom!
        GameObject boom = Instantiate(poisonBlastPrefab);
        Vector3 boomPos = selectedPosition;
        boomPos.z = 2;
        boom.transform.position = boomPos;
        */
        return true;
    }

    public float GetRadius()
    {
        return baseEffectRadius;
    }
}
