using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MerrhaBasicAttack : BasicAttackAction
{

    public GameObject gooPrefab;
    
    public override void DoBasicAttack(Fighter sourceFighter, GameObject target)
    {
        Debug.Log("Merrha basic attack");
        
        // Appearance
        GameObject gooAttack = Instantiate(gooPrefab);
        gooAttack.transform.parent = target.transform.Find("Appearance");
        gooAttack.transform.localPosition = Vector3.zero;

        float damage = sourceFighter.GetBasicAttackDamage();
        target.GetComponent<Fighter>().TakeDamage(damage);

        if (sourceFighter.team == CombatInfo.Team.Hero)
        {
            sourceFighter.GainMana(10);
        }
    }
}
