using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeBasicAttackAction : BasicAttackAction
{
    public override void DoBasicAttack(Fighter sourceFighter, GameObject target)
    {
        float damage = sourceFighter.GetBasicAttackDamage();
        target.GetComponent<Fighter>().TakeDamage(damage);

        if (sourceFighter.team == CombatInfo.Team.Hero)
        {
            sourceFighter.GainMana(10);
        }
    }
}
