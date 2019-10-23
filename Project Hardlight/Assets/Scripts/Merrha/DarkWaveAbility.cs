using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkWaveAbility : Ability
{
    public GameObject projectilePrefab;

    // Stats
    public float baseWidth = 1;
    public float baseEffectRange;
    public int damageAmount;
    public int healAmount;
        
    // Indicator prefabs
    public GameObject rangeIndicatorPrefab;
    public GameObject widthIndicatorPrefab;

    // Prefab instances
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
        rangeIndicator.transform.localScale *= GetRange();

        widthIndicator = Instantiate(widthIndicatorPrefab);
        widthIndicator.name = "Width";
        widthIndicator.transform.localScale *= GetRadius();

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
        if (Vector2.Distance(selectedPosition, transform.position) < GetRange())
        {
            // Display
            GameObject wind = Instantiate(projectilePrefab);
            // Set width
            wind.transform.localScale *= GetRadius();
            // Set initial pos to caster (Merrha)
            wind.transform.position = transform.position;
            // Set projectile movement
            ProjectileMovement projectile = wind.GetComponent<ProjectileMovement>();
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            projectile.SetTarget(mousePos);
            projectile.StartMovement();
            // Set projectile stats
            projectile.GetComponent<DarkWaveProjectile>().Initialize(transform.position, damageAmount, healAmount, GetRange());
            
            // Heal self
            GetComponent<Fighter>().Heal(healAmount);
            projectile.GetComponent<DarkWaveProjectile>().affectedFighters.Add(GetComponent<Fighter>());

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
