using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaHealingProjectile : MonoBehaviour
{
    public float dmg = 0;
    GameObject target;
    Fighter source;

    private void Start()
    {
        /*
        target = GetComponent<ProjectileMovement>().target;
        source = GetComponent<ProjectileMovement>().source.GetComponent<Fighter>();
        */
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Fighter hitFighter = other.GetComponent<Fighter>();

        if (hitFighter != null)
        {
            if (hitFighter.team == CombatInfo.Team.Enemy)
            {
                // Deal damage
                hitFighter.TakeDamage(dmg);

                //heal ninja
                source.Heal(dmg);
            }

        }

        if (other.gameObject == target)
        {
            Destroy(gameObject);
        }
    }
}
