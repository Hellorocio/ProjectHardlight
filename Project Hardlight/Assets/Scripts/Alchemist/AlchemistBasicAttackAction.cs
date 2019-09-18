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

    public void Start()
    {
        thisFighter = GetComponent<Fighter>();
    }

    public override void DoBasicAttack()
    {
        StartCoroutine("BasicAttackWithAnimationDelay");
    }

    IEnumerator BasicAttackWithAnimationDelay()
    {
        // TODO(mchi) scale animation delay to attack speed changes
        yield return new WaitForSeconds(animationDelay);

        GameObject target = thisFighter.currentTarget;
        if (target != null)
        {
            GameObject basicAttack = Instantiate(alchemistBasicAttackPrefab);
            basicAttack.transform.position = target.transform.position;
            basicAttack.transform.localScale *= 2 * damageRadius;

            // Deal damage
            float damage = thisFighter.basicAttackStats.damage + thisFighter.basicAttackStats.damage * thisFighter.attackBoost;
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(basicAttack.transform.position, damageRadius);
            foreach (Collider2D collider in hitColliders)
            {
                Fighter hitFighter = collider.gameObject.GetComponent<Fighter>();
                if (hitFighter != null)
                {
                    if (hitFighter.team == CombatInfo.Team.Enemy)
                    {
                        print("alchemist basic attack hit " + hitFighter.gameObject.name + " for damage = " + damage);
                        hitFighter.TakeDamage(damage);
                    }
                }
            }

            // Gain mana
            thisFighter.GainMana(10);
        }
        yield break;
    }
}
