using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkWaveAbility : Ability
{
    public GameObject projectilePrefab;

    // Stats
    [Header("Base Stats")]
    public float baseWidth = 1;
    public float healScale;
    
    [Header("Sunlight Augments")]
    // Increases healing and damage by numAffectedScale*sunlight per unit already affected by Dark Wind
    public float damageIncreaseScale;

    [Header("Moonlight Augments")]
    // Increased travel range and width
    public float rangeScale;

    public float widthScale;
    [Header("Starlight Augments")]
    public float healTrailScale;

    // Indicator prefabs
    [Header("Indicators")]
    public GameObject rangeIndicatorPrefab;
    public GameObject widthIndicatorPrefab;
    
    [Header("Donut touch")]
    // Prefab instances
    public GameObject rangeIndicator;
    public GameObject widthIndicator;

    public bool targeting;

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
        Augment();
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
            int damage = (int) (damageScale * GetComponent<Fighter>().GetAbility());
            int healing = (int) (healScale * GetComponent<Fighter>().GetAbility());
            int damageIncrease = (int) (sunlight*damageIncreaseScale);
            int healTrailAmt = (int) (moonlight*healTrailScale);
            Debug.Log("wtf" + healTrailAmt);
            projectile.GetComponent<DarkWaveProjectile>().Initialize(transform.position, damage, healing, GetRange(), damageIncrease, healTrailAmt);
            
            // Heal self
            GetComponent<Attackable>().Heal(healing);
            projectile.GetComponent<DarkWaveProjectile>().affectedAttackables.Add(GetComponent<Attackable>());

            if (gameObject.GetComponent<Fighter>().anim.HasState(0, Animator.StringToHash("Ability1")))
            {
                //Debug.Log("Ability1 anim is played");
                gameObject.GetComponent<Fighter>().anim.Play("Ability1");
            }

            return true;
        }
        else
        {
            Debug.Log("Dark Wave out of range");
            return false;
        }
    }

    public override float GetRange()
    {
        return baseEffectRange + starlight*rangeScale;
    }

    public float GetRadius()
    {
        return baseWidth + starlight*widthScale;
    }
}
