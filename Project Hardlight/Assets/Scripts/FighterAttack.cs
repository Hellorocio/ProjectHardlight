using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class implements a fighter's basic attacks
/// </summary>
public class FighterAttack : MonoBehaviour
{
    public List<CombatInfo.TargetPreference> targetPrefs;

    // Basic attacking
    private bool doBasicAttack;
    private IEnumerator basicAttackLoop;

    [HideInInspector]
    public GameObject currentTarget;
    public BasicAttackAction attack;

    private GameObject attackParent;

    private Fighter fighter;
    private FighterMove fighterMove;

    // This makes it so there's not weird jittering on the edge of your attack range
    private static float attackRangeAllowance = 0.1f;

    //switch target event
    public delegate void SwitchTarget();
    public event SwitchTarget OnSwitchTarget;

    // Start is called before the first frame update
    void Start()
    {
        fighter = GetComponent<Fighter>();
        fighterMove = GetComponent<FighterMove>();

        if (fighter.team == CombatInfo.Team.Hero)
        {
            attackParent = GameObject.Find("Enemies");
        }
        else
        {
            attackParent = GameObject.Find("Vessels");
        }
    }

    /// <summary>
    /// Subscribe to BattleStart event
    /// </summary>
    private void OnEnable()
    {
        BattleManager battleManager = BattleManager.Instance;
        
        if (battleManager != null)
        {
            battleManager.OnLevelStart += LevelStart;
        }
    }

    /// <summary>
    /// Unsubscribe from all events to avoid memory leaks
    /// </summary>
    private void OnDisable()
    {
        if (fighter != null && fighter.battleManager != null)
        {
            fighter.battleManager.OnLevelStart -= LevelStart;
        }
    }

    /// <summary>
    /// Called when the fighter gets the signal that the battle has started
    /// Sets targets for heroes, but not enemies
    /// (UPDATE) Now called directly by BattleManager, not by event because the events were causing problems
    /// </summary>
    public void LevelStart ()
    {
        //make sure everything has been initialized
        if (fighter == null)
        {
            Start();
        }
        
        //only start attacking right away if a hero or a fighter with no enemyTrigger
        if (fighter.team == CombatInfo.Team.Hero || (fighter.team == CombatInfo.Team.Enemy && GetComponentInParent<EnemyTrigger>() == null))
        {
            
            SetCurrentTarget();
        }
    }

    /// <summary>
    /// Called by FighterMove when this fighter is in range of its current attack target
    /// </summary>
    public void StartBasicAttacking()
    {
        StartCoroutine(BasicAttackLoop());
    }

    public void StopBasicAttacking()
    {
        StopCoroutine(BasicAttackLoop());
    }

    IEnumerator BasicAttackLoop()
    {
        while (currentTarget != null && currentTarget.activeSelf)
        {
            //check we are still in range
            if (!InRangeOfTarget(currentTarget.transform))
            {
                fighterMove.StartMoving(currentTarget.transform);
                break;
            }

            //attack
            attack.DoBasicAttack(GetComponent<Fighter>(), currentTarget);
            //&& (fighter.anim.GetCurrentAnimatorStateInfo(0).shortNameHash != Animator.StringToHash("Ability1")) ||
            //fighter.anim.GetCurrentAnimatorStateInfo(0).shortNameHash != Animator.StringToHash("Ability2")
            if (fighter.anim != null )
            {
                fighter.anim.Play("Attack");
            }

            AudioSource audioSource = gameObject.GetComponent<AudioSource>();
            if (audioSource != null && attack.sfx != null)
            {
                audioSource.clip = attack.sfx;
                audioSource.Play();
            }
            yield return new WaitForSeconds(fighter.GetAttackSpeed(attack.cooldown));
        }

        //make sure while stopped because currentFighter is gone
        if (currentTarget == null || !currentTarget.activeSelf)
        {
            SetCurrentTarget();
        }
    }

    /// <summary>
    /// Returns true if this fighter is in range of their currentTarget
    /// </summary>
    /// <returns></returns>
    public bool InRangeOfTarget (Transform t, bool useRange = true)
    {
        bool inRange = Vector2.Distance(transform.position, t.position) < attack.range + attackRangeAllowance;
        if (!useRange)
        {
            inRange = Vector2.Distance(transform.position, t.position) < attackRangeAllowance;
        }
        return inRange;
    }


