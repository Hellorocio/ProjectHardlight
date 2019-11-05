using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeBasicAttackAction : BasicAttackAction
{
    public override void DoBasicAttack(Fighter sourceFighter, GameObject target)
    {
        float damage = sourceFighter.GetBasicAttackDamage();

        Fighter enemyFighter = target.GetComponent<Fighter>();

        if (enemyFighter != null)
        {
            enemyFighter.TakeDamage(damage);
            sourceFighter.GainMana(10);
        }

        //target.GetComponent<Fighter>().TakeDamage(damage);

        
    }
}
