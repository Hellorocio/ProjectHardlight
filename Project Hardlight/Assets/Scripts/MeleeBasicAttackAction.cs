using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeBasicAttackAction : BasicAttackAction
{
    public override void DoBasicAttack()
    {
        Fighter thisFighter = GetComponent<Fighter>();
        int damage = thisFighter.basicAttackStats.damage;
        thisFighter.attackTarget.GetComponent<Fighter>().Attack(damage);
    }
}
