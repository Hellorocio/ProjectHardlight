using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeBasicAttackAction : BasicAttackAction
{
    public override void DoBasicAttack(Fighter sourceFighter, GameObject target)
    {
        float damage = sourceFighter.GetBasicAttackDamage();

        Fighter tmp = target.GetComponent<Fighter>();
        MonsterAI monster = target.GetComponent<MonsterAI>();

        if (tmp != null)
        {
            tmp.TakeDamage(damage);
            sourceFighter.GainMana(10);
        }

        if (monster != null)
        {
            monster.TakeDamage(damage);
            sourceFighter.GainMana(10);
        }

        //target.GetComponent<Fighter>().TakeDamage(damage);

        
    }
}
