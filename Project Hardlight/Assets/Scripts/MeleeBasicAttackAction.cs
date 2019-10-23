using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeBasicAttackAction : BasicAttackAction
{
    public override void DoBasicAttack(Fighter sourceFighter, GameObject target)
    {
        float damage = sourceFighter.GetBasicAttackDamage();

        Fighter tmp = target.GetComponent<Fighter>();
        GenericMeleeMonster tmp2 = target.GetComponent<GenericMeleeMonster>();
        GenericRangedMonster tmp3 = target.GetComponent<GenericRangedMonster>();

        if (tmp != null)
        {
            tmp.TakeDamage(damage);
            sourceFighter.GainMana(10);
        }

        if (tmp2 != null)
        {
            tmp2.TakeDamage(damage);
            sourceFighter.GainMana(10);
        }

        if (tmp3 != null)
        {
            tmp3.TakeDamage(damage);
            sourceFighter.GainMana(10);
        }
        //target.GetComponent<Fighter>().TakeDamage(damage);

        if (sourceFighter.team == CombatInfo.Team.Hero)
        {
            sourceFighter.GainMana(10);
        }
    }
}
