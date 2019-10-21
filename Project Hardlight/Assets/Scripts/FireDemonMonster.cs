using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireDemonMonster : MonoBehaviour
{
    public string characterName = "FireDemon";
    public CombatInfo.Team team = CombatInfo.Team.Enemy;
    public Animator animator;
    public float maxHealth;
    public float maxMana;
    public float maxAggroRange;
    public float alertedRange;
    private Vector3 startPos;
    public float moveSpeed;
   // public float basicAttackDamage;
    public float basicAttackHitTime; //How long into the animation before the hit should be displayed to the player
    //public float attackSpeed;
    public AnimationClip basicAttackClip;
    private Color defaultColor;
    private Color hitColor;
    private float currentHealth;
    private float currentMana;


    private GameObject currentTarget;
    //private Grid monsterAIGrid;
    private GameObject attackParent;
    private bool anyValidTargets;


    public enum MoveState {stopped, moving, patrolling, interrupted, basicAttacking, advancedAttacking}
    MoveState moveState = MoveState.stopped;
    public enum PatrolType {none, looping, reverse, random}

    public List<CombatInfo.TargetPreference> targetPrefs;

    public PatrolType patrolType;
    public List<Transform> patrolRoute;
    private int patrolIndex = -1;
    

    public BasicAttackStats basicAttackStats;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        currentMana = 0;
        //monsterAIGrid = GameObject.Find("MonsterAIGrid").GetComponent<Grid>();
        attackParent = GameObject.Find("Vessels");
        defaultColor = gameObject.GetComponentInChildren<SpriteRenderer>().color;
        hitColor = new Color(1f, .5235f, .6194f);
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        UpdateTarget();
        if (anyValidTargets)
        {
            DecideAttack();
        } else
        {
            DoPatrol();
        }
        
    }

    bool IsValidTarget(GameObject target)
    {
        anyValidTargets = (target != null && target.activeSelf && InMaxAgroRange(target.transform.position));
        return anyValidTargets;
    }


    /// <summary>
    /// Returns a list of fighters that are non-null, active, and within this monster's aggro range
    /// </summary>
    /// <returns></returns>
    List<Fighter> GetValidTargets()
    {
        List<Fighter> fighters = new List<Fighter>();
        Fighter[] enemyListTMP = attackParent.GetComponentsInChildren<Fighter>();
        for(int i = 0; i < enemyListTMP.Length; i++)
        {
            if(IsValidTarget(enemyListTMP[i].gameObject) && InMaxAgroRange(enemyListTMP[i].transform.position))
            {
                fighters.Add(enemyListTMP[i]);
            }
        }
        anyValidTargets = fighters.Count > 0;
        return fighters;
    }


    /// <summary>
    /// Check the monster's current target and if it is not valid then check if there are any valid targets
    /// </summary>
    void UpdateTarget()
    {
        if (!IsValidTarget(currentTarget)) // Needs to also check if there are multiple enemies in alerted dist
        {
            startPos = transform.position;
            if(moveState == MoveState.moving)
            {
                moveState = MoveState.stopped;
            }
            SetCurrentTarget();
        }
    }

    void DecideAttack()
    {
        if (IsValidTarget(currentTarget))
        {
            if (!InBasicRangeOfTarget(currentTarget.transform.position))
            {
                if (moveState != MoveState.moving)
                {
                    animator.Play("Walk");

                }
                transform.position = Vector3.MoveTowards(transform.position, currentTarget.transform.position, moveSpeed / 100 * Time.deltaTime);
                moveState = MoveState.moving;


            }
            else
            {
                StartBasicAttacking();
            }
        }
        else
        {

            //There is noone around to attack
            if (GetValidTargets().Count == 0)
            {
                moveState = MoveState.stopped;


            }
            else
            {
                SetCurrentTarget();
            }
        }

        
    }

    void MoveToTarget()
    {

        //Vector3 tmp = monsterAIGrid.WorldToCell(currentTarget.transform.position);

        if (IsValidTarget(currentTarget) && !InBasicRangeOfTarget(currentTarget.transform.position))
        {
            if(moveState != MoveState.moving)
            {
                animator.Play("Walk");

            }   
            transform.position = Vector3.MoveTowards(transform.position, currentTarget.transform.position, moveSpeed/100 * Time.deltaTime);
            moveState = MoveState.moving;


        }
        else
        {
            if(moveState == MoveState.moving)
            {
                animator.Play("Idle");
            }
            moveState = MoveState.stopped;
        }
        
    }

    void MoveToPosition(Vector3 pos)
    {
        if (!InBodyRangeOfTarget(pos))
        {
            if (moveState != MoveState.patrolling)
            {
                animator.Play("Walk");

            }
            //Debug.Log("My loc = " + transform.position.ToString() + " | Pos loc = " + pos.ToString());
            transform.position = Vector3.MoveTowards(transform.position, pos, moveSpeed / 100 * Time.deltaTime);
            moveState = MoveState.patrolling;


        }
        else
        {
            if (moveState == MoveState.patrolling)
            {
                animator.Play("Idle");
            }
            moveState = MoveState.stopped;
        }
    }

    void DoPatrol()
    {
        if (patrolType != PatrolType.none && moveState == MoveState.stopped)
        {
            
            if (patrolType == PatrolType.looping)
            {
                ++patrolIndex;
                if(patrolIndex >= patrolRoute.Count)
                {
                    patrolIndex = 0;
                }
                MoveToPosition(patrolRoute[patrolIndex].position);
            }
            else if (patrolType == PatrolType.reverse)
            {

            }
            else if (patrolType == PatrolType.random)
            {

            }
        } else if(moveState == MoveState.patrolling)
        {
            MoveToPosition(patrolRoute[patrolIndex].position);
        }
    }

    void StartBasicAttacking()
    {
        if(moveState != MoveState.basicAttacking)
        {
            moveState = MoveState.basicAttacking;
            StartCoroutine(BasicAttack());
        }
        
    }

   public void StopBasicAttacking()
    {
        StopCoroutine(BasicAttack());
        moveState = MoveState.stopped;
    }

    void DoBasicAttack(GameObject target)
    {
       
        target.GetComponent<Fighter>().TakeDamage(basicAttackStats.damage);

    }

    IEnumerator BasicAttack()
    {
        
        animator.Play("BasicAttack");
        AudioSource audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource != null && basicAttackStats.sfx != null)
        {
            audioSource.clip = basicAttackStats.sfx;
            audioSource.Play();
        }
        else
        {
            Debug.Log("No valid audioSource or sfx for this enemy's attack!!");
        }
        yield return new WaitForSeconds(basicAttackHitTime);
        DoBasicAttack(currentTarget);

        
        Debug.Log("recorder stop time is : " + (basicAttackClip.length));
        //Debug.Assert(animator.recorderStopTime - basicAttackHitTime > 0, "basicAttackHitTime must be smaller than recorderStopTime");
        yield return new WaitForSeconds(basicAttackClip.length - basicAttackHitTime);
        moveState = MoveState.stopped;
    }


    /// <summary>
    /// Returns true if this fighter is in range of their currentTarget
    /// </summary>
    /// <returns></returns>
    public bool InBasicRangeOfTarget(Vector3 p)
    {
        //Debug.Log(Vector2.Distance(transform.position, p).ToString() + " " + basicAttackStats.range);
        return Vector2.Distance(transform.position, p) < basicAttackStats.range;
        
    }


    /// <summary>
    /// Returns true if the monster is within "arms-reach" of the target, to be used for detecting proximity
    /// to patrol nodes for both ranged and melee monsters
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public bool InBodyRangeOfTarget(Vector3 p)
    {
        //Debug.Log(Vector2.Distance(transform.position, p).ToString() + " " + basicAttackStats.range);
        return Vector2.Distance(transform.position, p) < 0.75;

    }

    public bool InMaxAgroRange(Vector3 p)
    {
        return Vector2.Distance(startPos, p) < maxAggroRange;
    }

    public bool InAlertedRange(Vector3 p)
    {
        return Vector2.Distance(transform.position, p) < alertedRange;
    }


    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        IEnumerator colorThing = HitColorChanger();
        StartCoroutine(colorThing);
        if (currentHealth <= 0)
        {
            //death
        }
    }

    public void OnDeath()
    {

        //Tell Battle manager that an enemy has died

        gameObject.SetActive(false);
    }

    IEnumerator HitColorChanger()
    {

        gameObject.GetComponentInChildren<SpriteRenderer>().color = hitColor;
        yield return new WaitForSeconds((float)0.25);
        gameObject.GetComponentInChildren<SpriteRenderer>().color = defaultColor;
    }

    /// <summary>
    /// Sets the current target, if there is a valid one within maxAggroRange. Uses the preference enums if provided to select a specific type of fighter
    /// </summary>
    public void SetCurrentTarget()
    {
        //This code chuck below checks if any enemies are active in the scene before calling a targeting function
        Fighter[] enemyListTMP = GetValidTargets().ToArray();
        bool enemiesActive = false;

        for (int i = 0; i < enemyListTMP.Length; i++)
        {
            if (enemyListTMP[i].gameObject.activeSelf)
            {
                enemiesActive = true;
                break;
            }
        }

        bool newTargetWasSelected = false;
        if (enemiesActive)
        {
            //Default if no preferences exist
            if (targetPrefs.Count == 0)
            {
                SetClosestAttackTarget();
            }
            else
            {
                for (int i = 0; i < targetPrefs.Count; i++)
                {
                    if (targetPrefs[i] == CombatInfo.TargetPreference.Strongest)
                    {
                        SetStrongesttAttackTarget();
                        newTargetWasSelected = true;
                        break;
                    }
                    else if (targetPrefs[i] == CombatInfo.TargetPreference.Weakest)
                    {
                        SetWeakestAttackTarget();
                        newTargetWasSelected = true;
                        break;
                    }
                    else if (targetPrefs[i] == CombatInfo.TargetPreference.Closest)
                    {
                        SetClosestAttackTarget();
                        newTargetWasSelected = true;
                        break;
                    }
                    else if (targetPrefs[i] == CombatInfo.TargetPreference.Melee)
                    {
                        if (SetMeleeAttackTarget())
                        {
                            newTargetWasSelected = true;
                            break;
                        }
                    }
                    else if (targetPrefs[i] == CombatInfo.TargetPreference.Ranged)
                    {
                        if (SetRangedAttackTarget())
                        {
                            newTargetWasSelected = true;
                            break;
                        }
                    }
                    else if (targetPrefs[i] == CombatInfo.TargetPreference.Healer)
                    {
                        if (SetHealerAttackTarget())
                        {
                            newTargetWasSelected = true;
                            break;
                        }
                    }
                    else if (targetPrefs[i] == CombatInfo.TargetPreference.WeakestTeamate)
                    {
                        SetOptimalHealingTarget();
                        newTargetWasSelected = true;
                        break;
                    }

                }

                if (!newTargetWasSelected)
                {
                    SetClosestAttackTarget();
                }
            }
        }


        //start moving toward target
        
    }

    void SetWeakestAttackTarget()
    {
        Fighter[] currentTargets = GetValidTargets().ToArray();
        float hp = float.MaxValue;
        int index = 0;

        for (int i = 0; i < currentTargets.Length; i++)
        {
            if (currentTargets[i].gameObject.activeSelf)
            {
                if (currentTargets[i].GetHealth() < hp)
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
        Fighter[] currentTargets = GetValidTargets().ToArray();
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
        Fighter[] currentTargets = GetValidTargets().ToArray();
        float minDist = float.MaxValue;
        GameObject tempcurrentTarget = null;

        for (int i = 0; i < currentTargets.Length; i++)
        {
            if (currentTargets[i].gameObject.activeSelf)
            {
                float dist = (transform.position - currentTargets[i].transform.position).sqrMagnitude;
                if (dist < minDist && currentTargets[i].GetComponent<FighterAttack>().basicAttackStats.range > 3)
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
    /// Attempts to find a melee hero to kill
    /// If one doesn't exist, use next preference
    /// </summary>
    bool SetMeleeAttackTarget()
    {
        Fighter[] currentTargets = GetValidTargets().ToArray();
        float minDist = float.MaxValue;
        GameObject tempcurrentTarget = null;

        for (int i = 0; i < currentTargets.Length; i++)
        {
            if (currentTargets[i].gameObject.activeSelf)
            {
                float dist = (transform.position - currentTargets[i].transform.position).sqrMagnitude;
                if (dist < minDist && currentTargets[i].GetComponent<FighterAttack>().basicAttackStats.range < 4)
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
    /// Attempts to find a healer hero to kill (healers always have a targetPref for weakestTeamate
    /// If one doesn't exist, use next preference
    /// </summary>
    bool SetHealerAttackTarget()
    {
        Fighter[] currentTargets = GetValidTargets().ToArray();
        float minDist = float.MaxValue;
        GameObject tempcurrentTarget = null;

        for (int i = 0; i < currentTargets.Length; i++)
        {
            if (currentTargets[i].gameObject.activeSelf)
            {
                float dist = (transform.position - currentTargets[i].transform.position).sqrMagnitude;
                if (dist < minDist && currentTargets[i].GetComponent<FighterAttack>().HasHealingBasicAttack())
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
    void SetClosestAttackTarget()
    {
        Fighter[] currentTargets = GetValidTargets().ToArray();
        float minDist = float.MaxValue;
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
    void SetOptimalHealingTarget()
    {
        Fighter[] currentTargets = GetValidTargets().ToArray();
        float maxHealth = float.MaxValue;
        GameObject tempcurrentTarget = null;

        for (int i = 0; i < currentTargets.Length; i++)
        {
            if (currentTargets[i].gameObject.activeSelf)
            {
                float checkHealth = currentTargets[i].GetHealth();
                if (checkHealth < maxHealth)
                {
                    maxHealth = checkHealth;
                    tempcurrentTarget = currentTargets[i].gameObject;
                }
            }
        }
        currentTarget = tempcurrentTarget;
    }
}
