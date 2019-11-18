using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeBasicAttackAction : BasicAttackAction
{
    public float animationDelay;
    private Coroutine basicAttack;
    public override void DoBasicAttack(Fighter sourceFighter, GameObject target)
    {
        if(basicAttack == null)
        {
            basicAttack = StartCoroutine(BasicAttackWithAnimationDelay(sourceFighter, target));
        }
        
    }

    IEnumerator BasicAttackWithAnimationDelay(Fighter sourceFighter, GameObject target)
    {
        yield return new WaitForSeconds(animationDelay);
        float damage = sourceFighter.GetBasicAttackDamage();

        Attackable attackableEnemy = target.GetComponent<Attackable>();

        attackableEnemy.TakeDamage(damage);
        sourceFighter.GainMana(10);
        basicAttack = null;
    }

    }
