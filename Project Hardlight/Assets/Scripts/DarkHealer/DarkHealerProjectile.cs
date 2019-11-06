using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkHealerProjectile : MonoBehaviour
{
    
    public float dmg = 0;
    GameObject target;

    private void Start()
    {
        //target = GetComponent<ProjectileMovement>().target;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Attackable hitAttackable = other.GetComponent<Attackable>();

        if (hitAttackable != null)
        {
            if (hitAttackable.team == CombatInfo.Team.Hero)
            {
                //heal
                hitAttackable.Heal(dmg);
            }
            else
            {
                // Deal damage
                hitAttackable.TakeDamage(dmg);
            }
            
        }

        if (other.gameObject == target)
        {
            Destroy(gameObject);
        }
    }
}