    /// <summary>
    /// Sets the attack target, called when player manually changes attack target
    /// </summary>
    public void SetIssuedCurrentTarget(Fighter target)
    {
        if (target != null && ((target.team == CombatInfo.Team.Enemy && !HasHealingBasicAttack())
                || (target.team == CombatInfo.Team.Hero && HasHealingBasicAttack())))
        {
            //Updates the current target
            currentTarget = target.gameObject;

            //invoke OnSwitchTarget event
            OnSwitchTarget?.Invoke();

            //start moving toward target
            if (currentTarget != null)
            {
                fighterMove.StartMoving(currentTarget.transform);
            }
        }
    }

    /// <summary>
    /// Searches enemies or players gameObject for a target to attack and sets currentTarget
    /// Sets currentTarget to null if there are no more things to attack
    /// Will use targetprefs list if provided, otherwise will default to closest algorithm
    /// </summary>
    public void SetCurrentTarget()
    {
        //This code chuck below checks if any enemies are active in the scene before calling a targeting function
        GenericMeleeMonster[] enemyListTMP1 = attackParent.GetComponentsInChildren<GenericMeleeMonster>();
        GenericRangedMonster[] enemyListTMP2 = attackParent.GetComponentsInChildren<GenericRangedMonster>();
        bool enemiesActive = false;

        for (int i = 0; i < enemyListTMP1.Length; i++)
        {
            if (enemyListTMP1[i].gameObject.activeSelf)
            {
                enemiesActive = true;
                break;
            }
        }

        if (!enemiesActive)
        {
            for (int i = 0; i < enemyListTMP2.Length; i++)
            {
                if (enemyListTMP2[i].gameObject.activeSelf)
                {
                    enemiesActive = true;
                    break;
                }
            }
        }

        bool newTargetWasSelected = false;
        if (enemiesActive)
        {
            //Default if no preferences exist
            //if (targetPrefs.Count == 0)
            //{
                SetClosestAttackTarget();
            //}
            /*
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
            } */
        }

        //invoke OnSwitchTarget event
        OnSwitchTarget?.Invoke();

        //start moving toward target

        //Debug.Log("Game obj is " + gameObject.name + " | current target is null? = " + (currentTarget == null));
        if (currentTarget != null)
        {
            fighterMove.StartMoving(currentTarget.transform);
        }
    }

