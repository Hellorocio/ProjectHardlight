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
        GenericMonsterAI monster = target.GetComponent<GenericMonsterAI>();

        if(monster != null)
        {
            monster.TakeDamage(damage);
        }
        
        //target.GetComponent<Fighter>().TakeDamage(damage);

        if (sourceFighter.team == CombatInfo.Team.Hero)
        {
            sourceFighter.GainMana(10);
        }
    }
}
