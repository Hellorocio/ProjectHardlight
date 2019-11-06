﻿using System.Collections;
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

    private int maxMana;
    private float speed = 1;
    public Soul soul;

    private int mana = 0;

    public AudioClip manaFullSfx;

    [HideInInspector]
    public BattleManager battleManager;

    //Buff list
    // TODO cleanup and use above section's system instead (uses overridden Buff objects)
    List<BuffObj> buffs;
    private IEnumerator buffLoop;
    private float movementSpeedBoost;
    private float attackSpeedBoost;
    private float defenseBoost;
    private float attackBoost; //this is for basic attacks only
    private float abilityAttackBoost; //this is the attack boost for abilities
    private float manaGenerationBoost;

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

        GameObject battleManagerObj = GameObject.Find("BattleManager");
        if (battleManagerObj != null)
        {
            battleManager = battleManagerObj.GetComponent<BattleManager>();
        }

        buffs = new List<BuffObj>();
    }

    /// <summary>
    /// Adds a new buff to the list and implements its effect
    /// Starts buff timer if it isn't already running
    /// </summary>
    /// <param name="newBuff"></param>
    public void AddTimedBuff (BuffObj newBuff)
    {
        BuffObj cloneBuff = Instantiate(newBuff);
        buffs.Add(cloneBuff);

        movementSpeedBoost += newBuff.movementSpeedBoost;
        attackSpeedBoost += newBuff.attackSpeedBoost;
        defenseBoost += newBuff.defenseBoost;
        attackBoost += newBuff.attackBoost;
        manaGenerationBoost += newBuff.manaGenerationBoost;

        if (buffLoop == null)
        {
            buffLoop = UpdateBuff();
            StartCoroutine(buffLoop);
        }
    }
    
    IEnumerator UpdateBuff ()
    {
        while (buffs.Count > 0)
        {
            for (int i = 0; i < buffs.Count; i++)
            {
                buffs[i].timeActive--;
                if (buffs[i].timeActive <= 0)
                {
                    movementSpeedBoost -= buffs[i].movementSpeedBoost;
                    attackSpeedBoost -= buffs[i].attackSpeedBoost;
                    defenseBoost -= buffs[i].defenseBoost;
                    attackBoost -= buffs[i].attackBoost;
                    manaGenerationBoost -= buffs[i].manaGenerationBoost;

                    buffs.Remove(buffs[i]);
                    i--;
                }
            }
            yield return new WaitForSeconds(1);
        }
    }

    float GetAttackBoost ()
    {
        if (attackBoost < 0)
        {
            return 0;
        }
        return attackBoost;
    }

    /// <summary>
    /// Returns this fighter's attack speed based on buffs and souls
    /// </summary>
    /// <returns></returns>
    public float GetAttackSpeed(float attackSpeed)
    {
        int soulBoost = 0;
        if (soul != null)
        {
            soulBoost = soul.GetAttackSpeedBonus((int)attackSpeed);
        }
        return attackSpeed + attackSpeed * attackSpeedBoost + soulBoost;
    }

    /// <summary>
    /// Returns this fighter's speed based on base stat, soul, and buffs
    /// </summary>
    /// <returns></returns>
    public float GetSpeed()
    {
        return speed + speed * movementSpeedBoost;
    }

    /// <summary>
    /// Returns this fighter's damage for a based on buffs (used for abilities)
    /// </summary>
    /// <returns></returns>
    public float GetDamage(float dmg)
    {
        int soulBoost = 0;
        if (soul != null)
        {
            soulBoost = soul.GetAbilityBonus((int)dmg);
        }
        return dmg + dmg * abilityAttackBoost + soulBoost;
    }

    /// <summary>
    /// Returns this fighter's damage based on buffs and basicAttackStats
    /// </summary>
    /// <returns></returns>
    public float GetBasicAttackDamage ()
    {
        float dmg = GetComponent<FighterAttack>().attack.damage;
        int soulBoost = 0;
        if (soul != null)
        {
            soulBoost = soul.GetAttackBonus((int)dmg);
        }
        
        return dmg + dmg * GetAttackBoost() + soulBoost;
    }

    /// <summary>
    /// Returns this fighter's mana regeneration based on buffs and soul stat
    /// </summary>
    /// <returns></returns>
    public int GetManaGeneration (int manaGained)
    {
        return (int) (manaGained + manaGained * manaGenerationBoost);
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
        selectedUI.SetActive(active);

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
