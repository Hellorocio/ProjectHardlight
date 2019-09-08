﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeBasicAttackAction : BasicAttackAction
{
    public override void DoBasicAttack()
    {
        Fighter thisFighter = GetComponent<Fighter>();
        float damage = thisFighter.basicAttackStats.damage + thisFighter.basicAttackStats.damage * (thisFighter.fighterStats.soul.attackBoost / 100);
        thisFighter.attackTarget.GetComponent<Fighter>().Attack(damage);
    }
}
