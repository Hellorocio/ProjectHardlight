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

    public void Initialize(float damage, Fighter source)
    {
        dmg = damage;
        this.source = source;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("proj hit");
        Fighter hitFighter = other.GetComponent<Fighter>();

        if (hitFighter != null)
        {
            if (hitFighter.team == CombatInfo.Team.Enemy)
            {
                // Deal damage
                hitFighter.TakeDamage(dmg);

                //heal ninja
                source.Heal(dmg);
                Destroy(gameObject);
            }

        }
        GenericMeleeMonster tmp2 = target.GetComponent<GenericMeleeMonster>();
        GenericRangedMonster tmp3 = target.GetComponent<GenericRangedMonster>();

        if (tmp2 != null)
        {
            tmp2.TakeDamage(dmg);

            //heal ninja
            source.Heal(dmg);
            Destroy(gameObject);
        }

        if (tmp3 != null)
        {
            tmp3.TakeDamage(dmg);

            //heal ninja
            source.Heal(dmg);
            Destroy(gameObject);
        }
    }
}
