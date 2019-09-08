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
            int damage = sourceFighter.basicAttackStats.damage;
            target.GetComponent<Fighter>().Attack(damage);

            // Gain mana
            sourceFighter.GainMana(10);

            Destroy(gameObject);
        }
    }
}
