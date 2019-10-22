using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkWaveAbility : Ability
{
    public GameObject projectilePrefab;

    public float baseWidth = 1;
    
    public float baseEffectRange;

    public GameObject rangeIndicatorPrefab;
    public GameObject widthIndicatorPrefab;

    public GameObject rangeIndicator;
    public GameObject widthIndicator;

    private bool targeting;
    
    public void Update()
    {
        if (targeting)
        {
            // Update positions of range and indicator
            rangeIndicator.transform.position = gameObject.transform.position;
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            widthIndicator.transform.position = new Vector3(mousePos.x, mousePos.y, widthIndicator.transform.position.z);
            
            // Update rotation of indicator
            Vector3 dir = mousePos - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            widthIndicator.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    public override bool StartTargeting()
    {
        targeting = true;

        rangeIndicator = Instantiate(rangeIndicatorPrefab);
        rangeIndicator.name = "Range";
        rangeIndicator.transform.localScale *= 2 * GetRange();

        widthIndicator = Instantiate(widthIndicatorPrefab);
        widthIndicator.name = "Width";
        widthIndicator.transform.localScale *= 2 * GetRadius();

        return true;
    }

    public override void StopTargeting()
    {
        targeting = false;

        Destroy(rangeIndicator);
        Destroy(widthIndicator);
    }

    public override bool DoAbility()
    {
        // Check that selectedPosition (set by BM) is in range
        if (Vector2.Distance(selectedPosition, gameObject.transform.position) < GetRange())
        {
            Debug.Log("Dark Wave casted");

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
            }

            // Display
            GameObject wind = Instantiate(projectilePrefab);
            // Set width
            wind.transform.localScale *= 2 * GetRadius();
            // Set initial pos to caster (Merrha)
            wind.transform.position = transform.position;
            // Set projectile movement
            ProjectileMovement projectile = wind.GetComponent<ProjectileMovement>();
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            projectile.SetTarget(mousePos);
            projectile.StartMovement();
            
            return true;
        }
        else
        {
            Debug.Log("Dark Wave out of range");
            return false;
        }
    }

    public float GetRange()
    {
        return baseEffectRange;
    }

    public float GetRadius()
    {
        return baseWidth;
    }
}
