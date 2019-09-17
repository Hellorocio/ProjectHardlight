using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.Events;

public class Fighter : MonoBehaviour
{
    public string characterName = "defaultCharacterName";
    public CombatInfo.Team team;

    public GameObject healthUI;
    public GameObject manaUI;
    public GameObject maxManaUI;
    public GameObject selectedUI;
    public GameObject maxManaGlow;
    public Animator anim;
    public FighterStats fighterStats;
    public SoulStats soul;
    public bool healer;

    public MonoBehaviour basicAttackAction;

    private float health;
    private float speed;
    private int mana;

    private GameObject attackParent;

    // Basic attacking
    public BasicAttackStats basicAttackStats;
    public GameObject currentTarget;
    private IEnumerator basicAttackLoop;
    // This makes it so there's not weird jittering on the edge of your attack range
    private static float attackRangeAllowance = 0.1f;
    
    // Move
    private IEnumerator moveLoop;

    //Buff list
    List<BuffObj> buffs;
    private IEnumerator buffLoop;
    private float movementSpeedBoost;
    private float attackSpeedBoost;
    private float defenseBoost;
    public float attackBoost;
    private float manaGenerationBoost;


    //have max mana event
    public delegate void MaxManaReached();
    public event MaxManaReached OnMaxMana;

    //switch target event
    public delegate void SwitchTarget();
    public event SwitchTarget OnSwitchTarget;

    //called on death
    public delegate void Death();
    public event Death OnDeath;

    // Start is called before the first frame update
    void Start()
    {
        InitBoosts();

        //initialize temp stats
        //get max health based on soul boost if there is one
        if (soul != null)
        {
            health = fighterStats.maxHealth + fighterStats.maxHealth * soul.healthBoost;
        }
        else
        {
            health = fighterStats.maxHealth;
        }
        speed = fighterStats.movementSpeed;
        mana = 0;

        if (team == CombatInfo.Team.Hero)
        {
            attackParent = GameObject.Find("Enemies");
        }
        else
        {
            attackParent = GameObject.Find("Players");
        }

        buffs = new List<BuffObj>();
        
        SetHealthUI();
        SetManaUI();
        SetMaxManaUI();
        SetCurrentTarget();
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
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Basic AI
        // TODO(Don't stop movement if player issued the Move command)

        // You have a target to go for
        if (currentTarget != null && currentTarget.activeSelf)
        {
            float distanceFromTarget = Vector2.Distance(transform.position, currentTarget.transform.position);

            // In range of target
            if (distanceFromTarget < basicAttackStats.range)
            {

                // Stop moving
                if (moveLoop != null)
                {
                    StopCoroutine(moveLoop);
                    moveLoop = null;
                }

                // Start basic attacking
                if (basicAttackLoop == null)
                {
                    basicAttackLoop = BasicAttackLoop();
                    StartCoroutine(basicAttackLoop);
                }
            }
            else if (distanceFromTarget > basicAttackStats.range + attackRangeAllowance)
            {
                // Out of range

                // Move towards target
                if (moveLoop == null)
                {
                    moveLoop = MoveLoop();
                    StartCoroutine(moveLoop);
                }

                // Stop basic attacking
                if (basicAttackLoop != null)
                {
                    StopCoroutine(basicAttackLoop);
                    basicAttackLoop = null;
                }
            }

        }
        else
        {
            // No target to go for, get one
            SetCurrentTarget();
            if (currentTarget == null)
            {
                // Stop basic attacking
                if (basicAttackLoop != null)
                {
                    StopCoroutine(basicAttackLoop);
                    basicAttackLoop = null;
                }
            }
        }
    }
    
