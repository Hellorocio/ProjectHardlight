using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageAoEBlast : Ability
{
    public float baseEffectRange;
    public float baseEffectRadius;

    public GameObject rangeIndicatorPrefab;
    public GameObject radiusIndicatorPrefab;

    public GameObject lightBlastPrefab;

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
        rangeIndicator.transform.localScale *=  GetRange();

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
            Debug.Log("AoE blast casted");

            // Hit enemies
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(selectedPosition, GetRadius());
            foreach (Collider2D collider in hitColliders)
            {
                Fighter hitFighter = collider.gameObject.GetComponent<Fighter>();
                if (hitFighter != null)
                {
                    if (hitFighter.team == CombatInfo.Team.Enemy)
                    {
                        hitFighter.TakeDamage(GetDamage());
                    }
                }

                GenericMeleeMonster tmp2 = collider.gameObject.GetComponent<GenericMeleeMonster>();
                GenericRangedMonster tmp3 = collider.gameObject.GetComponent<GenericRangedMonster>();

                if (tmp2 != null)
                {
                    tmp2.TakeDamage(GetDamage());
                }

                if (tmp3 != null)
                {
                    tmp3.TakeDamage(GetDamage());
                }
            }

            //display boom!
            GameObject boom = Instantiate(lightBlastPrefab);
            Vector3 boomPos = selectedPosition;
            boomPos.z = 2;
            boom.transform.position = boomPos;

            return true;
        }
        else
        {
            Debug.Log("AoE blast out of range");
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
