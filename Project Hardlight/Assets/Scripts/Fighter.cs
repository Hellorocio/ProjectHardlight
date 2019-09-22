using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.Events;

/// <summary>
/// This class just stores stat information about the fighter, and has accessors for all these stats
/// </summary>
public class Fighter : MonoBehaviour
{
    public string characterName = "defaultCharacterName";
    public CombatInfo.Team team;
    
    public GameObject healthUI;
    public GameObject manaUI;
    public GameObject selectedUI;
    public GameObject maxManaGlow;
    public Animator anim;

    public float maxHealth = 100;
    public float maxMana = 30;
    public float speed = 1;
    public SoulStats soul;

    private Color defaultColor;
    private Color hitColor;
    
    private float health;
    private float mana = 0;

    [HideInInspector]
    public BattleManager battleManager;
    
    //Buff list
    List<BuffObj> buffs;
    private IEnumerator buffLoop;
    private float movementSpeedBoost;
    private float attackSpeedBoost;
    private float defenseBoost;
    private float attackBoost;
    private float manaGenerationBoost;

    //have max mana event
    public delegate void MaxManaReached();
    public event MaxManaReached OnMaxMana;

    // Start is called before the first frame update
    void Start()
    {
        InitBoosts();

        defaultColor = gameObject.GetComponentInChildren<SpriteRenderer>().color;
        hitColor = new Color(1f, .5235f, .6194f);
        
        health = maxHealth;        

        GameObject battleManagerObj = GameObject.Find("BattleManager");
        if (battleManagerObj != null)
        {
            battleManager = battleManagerObj.GetComponent<BattleManager>();
        }

        buffs = new List<BuffObj>();
        
        SetHealthUI();
        SetManaUI();
    }

    /// <summary>
    /// Initializes boost amounts based on soul boosts
    /// </summary>
    void InitBoosts ()
    {
        if (soul != null)
        {
            movementSpeedBoost += soul.movementSpeedBoost;
            attackSpeedBoost += soul.attackSpeedBoost;
            defenseBoost += soul.defenseBoost;
            attackBoost += soul.attackBoost;
            manaGenerationBoost += soul.manaGenerationBoost;

            maxHealth = maxHealth + maxHealth * soul.healthBoost;
        }
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
    /// Returns this fighter's attack speed based on buffs
    /// </summary>
    /// <returns></returns>
    public float GetAttackSpeed(float attackSpeed)
    {
        return attackSpeed + attackSpeed * attackSpeedBoost;
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
    /// Returns this fighter's damage based on buffs
    /// </summary>
    /// <returns></returns>
    public float GetDamage(float dmg)
    {
        return dmg + dmg * attackBoost;
    }

    /// <summary>
    /// Returns this fighter's damage based on buffs and basicAttackStats
    /// </summary>
    /// <returns></returns>
    public float GetBasicAttackDamage ()
    {
        return GetDamage(GetComponent<FighterAttack>().basicAttackStats.damage);
    }

    /// <summary>
    /// Returns this fighter's mana regeneration based on buffs and soul stat
    /// </summary>
    /// <returns></returns>
    public float GetManaGeneration (float manaGained)
    {
        return manaGained + manaGained * manaGenerationBoost;
    }

    /// <summary>
    /// Called by other fighters when they attack this one
    /// </summary>
    /// <param name="dmg"></param>
    public void TakeDamage (float dmg)
    {
        health -= dmg - dmg * defenseBoost;
        IEnumerator colorThing = HitColorChanger();
        StartCoroutine(colorThing);
        if (health <= 0)
        {
            gameObject.SetActive(false);

            //tell battleManager this fighter died so it can keep track of level completion info
            battleManager.OnDeath(this);
        }

        SetHealthUI();
    }

    IEnumerator HitColorChanger()
    {
        gameObject.GetComponentInChildren<SpriteRenderer>().color = hitColor;
        yield return new WaitForSeconds((float)0.25);
        gameObject.GetComponentInChildren<SpriteRenderer>().color = defaultColor;
    }

    public void Heal (float amt)
    {
        health += amt;
        if (health >= maxHealth)
        {
            health = maxHealth;
        }
        SetHealthUI();
    }

    /// <summary>
    /// Adds mana and triggers max mana event if unit has reached max mana
    /// </summary>
    /// <param name="manaGained"></param>
    public void GainMana(int manaGained)
    {
        float prevMana = mana;

        mana += GetManaGeneration(manaGained);
        mana = Mathf.Clamp(mana, 0, maxMana);

        if (prevMana != mana && mana == maxMana)
        {
            Debug.Log("READY TO CAST SPELLS!");

            //invoke onmaxmana event
            OnMaxMana?.Invoke();

            if (maxManaGlow != null)
            {
                maxManaGlow.SetActive(true);
            }
        }

        SetManaUI();
    }

    public void LoseMana (int manaLost)
    {
        mana -= manaLost;
        mana = Mathf.Clamp(mana, 0, maxMana);

        if (maxManaGlow != null)
        {
            maxManaGlow.SetActive(false);
        }

        SetManaUI();
    }

    public float GetHealth()
    {
        return health;
    }

    public float GetCurrentMana()
    {
        return mana;
    }

    /// <summary>
    /// Right now this sets the HPText, probably change later to a health bar
    /// </summary>
    void SetHealthUI()
    {
        healthUI.GetComponent<Text>().text = ((int)health).ToString();
    }

    void SetManaUI()
    {
        if (manaUI != null)
        {
            manaUI.GetComponent<Text>().text = ((int)mana).ToString() + "/" + maxMana.ToString();
        }
    }

    public void SetSelectedUI(bool active)
    {
        selectedUI.SetActive(active);
    }
}