    /*
    /// <summary>
    /// Finds the weakest enemy and sets current target
    /// </summary>
    void SetWeakestAttackTarget()
    {
        GenericMeleeMonster[] currentTargets1 = attackParent.GetComponentsInChildren<GenericMeleeMonster>();
        GenericRangedMonster[] currentTargets2 = attackParent.GetComponentsInChildren<GenericRangedMonster>();
        bool firstList = true;
        float hp = float.MaxValue;
        int index = 0;
        
        for (int i = 0; i < currentTargets1.Length; i++)
        {
            if (currentTargets1[i].gameObject.activeSelf)
            {
                if (currentTargets1[i].GetHealth() < hp)
                {
                    hp = currentTargets1[i].GetHealth();
                    index = i;
                }
            }
        }

        for (int i = 0; i < currentTargets2.Length; i++)
        {
            if (currentTargets2[i].gameObject.activeSelf)
            {
                if (currentTargets2[i].GetHealth() < hp)
                {
                    hp = currentTargets2[i].GetHealth();
                    index = i;
                    firstList = false;
                }
            }
        }
        if (firstList)
        {
            currentTarget = currentTargets1[index].gameObject;
        } else
        {
            currentTarget = currentTargets2[index].gameObject;
        }
    }

    /// <summary>
    /// Finds the strongest (Most HP) enemy and sets current target
    /// </summary>
    void SetStrongesttAttackTarget()
    {
        GenericMeleeMonster[] currentTargets1 = attackParent.GetComponentsInChildren<GenericMeleeMonster>();
        GenericRangedMonster[] currentTargets2 = attackParent.GetComponentsInChildren<GenericRangedMonster>();
        bool firstList = true;
        float hp = -1;
        int index = 0;

        for (int i = 0; i < currentTargets1.Length; i++)
        {
            if (currentTargets1[i].gameObject.activeSelf)
            {
                if (currentTargets1[i].GetHealth() > hp)
                {
                    hp = currentTargets1[i].GetHealth();
                    index = i;
                }
            }
        }

        for (int i = 0; i < currentTargets2.Length; i++)
        {
            if (currentTargets2[i].gameObject.activeSelf)
            {
                if (currentTargets2[i].GetHealth() > hp)
                {
                    hp = currentTargets2[i].GetHealth();
                    index = i;
                    firstList = false;
                }
            }
        }
        if (firstList)
        {
            currentTarget = currentTargets1[index].gameObject;
        }
        else
        {
            currentTarget = currentTargets2[index].gameObject;
        }
    }

    /// <summary>
    /// Attempts to find a ranged hero to kill
    /// If one doesn't exist, use next preference
    /// </summary>
    bool SetRangedAttackTarget()
    {
        GenericMeleeMonster[] currentTargets1 = attackParent.GetComponentsInChildren<GenericMeleeMonster>();
        GenericRangedMonster[] currentTargets2 = attackParent.GetComponentsInChildren<GenericRangedMonster>();
        bool firstList = true;
        float minDist = float.MaxValue;
        GameObject tempcurrentTarget = null;

        for (int i = 0; i < currentTargets.Length; i++)
        {
            if (currentTargets[i].gameObject.activeSelf)
            {
                float dist = (transform.position - currentTargets[i].transform.position).sqrMagnitude;
                if (dist < minDist && currentTargets[i].GetComponent<FighterAttack>().attack.range > 3)
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
        GenericMeleeMonster[] currentTargets1 = attackParent.GetComponentsInChildren<GenericMeleeMonster>();
        GenericRangedMonster[] currentTargets2 = attackParent.GetComponentsInChildren<GenericRangedMonster>();
        bool firstList = true;
        float minDist = float.MaxValue;
        GameObject tempcurrentTarget = null;

        for (int i = 0; i < currentTargets.Length; i++)
        {
            if (currentTargets[i].gameObject.activeSelf)
            {
                float dist = (transform.position - currentTargets[i].transform.position).sqrMagnitude;
                if (dist < minDist && currentTargets[i].GetComponent<FighterAttack>().attack.range < 4)
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
        GenericMeleeMonster[] currentTargets1 = attackParent.GetComponentsInChildren<GenericMeleeMonster>();
        GenericRangedMonster[] currentTargets2 = attackParent.GetComponentsInChildren<GenericRangedMonster>();
        bool firstList = true;
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
    */
    /// <summary>
    /// Finds the closest enemy and sets current target
    /// </summary>
    void SetClosestAttackTarget()
    {
        GenericMeleeMonster[] currentTargets1 = attackParent.GetComponentsInChildren<GenericMeleeMonster>();
        GenericRangedMonster[] currentTargets2 = attackParent.GetComponentsInChildren<GenericRangedMonster>();
        bool firstList = true;
        float minDist = float.MaxValue;
        GameObject tempcurrentTarget = null;

        for (int i = 0; i < currentTargets1.Length; i++)
        {
            if (currentTargets1[i].gameObject.activeSelf)
            {
                float dist = (transform.position - currentTargets1[i].transform.position).sqrMagnitude;
                if (dist < minDist)
                {
                    minDist = dist;
                    tempcurrentTarget = currentTargets1[i].gameObject;
                }
            }
        }

        for (int i = 0; i < currentTargets2.Length; i++)
        {
            if (currentTargets2[i].gameObject.activeSelf)
            {
                float dist = (transform.position - currentTargets2[i].transform.position).sqrMagnitude;
                if (dist < minDist)
                {
                    minDist = dist;
                    tempcurrentTarget = currentTargets2[i].gameObject;
                    firstList = false;
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

    public bool HasHealingBasicAttack ()
    {
        return targetPrefs.Count > 0 && targetPrefs[0] == CombatInfo.TargetPreference.WeakestTeamate;
    }
}
