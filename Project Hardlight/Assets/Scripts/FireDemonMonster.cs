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
    public float moveSpeed;
    public float basicAttackDamage;
    public float attackSpeed;

    private float currentHealth;
    private float currentMana;

    private GameObject currentTarget;
    //private Grid monsterAIGrid;
    private GameObject attackParent;


    public enum MoveState {stopped, moving, patrolling, interrupted, basicAttacking, advancedAttacking}
    MoveState moveState = MoveState.stopped;
    public enum PatrolType {none, looping, reverse, random}

    public List<CombatInfo.TargetPreference> targetPrefs;

    public PatrolType patrolType;
    public List<Transform> patrolRoute;
    

    public BasicAttackStats basicAttackStats;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        currentMana = 0;
        //monsterAIGrid = GameObject.Find("MonsterAIGrid").GetComponent<Grid>();
        attackParent = GameObject.Find("Vessels");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        MoveToTarget();
        DecideAttack();
    }

    bool IsValidTarget(GameObject target)
    {
        return (target != null && target.activeSelf);
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
        return fighters;
    }

    void DecideAttack()
    {
        if (IsValidTarget(currentTarget))
        {
            if(moveState == MoveState.stopped)
            {
                StartBasicAttacking();
            }
        } else
        {
            //There is noone around to attack
            if(GetValidTargets().Count == 0)
            {

            } else
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
                
            transform.position = Vector3.MoveTowards(transform.position, currentTarget.transform.position, moveSpeed * Time.deltaTime);
            moveState = MoveState.moving;

        } else
        {
            moveState = MoveState.stopped;
        }
        
    }

    void StartBasicAttacking()
    {
        moveState = MoveState.basicAttacking;
        StartCoroutine(BasicAttack());
    }

   public void StopBasicAttacking()
    {
        StopCoroutine(BasicAttack());
        moveState = MoveState.stopped;
    }

    void DoBasicAttack(GameObject target)
    {
       
        target.GetComponent<Fighter>().TakeDamage(basicAttackDamage);

    }

    IEnumerator BasicAttack()
    {
        
        DoBasicAttack(currentTarget);

        AudioSource audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource != null && basicAttackStats.sfx != null)
        {
            audioSource.clip = basicAttackStats.sfx;
            audioSource.Play();
        }
        animator.Play("BasicAttack");

        Debug.Log("recorder stop time is : " + animator.recorderStopTime);
        yield return new WaitForSeconds(animator.recorderStopTime);
        moveState = MoveState.stopped;
    }


    /// <summary>
    /// Returns true if this fighter is in range of their currentTarget
    /// </summary>
    /// <returns></returns>
    public bool InBasicRangeOfTarget(Vector3 p)
    {
        return Vector2.Distance(transform.position, p) < basicAttackStats.range;
        
    }

    public bool InMaxAgroRange(Vector3 p)
    {
        return Vector2.Distance(transform.position, p) < maxAggroRange;
    }

    public bool InAlertedRange(Vector3 p)
    {
        return Vector2.Distance(transform.position, p) < alertedRange;
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
