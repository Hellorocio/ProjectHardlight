using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.Events;

public class Fighter : MonoBehaviour
{
    public string characterName = "defaultCharacterName";
    public CombatInfo.Team team;
    public List<CombatInfo.TargetPreference> targetPrefs;
    public GameObject healthUI;
    public GameObject manaUI;
    public GameObject maxManaUI;
    public GameObject selectedUI;
    public GameObject maxManaGlow;
    public Animator anim;
    public FighterStats fighterStats;
    public SoulStats soul;
    public bool healer;

    private Color defaultColor;
    private Color hitColor;

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

        defaultColor = gameObject.GetComponentInChildren<SpriteRenderer>().color;
        hitColor = new Color(1f, .5235f, .6194f);
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
        DefaultAI();
    }

    private void DefaultAI()
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
        IEnumerator colorThing = HitColorChanger();
        StartCoroutine(colorThing);
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

    IEnumerator HitColorChanger()
    {
        gameObject.GetComponentInChildren<SpriteRenderer>().color = hitColor;
        yield return new WaitForSeconds((float)0.25);
        gameObject.GetComponentInChildren<SpriteRenderer>().color = defaultColor;
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
    /// Searches enemies or players gameObject for a target to attack and sets currentTarget
    /// Sets currentTarget to null if there are no more things to attack
    /// Will use targetprefs list if provided, otherwise will default to closest algorithm
    /// </summary>
    void SetCurrentTarget()
    {
        //This code chuck below checks if any enemies are active in the scene before calling a targeting function
        Fighter[] enemyListTMP = attackParent.GetComponentsInChildren<Fighter>();
        bool boolTMP = false;
        
        for (int i = 0; i < enemyListTMP.Length; i++)
        {
            if (enemyListTMP[i].gameObject.activeSelf)
            {
                boolTMP = true;
                i = enemyListTMP.Length;
            }
        }

        bool newTargetWasSelected = false;
        if (!healer && boolTMP)
        {
            //Default if no preferences exist
            if(targetPrefs.Count == 0)
            {
                SetClosestAttackTarget();
            } else
            {
                for (int i = 0; i < targetPrefs.Count; i++)
                {
                    if (targetPrefs[i] == CombatInfo.TargetPreference.Strongest)
                    {
                        SetStrongesttAttackTarget();
                        i = targetPrefs.Count;
                        newTargetWasSelected = true;
                    }
                    else if (targetPrefs[i] == CombatInfo.TargetPreference.Weakest)
                    {
                        SetWeakestAttackTarget();
                        i = targetPrefs.Count;
                        newTargetWasSelected = true;
                    }
                    else if (targetPrefs[i] == CombatInfo.TargetPreference.Closest)
                    {
                        SetClosestAttackTarget();
                        i = targetPrefs.Count;
                        newTargetWasSelected = true;
                    }
                    else if (targetPrefs[i] == CombatInfo.TargetPreference.Melee)
                    {
                        if (SetMeleeAttackTarget())
                        {
                            i = targetPrefs.Count;
                            newTargetWasSelected = true;
                        }
                    }
                    else if (targetPrefs[i] == CombatInfo.TargetPreference.Ranged)
                    {
                        if (SetRangedAttackTarget())
                        {
                            i = targetPrefs.Count;
                            newTargetWasSelected = true;
                        }
                    }
                    else if(targetPrefs[i] == CombatInfo.TargetPreference.Healer)
                    {
                        if (SetHealerAttackTarget())
                        {
                            i = targetPrefs.Count;
                            newTargetWasSelected = true;
                        }
                    }
                    
                }

                if (!newTargetWasSelected)
                {
                    SetClosestAttackTarget();
                }
            }

        }
        else if(healer)
        {
            SetOptimalHealingTarget();
        }

        if (OnSwitchTarget != null)
        {
            OnSwitchTarget();
        }
    }

    /// <summary>
    /// Finds the weakest enemy and sets current target
    /// </summary>
    void SetWeakestAttackTarget()
    {
        Fighter[] currentTargets = attackParent.GetComponentsInChildren<Fighter>();
        float hp = 100000000;
        int index = 0;

        for (int i = 0; i < currentTargets.Length; i++)
        {
            if (currentTargets[i].gameObject.activeSelf)
            {
                if(currentTargets[i].GetHealth() < hp)
                {
                    hp = currentTargets[i].GetHealth();
                    index = i;
                }
            }
        }
        currentTarget = currentTargets[index].gameObject;
    }

    /// <summary>
    /// Finds the strongest (Most HP) enemy and sets current target
    /// </summary>
    void SetStrongesttAttackTarget()
    {
        Fighter[] currentTargets = attackParent.GetComponentsInChildren<Fighter>();
        float hp = -1;
        int index = 0;

        for (int i = 0; i < currentTargets.Length; i++)
        {
            if (currentTargets[i].gameObject.activeSelf)
            {
                if (currentTargets[i].GetHealth() > hp)
                {
                    hp = currentTargets[i].GetHealth();
                    index = i;
                }
            }
        }
        currentTarget = currentTargets[index].gameObject;
    }

    /// <summary>
    /// Attempts to find a ranged hero to kill
    /// If one doesn't exist, use next preference
    /// </summary>
    bool SetRangedAttackTarget()
    {
        Fighter[] currentTargets = attackParent.GetComponentsInChildren<Fighter>();
        float minDist = 1000f;
        GameObject tempcurrentTarget = null;

        for (int i = 0; i < currentTargets.Length; i++)
        {
            if (currentTargets[i].gameObject.activeSelf)
            {
                float dist = (transform.position - currentTargets[i].transform.position).sqrMagnitude;
                if (dist < minDist && currentTargets[i].basicAttackStats.range > 3)
                {
                    minDist = dist;
                    tempcurrentTarget = currentTargets[i].gameObject;
                }
            }
        }
        if(tempcurrentTarget == null)
        {
            return false;
        }
        currentTarget = tempcurrentTarget;
        return true;
    }

    /// <summary>
    /// Attempts to find a melee hero to kill
    /// If one doesn't exist, use next preference
    /// </summary>
    bool SetMeleeAttackTarget()
    {
        Fighter[] currentTargets = attackParent.GetComponentsInChildren<Fighter>();
        float minDist = 1000f;
        GameObject tempcurrentTarget = null;

        for (int i = 0; i < currentTargets.Length; i++)
        {
            if (currentTargets[i].gameObject.activeSelf)
            {
                float dist = (transform.position - currentTargets[i].transform.position).sqrMagnitude;
                if (dist < minDist && currentTargets[i].basicAttackStats.range < 4)
                {
                    minDist = dist;
                    tempcurrentTarget = currentTargets[i].gameObject;
                }
            }
        }
        if (tempcurrentTarget == null)
        {
            return false;
        }
        currentTarget = tempcurrentTarget;
        return true;
    }

    /// <summary>
    /// Attempts to find a healer hero to kill
    /// If one doesn't exist, use next preference
    /// </summary>
    bool SetHealerAttackTarget()
    {
        Fighter[] currentTargets = attackParent.GetComponentsInChildren<Fighter>();
        float minDist = 1000f;
        GameObject tempcurrentTarget = null;

        for (int i = 0; i < currentTargets.Length; i++)
        {
            if (currentTargets[i].gameObject.activeSelf)
            {
                float dist = (transform.position - currentTargets[i].transform.position).sqrMagnitude;
                if (dist < minDist && currentTargets[i].healer)
                {
                    minDist = dist;
                    tempcurrentTarget = currentTargets[i].gameObject;
                }
            }
        }
        if (tempcurrentTarget == null)
        {
            return false;
        }
        currentTarget = tempcurrentTarget;
        return true;
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
