using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeBasicAttackAction : BasicAttackAction
{
    public override void DoBasicAttack(Fighter sourceFighter, GameObject target)
    {
        float damage = sourceFighter.GetBasicAttackDamage();

        Attackable attackableEnemy = target.GetComponent<Attackable>();

        attackableEnemy.TakeDamage(damage);
        sourceFighter.GainMana(10);

    }
}
