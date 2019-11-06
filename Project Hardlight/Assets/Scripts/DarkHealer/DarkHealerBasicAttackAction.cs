using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkHealerBasicAttackAction : BasicAttackAction
{

    public GameObject DarkHealerBasicAttackPrefab;
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
            GameObject basicAttack = Instantiate(DarkHealerBasicAttackPrefab);
            basicAttack.transform.position = transform.position;
            basicAttack.transform.localScale *= 2 * damageRadius;

            // Deal damage
            float damage = thisFighter.GetBasicAttackDamage();
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(basicAttack.transform.position, damageRadius);
            foreach (Collider2D collider in hitColliders)
            {
                Attackable attackable = collider.gameObject.GetComponent<Attackable>();

                if (attackable != null)
                {
                    if (attackable.team == CombatInfo.Team.Enemy)
                    {
                        attackable.TakeDamage(damage);
                    }
                }
                
            }

            // Gain mana
            thisFighter.GainMana(10);
        }
        yield break;
    }
}
