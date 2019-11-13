using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attackable : MonoBehaviour
{
    
    public CombatInfo.Team team;
    
    public BuffBar buffBar;
    
    [Header("Donut Touch")]
    // Stat modifiers (usually modified by buffs)
    // e.g. percentDamageTakenModifier = -.2 --> Take 20% less damage
    public float percentDamageTakenModifier;
    public float percentAttackDamageModifier;
    public float percentAttackSpeedModifier;
    public float percentAbilityModifier;

    public float percentMovementSpeedModifier;
    
    
    public int maxHealth;
    public float currentHealth;
    public Color defaultColor;

    //health changed event- used by HealthBar
    public delegate void HealthChanged(float health);
    public event HealthChanged OnHealthChanged;
    public event AttackableDeath OnAttackableDeath;

    //death event
    public delegate void AttackableDeath(Attackable attackable);

    void Start()
    {
        defaultColor = gameObject.GetComponentInChildren<SpriteRenderer>().color;
        maxHealth = GetMaxHealth();
        currentHealth = GetMaxHealth();
        percentDamageTakenModifier = 0.0f;
        percentAttackDamageModifier = 0.0f;
    }
    
    public void Heal (float amt)
    {
        currentHealth += amt;
        if (currentHealth >= maxHealth)
        {
            currentHealth = maxHealth;
        }

        //OnHealthChanged event
        OnHealthChanged?.Invoke(currentHealth);
    }
    
    /// <summary>
    /// Called by other fighters when they attack this one
    /// </summary>
    /// <param name="dmg"></param>
    public void TakeDamage (float dmg)
    {
        // Calculate based on modifiers
        float realDamage = dmg * (1.0f + percentDamageTakenModifier);
        //Debug.Log(realDamage);
        currentHealth -= realDamage;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        IEnumerator colorThing = HitColorChanger();
        StartCoroutine(colorThing);
        if (currentHealth <= 0)
        {
            if (team == CombatInfo.Team.Hero)
            {
                //remove moveLoc if following a move command
                if (GetComponent<FighterMove>().followingMoveOrder)
                {
                    GetComponent<FighterMove>().StopMovingCommandHandle();
                }
            }

            OnDeath();

            //death event
            OnAttackableDeath?.Invoke(this);
        }
        
        //OnHealthChanged event
        OnHealthChanged?.Invoke(currentHealth);
    }

    public void OnDeath()
    {
        // Tell Battle manager that this died
        if(gameObject.GetComponent<IndividualSwarmerAI>() == null)
        {
            BattleManager.Instance.OnDeath(this);
        }
        
        gameObject.SetActive(false);
    }

    public float GetHealth()
    {
        return currentHealth;
    }
    
    public int GetMaxHealth()
    {
        int baseHealth = 0;

        if (team == CombatInfo.Team.Hero)
        {
            Soul soul = GetComponent<Fighter>().soul;
            if (soul != null)
            {
                baseHealth = GetComponent<VesselData>().baseHealth;
                baseHealth += soul.GetHealthBonus(baseHealth);
            }
            else
            {
                baseHealth = GetComponent<VesselData>().baseHealth;
            }

        }
        else if (team == CombatInfo.Team.Enemy)
        {
            baseHealth = GetComponent<GenericMonsterAI>().maxHealth;
        }
        else
        {
            Debug.Log("Attackable doesn't have a Team set'");
        }
        
        
        return baseHealth;

    }


    IEnumerator HitColorChanger()
    {
        
        gameObject.GetComponentInChildren<SpriteRenderer>().color = BattleManager.Instance.hitColor;
        yield return new WaitForSeconds((float)0.25);
        gameObject.GetComponentInChildren<SpriteRenderer>().color = defaultColor;
    }

    
    public BuffInstance AddBuff(Buff buff)
    {
        BuffInstance buffInstance = gameObject.AddComponent(typeof(BuffInstance)) as BuffInstance;
        buffInstance.SetBuff(buff);
        buffInstance.StartBuff();
        
        // Add to buff bar
        Debug.Log(gameObject.name);
        buffBar.AddBuffInstance(buffInstance);

        return buffInstance;
    }
    
    // Called by the buff to tell you it's done
    public void RemoveBuff(BuffInstance buffInstance)
    {
        buffBar.RemoveBuffInstance(buffInstance);
        Destroy(buffInstance);
    }
}
