using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageBasicAttackProjectile : MonoBehaviour
{
    public Fighter sourceFighter;

    public void SetSource(Fighter sourceFighter)
    {
        this.sourceFighter = sourceFighter;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject target = GetComponent<ProjectileMovement>().targetObject;

        if (other.gameObject == target)
        {
            // Deal damage
            Attackable attackable = target.GetComponent<Attackable>();

            if(attackable != null)
            {
                attackable.TakeDamage(sourceFighter.GetBasicAttackDamage());
                sourceFighter.GainMana(10);
            }

            Destroy(gameObject);
        }
    }
}
