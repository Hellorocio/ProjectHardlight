using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericRangedMonsterProjectile : MonoBehaviour
{
    public float damage;
    // Adds this to something if not null
    public Buff buff = null;
    private void OnTriggerEnter2D(Collider2D other)
    {
        //GameObject target = GetComponent<ProjectileMovement>().targetObject;
        Attackable target = other.GetComponent<Attackable>();
        
        if (target != null && target.team == CombatInfo.Team.Hero)
        {
            //Fighter sourceFighter = GetComponent<ProjectileMovement>().source.GetComponent<Fighter>();

            // Deal damage
            //float damage = sourceFighter.GetBasicAttackDamage();
            target.TakeDamage(damage);

            // Gain mana
            //sourceFighter.GainMana(10);

            if (buff != null)
            {
                target.AddBuff(buff);
            }

            Destroy(gameObject);
        }
        
    }
}
