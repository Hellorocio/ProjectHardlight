using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageBasicAttackProjectile : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject target = GetComponent<ProjectileMovement>().target;

        if (other.gameObject == target)
        {
            Fighter sourceFighter = GetComponent<ProjectileMovement>().source.GetComponent<Fighter>();

            // Deal damage
            float damage = sourceFighter.basicAttackStats.damage + sourceFighter.basicAttackStats.damage * sourceFighter.attackBoost;
            target.GetComponent<Fighter>().TakeDamage(damage);

            // Gain mana
            sourceFighter.GainMana(10);

            Destroy(gameObject);
        }
    }
}
