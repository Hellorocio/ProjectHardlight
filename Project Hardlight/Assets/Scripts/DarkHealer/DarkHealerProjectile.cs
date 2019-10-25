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
        Fighter hitFighter = other.GetComponent<Fighter>();

        if (hitFighter != null)
        {
            if (hitFighter.team == CombatInfo.Team.Hero)
            {
                //heal
                hitFighter.Heal(dmg);
            }
            else
            {
                // Deal damage
                hitFighter.TakeDamage(dmg);
            }
            
        }

        MonsterAI monster = target.GetComponent<MonsterAI>();

        if (monster != null)
        {
            monster.TakeDamage(dmg);
        }

        if (other.gameObject == target)
        {
            Destroy(gameObject);
        }
    }
}
