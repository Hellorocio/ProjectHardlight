using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MerrhaBasicAttack : BasicAttackAction
{

    public GameObject gooPrefab;
    
    public override void DoBasicAttack(Fighter sourceFighter, GameObject target)
    {
        // Appearance
        GameObject gooAttack = Instantiate(gooPrefab);
        gooAttack.transform.parent = target.transform.Find("Appearance");
        gooAttack.transform.localPosition = Vector3.zero;

        float damage = sourceFighter.GetBasicAttackDamage();
        MonsterAI enemyMonster = target.GetComponent<MonsterAI>();

        if (enemyMonster != null)
        {
            enemyMonster.TakeDamage(damage);
            sourceFighter.GainMana(10);
        }

        if (sourceFighter.team == CombatInfo.Team.Hero)
        {
            sourceFighter.GainMana(10);
        }
    }
}
