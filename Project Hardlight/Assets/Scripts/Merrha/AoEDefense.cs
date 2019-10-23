using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoEDefense : Ability
{
    public float baseEffectRange;
    public float baseEffectRadius;

    public Buff defenseBuff;
    
    public GameObject rangeIndicatorPrefab;
    public GameObject radiusIndicatorPrefab;

    public GameObject defenseSpellPrefab;

    public GameObject rangeIndicator;
    public GameObject radiusIndicator;

    private bool targeting;
    
    public void Update()
    {
        if (targeting)
        {
            // Update positions
            rangeIndicator.transform.position = gameObject.transform.position;
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            radiusIndicator.transform.position = new Vector3(mousePos.x, mousePos.y, radiusIndicator.transform.position.z);
        }
    }

    public override bool StartTargeting()
    {
        targeting = true;

        rangeIndicator = Instantiate(rangeIndicatorPrefab);
        rangeIndicator.name = "Range";
        rangeIndicator.transform.localScale *= GetRange();

        radiusIndicator = Instantiate(radiusIndicatorPrefab);
        radiusIndicator.name = "Radius";
        radiusIndicator.transform.localScale *= GetRadius();

        return true;
    }

    public override void StopTargeting()
    {
        targeting = false;

        Destroy(rangeIndicator);
        Destroy(radiusIndicator);
    }

    public override bool DoAbility()
    {
        // Check that selectedPosition (set by BM) is in range
        if (Vector2.Distance(selectedPosition, gameObject.transform.position) < GetRange())
        {
            
            // Cast spell appearance
            GameObject spellInstance = Instantiate(defenseSpellPrefab);
            spellInstance.transform.localScale *= GetRadius();
            spellInstance.transform.position = new Vector3(selectedPosition.x, selectedPosition.y, spellInstance.transform.position.z);
            
            // Hit allies in a circle to buff them
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(selectedPosition, GetRadius());
            foreach (Collider2D collider in hitColliders)
            {
                Fighter hitFighter = collider.gameObject.GetComponent<Fighter>();
                if (hitFighter != null)
                {
                    if (hitFighter.team == CombatInfo.Team.Hero)
                    {
                        // Give them the buff
                        hitFighter.AddBuff(defenseBuff);
                    }
                }
            }
            
            return true;
        }
        else
        {
            return false;
        }
    }

    public float GetRange()
    {
        return baseEffectRange;
    }

    public float GetRadius()
    {
        return baseEffectRadius;
    }
}
