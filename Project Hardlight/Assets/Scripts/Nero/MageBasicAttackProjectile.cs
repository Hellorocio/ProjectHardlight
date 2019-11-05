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

            //Sorry for the gross code, this will be refactored after this playtest!!!
            Fighter tmp = target.GetComponent<Fighter>();
            GenericMonsterAI monster = target.GetComponent<GenericMonsterAI>();
           

            if(tmp != null)
            {
                tmp.TakeDamage(sourceFighter.GetBasicAttackDamage());
                sourceFighter.GainMana(10);
            }

            if(monster != null)
            {
                monster.TakeDamage(sourceFighter.GetBasicAttackDamage());
                sourceFighter.GainMana(10);
            }

            

            Destroy(gameObject);
        }
    }
}