    /// <summary>
    /// Unit moves towards its current target with speed based on stats
    /// </summary>
    /// <returns></returns>
    IEnumerator MoveLoop()
    {
        while (true)
        {
            float currentSpeed = speed + (speed * movementSpeedBoost);
            transform.position = Vector3.MoveTowards(transform.position, currentTarget.transform.position, currentSpeed * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator BasicAttackLoop()
    {
        while (currentTarget != null && currentTarget.activeSelf)
        {
            BasicAttackAction attack = (BasicAttackAction)basicAttackAction;
            attack.DoBasicAttack();
            if (anim != null)
            {
                anim.Play("Attack");
            }

            AudioSource audioSource = gameObject.GetComponent<AudioSource>();
            if (audioSource != null && basicAttackStats.sfx != null)
            {
                audioSource.clip = basicAttackStats.sfx;
                audioSource.Play();
            }
            yield return new WaitForSeconds(basicAttackStats.attackSpeed + basicAttackStats.attackSpeed * attackSpeedBoost);

            if (healer)
            {
                //SetCurrentTarget(); // This may be overriding the commandUI's set new target
            }
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
    /// Called by other fighters when they attack this one
    /// </summary>
    /// <param name="dmg"></param>
    public void TakeDamage (float dmg)
    {
        health -= dmg - dmg * defenseBoost;

        if (health <= 0)
        {
            gameObject.SetActive(false);
            GameObject.FindGameObjectWithTag("GameController").GetComponent<LevelManager>().checkFighters();
            
            if (OnDeath != null)
            {
                OnDeath();
            }
        }

        SetHealthUI();
    }

    public void Heal (float amt)
    {
        health += amt;
        if (health >= fighterStats.maxHealth)
        {
            health = fighterStats.maxHealth;
        }
        SetHealthUI();
    }

    /// <summary>
    /// Adds mana and triggers max mana event if unit has reached max mana
    /// </summary>
    /// <param name="manaGained"></param>
    public void GainMana(int manaGained)
    {
        int prevMana = mana;

        mana += (int) (manaGained + manaGained * manaGenerationBoost);
        mana = Mathf.Clamp(mana, 0, fighterStats.maxMana);

        if (prevMana != mana && mana == fighterStats.maxMana)
        {
            Debug.Log("READY TO CAST SPELLS!");
            if (OnMaxMana != null)
            {
                //invoke event
                OnMaxMana();
            }
            
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
        mana = Mathf.Clamp(mana, 0, fighterStats.maxMana);

        if (maxManaGlow != null)
        {
            maxManaGlow.SetActive(false);
        }

        SetManaUI();
    }

    public int GetCurrentMana()
    {
        return mana;
    }

    /// <summary>
    /// Searches enemies or players gameObject for the closest thing to attack and sets currentTarget
    /// Sets currentTarget to null if there are no more things to attack
    /// </summary>
    void SetCurrentTarget()
    {
        if (!healer)
        {
            SetClosestAttackTarget();
        }
        else
        {
            SetOptimalHealingTarget();
        }

        if (OnSwitchTarget != null)
        {
            OnSwitchTarget();
        }
    }

    /// <summary>
    /// Finds the closest enemy and sets current target
    /// </summary>
    void SetClosestAttackTarget ()
    {
        Fighter[] currentTargets = attackParent.GetComponentsInChildren<Fighter>();
        float minDist = 1000f;
        GameObject tempcurrentTarget = null;

        for (int i = 0; i < currentTargets.Length; i++)
        {
            if (currentTargets[i].gameObject.activeSelf)
            {
                float dist = (transform.position - currentTargets[i].transform.position).sqrMagnitude;
                if (dist < minDist)
                {
                    minDist = dist;
                    tempcurrentTarget = currentTargets[i].gameObject;
                }
            }
        }
        currentTarget = tempcurrentTarget;
    }
    /// <summary>
    /// Finds the lowest health friendly in range and sets them as target
    /// </summary>
    void SetOptimalHealingTarget ()
    {
        Fighter[] currentTargets = transform.parent.GetComponentsInChildren<Fighter>();
        float maxHealth = 1000f;
        GameObject tempcurrentTarget = null;

        for (int i = 0; i < currentTargets.Length; i++)
        {
            if (currentTargets[i].gameObject.activeSelf)
            {
                float checkHealth = currentTargets[i].health;
                if (checkHealth < maxHealth)
                {
                    maxHealth = checkHealth;
                    tempcurrentTarget = currentTargets[i].gameObject;
                }
            }
        }
        currentTarget = tempcurrentTarget;
    }

    /// <summary>
    /// Sets the attack target
    /// </summary>
    public void SetIssuedCurrentTarget(Fighter target)
    {
        if(target != null && ((target.team == CombatInfo.Team.Enemy && !healer)
                || (target.team == CombatInfo.Team.Hero && healer)))
        {
            //Updates the current target
            currentTarget = target.gameObject;

            if (OnSwitchTarget != null)
            {
                OnSwitchTarget();
            }
        }
        
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
        manaUI.GetComponent<Text>().text = mana.ToString();
    }

    void SetMaxManaUI()
    {
        if (maxManaUI != null)
        {
            maxManaUI.GetComponent<Text>().text = "/" + fighterStats.maxMana.ToString();
        }
    }

    public void SetSelectedUI(bool active)
    {
        selectedUI.SetActive(active);
    }

    public float GetHealth ()
    {
        return health;
    }
}
