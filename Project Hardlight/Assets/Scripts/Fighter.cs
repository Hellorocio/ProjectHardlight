using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.Events;

/// <summary>
/// Just stores stat information about a Fighter (Vessel or Enemy) and has accessors for all these stats, seriously
/// See FighterAttack for a Fighter's basic attack implementation
/// See FighterMove for a Fighter's movement implementation
/// </summary>
public class Fighter : MonoBehaviour
{
    public GameObject selectedUI;
    public GameObject maxManaGlow;
    public Animator anim;
    public Attackable attackable;

    private int maxMana;
    private float speed = 1;
    public Soul soul;

    private int mana = 0;

    public AudioClip manaFullSfx;
    
    // Set by soul
    public int sunlight = 0;
    public int moonlight = 0;
    public int starlight = 0;

    [HideInInspector]
    public BattleManager battleManager;

    //have max mana event
    public delegate void MaxManaReached(Fighter fighter);
    public event MaxManaReached OnMaxMana;
    
    //lose mana event (only used by fighterUseAbilityPopup right now)
    public delegate void FighterLoseMana(Fighter fighter);
    public event FighterLoseMana OnLoseMana;

    //mana changed event- used by ManaBar
    public delegate void ManaChanged(float mana);
    public event ManaChanged OnManaChanged;


    // Start is called before the first frame update
    void Start()
    {
        maxMana = GetMaxMana();
        speed = GetMovementSpeed();
        attackable = GetComponent<Attackable>();

        GameObject battleManagerObj = GameObject.Find("BattleManager");
        if (battleManagerObj != null)
        {
            battleManager = battleManagerObj.GetComponent<BattleManager>();
        }
    }

    private void OnEnable()
    {
        if (soul != null)
        {
            sunlight = soul.GetAllightValue(AllightType.SUNLIGHT);
            moonlight = soul.GetAllightValue(AllightType.MOONLIGHT);
            starlight = soul.GetAllightValue(AllightType.STARLIGHT);
        }
    }

    /// <summary>
    /// Returns this fighter's attack speed based on buffs and souls
    /// </summary>
    /// <returns></returns>
    public float GetAttackSpeed(float attackSpeed)
    {
        float soulBoost = 0;
        if (soul != null)
        {
            soulBoost = soul.GetAttackSpeedBonus(attackSpeed);
        }
        return attackSpeed + attackSpeed * attackable.percentAttackSpeedModifier + soulBoost;
    }

    /// <summary>
    /// Returns this fighter's speed based on base stat, soul, and buffs
    /// </summary>
    /// <returns></returns>
    public float GetSpeed()
    {
        return speed + speed * attackable.percentMovementSpeedModifier;
    }

    /// <summary>
    /// Returns this fighter's ability based on buffs (used for abilities)
    /// </summary>
    /// <returns></returns>
    public float GetAbility()
    {
        int baseAbility = GetComponent<VesselData>().baseAbility;
        int soulBoost = 0;
        if (soul != null)
        {
            soulBoost = soul.GetAbilityBonus((int)baseAbility);
        }
        return baseAbility * (1.0f + GetComponent<Attackable>().percentAbilityModifier) + soulBoost;
    }

    /// <summary>
    /// Returns this fighter's damage based on buffs and basicAttackStats
    /// </summary>
    /// <returns></returns>
    public float GetBasicAttackDamage ()
    {
        float dmg = GetComponent<VesselData>().basicAttack.damage;
        int soulBoost = 0;
        if (soul != null)
        {
            soulBoost = soul.GetAttackBonus((int)dmg);
        }
        
        return (dmg * (1.0f + attackable.percentAttackDamageModifier)) + soulBoost;
    }

    /// <summary>
    /// Returns this fighter's mana regeneration based on buffs and soul stat
    /// </summary>
    /// <returns></returns>
    public int GetManaGeneration (int manaGained)
    {
        return (int) (manaGained);
    }


    public void SetMaxMana ()
    {
        GainMana(maxMana);
    }

    /// <summary>
    /// Adds mana and triggers max mana event if unit has reached max mana
    /// </summary>
    /// <param name="manaGained"></param>
    public void GainMana(int manaGained)
    {
        if (TutorialManager.Instance.tutorialEnabled && !TutorialManager.Instance.startGeneratingMana)
        {
            return;
        }

        int prevMana = mana;

        mana += GetManaGeneration(manaGained);
        mana = Mathf.Clamp(mana, 0, maxMana);

        if (prevMana != mana && mana == maxMana)
        {
            //Debug.Log("READY TO CAST SPELLS!");

            //invoke onmaxmana event
            OnMaxMana?.Invoke(this);
            
            if (TutorialManager.Instance.tutorialEnabled && !TutorialManager.Instance.usedAbility)
            {
                GameManager.Instance.SayTop("When a hero hits max mana, you can click it to cast one of its abilities.", 10);
            }

            if (maxManaGlow != null)
            {
                maxManaGlow.SetActive(true);
            }

            if (GetComponent<AudioSource>() != null &&  manaFullSfx)
            {
                GetComponent<AudioSource>().clip = manaFullSfx;
                GetComponent<AudioSource>().Play();
            }
        }

        //mana changed event
        OnManaChanged?.Invoke(mana);
    }

    public void LoseMana (int manaLost)
    {
        mana -= manaLost;
        mana = Mathf.Clamp(mana, 0, maxMana);

        if (maxManaGlow != null)
        {
            maxManaGlow.SetActive(false);
        }

        //lose mana event
        OnLoseMana?.Invoke(this);

        //mana changed event
        OnManaChanged?.Invoke(mana);
    }

    public float GetCurrentMana()
    {
        return mana;
    }

    public void SetSelectedUI(bool active)
    {
        //selectedUI.SetActive(active);

        //start switching over to highlighted outline
        if (GetComponent<HighlightFighter>() != null)
        {
            GetComponent<HighlightFighter>().highlight = active;
        }
    }
    
    public int GetMaxMana ()
    {
        int baseMana = GetComponent<VesselData>().baseMana;
        return baseMana;
    }

    public int GetMovementSpeed()
    {
        int baseSpeed = GetComponent<VesselData>().baseMovementSpeed / 100;

        //if(baseSpeed <= 0)
        //{
            
        //    return 1;
        //}
        return baseSpeed;
    }

    
}
