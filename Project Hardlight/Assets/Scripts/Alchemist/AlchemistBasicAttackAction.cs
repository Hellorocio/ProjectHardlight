using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlchemistBasicAttackAction : BasicAttackAction
{

    public GameObject alchemistBasicAttackPrefab;
    public float damageRadius = 3f;

    Fighter thisFighter;
    // Used for offsetting to match animation
    public float animationDelay;

    public override void DoBasicAttack(Fighter sourceFighter, GameObject target)
    {
        thisFighter = sourceFighter;
        StartCoroutine(BasicAttackWithAnimationDelay(target));
    }

    IEnumerator BasicAttackWithAnimationDelay(GameObject target)
    {
        // TODO(mchi) scale animation delay to attack speed changes
        yield return new WaitForSeconds(animationDelay);
        
        if (target != null)
        {
            GameObject basicAttack = Instantiate(alchemistBasicAttackPrefab);
            basicAttack.transform.position = target.transform.position;
            basicAttack.transform.localScale *= 2 * damageRadius;

            // Deal damage
            float damage = thisFighter.GetBasicAttackDamage();
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(basicAttack.transform.position, damageRadius);
            foreach (Collider2D collider in hitColliders)
            {
                Fighter tmp = collider.gameObject.GetComponent<Fighter>();
                GenericMeleeMonster tmp2 = collider.gameObject.GetComponent<GenericMeleeMonster>();
                GenericRangedMonster tmp3 = collider.gameObject.GetComponent<GenericRangedMonster>();

                if (tmp != null)
                {
                    if (tmp.team == CombatInfo.Team.Enemy)
                    {
                        tmp.TakeDamage(damage);
                    }
                }

                if (tmp2 != null)
                {
                    tmp2.TakeDamage(damage);
                }

                if (tmp3 != null)
                {
                    tmp3.TakeDamage(damage);
                }
            }

            // Gain mana
            thisFighter.GainMana(10);
        }
        yield break;
    }
}
