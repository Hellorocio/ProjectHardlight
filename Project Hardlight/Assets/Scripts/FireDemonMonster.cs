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
    public float maxAgroRange;
    public float alertedRange;
    public float moveSpeed;
    public float basicAttackDamage;
    public float attackSpeed;

    private float currentHealth;
    private float currentMana;

    private GameObject currentTarget;
    //private Grid monsterAIGrid;
    private GameObject attackParent;


    public enum MoveState {stopped, moving, interrupted, basicAttacking, advancedAttacking}
    MoveState moveState = MoveState.stopped;


    public List<CombatInfo.TargetPreference> targetPrefs;


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

    bool IsValidTarget()
    {
        return (currentTarget != null && currentTarget.activeSelf);
    }

    void DecideAttack()
    {
        if (IsValidTarget())
        {
            if(moveState == MoveState.stopped)
            {
                StartBasicAttacking();
            }
        } else
        {
            SetCurrentTarget();
        }
    }

    void MoveToTarget()
    {

        //Vector3 tmp = monsterAIGrid.WorldToCell(currentTarget.transform.position);

        if (IsValidTarget() && !InBasicRangeOfTarget(currentTarget.transform.position))
        {
                
            transform.position = Vector3.MoveTowards(transform.position, currentTarget.transform.position, moveSpeed * Time.deltaTime);
            moveState = MoveState.moving;
        } else
        {
            moveState = MoveState.stopped;
        }
        
    }

    public void StartBasicAttacking()
    {
        StartCoroutine(BasicAttack());
    }

    public void StopBasicAttacking()
    {
        StopCoroutine(BasicAttack());
    }

    public void DoBasicAttack(GameObject target)
    {
       
        target.GetComponent<Fighter>().TakeDamage(basicAttackDamage);

    }

    IEnumerator BasicAttack()
    {
        moveState = MoveState.basicAttacking;
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

    //IEnumerator BasicAttackLoop()
    //{
    //    while (currentTarget != null && currentTarget.activeSelf)
    //    {
    //        //check we are still in range
    //        if (!InBasicRangeOfTarget(currentTarget.transform.position))
    //        {
    //            StopBasicAttacking();
    //            break;
    //        }

    //        //attack
    //        DoBasicAttack(currentTarget);
    //        //&& (fighter.anim.GetCurrentAnimatorStateInfo(0).shortNameHash != Animator.StringToHash("Ability1")) ||
    //        //fighter.anim.GetCurrentAnimatorStateInfo(0).shortNameHash != Animator.StringToHash("Ability2")

    //        AudioSource audioSource = gameObject.GetComponent<AudioSource>();
    //        if (audioSource != null && basicAttackStats.sfx != null)
    //        {
    //            audioSource.clip = basicAttackStats.sfx;
    //            audioSource.Play();
    //        }
    //        yield return new WaitForSeconds(attackSpeed);
    //    }

    //    //make sure while stopped because currentFighter is gone
    //    if (currentTarget == null || !currentTarget.activeSelf)
    //    {
    //        SetCurrentTarget();
    //    }
    //}

    /// <summary>
    /// Returns true if this fighter is in range of their currentTarget
    /// </summary>
    /// <returns></returns>
    public bool InBasicRangeOfTarget(Vector3 t)
    {
        bool inRange = Vector2.Distance(transform.position, t) < basicAttackStats.range;
        
        return inRange;
    }

    public void SetCurrentTarget()
    {
        //This code chuck below checks if any enemies are active in the scene before calling a targeting function
        Fighter[] enemyListTMP = attackParent.GetComponentsInChildren<Fighter>();
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
        Fighter[] currentTargets = attackParent.GetComponentsInChildren<Fighter>();
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
        Fighter[] currentTargets = attackParent.GetComponentsInChildren<Fighter>();
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
        Fighter[] currentTargets = attackParent.GetComponentsInChildren<Fighter>();
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
        Fighter[] currentTargets = attackParent.GetComponentsInChildren<Fighter>();
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
        Fighter[] currentTargets = transform.parent.GetComponentsInChildren<Fighter>();
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
