using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericRangedMonsterProjectile : MonoBehaviour
{
    public float damage;
    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject target = GetComponent<ProjectileMovement>().targetObject;

        
        if (other.gameObject == target)
        {
            //Fighter sourceFighter = GetComponent<ProjectileMovement>().source.GetComponent<Fighter>();

            // Deal damage
            //float damage = sourceFighter.GetBasicAttackDamage();
            target.GetComponent<Fighter>().TakeDamage(damage);

            // Gain mana
            //sourceFighter.GainMana(10);

            Destroy(gameObject);
        }
        
    }
}
