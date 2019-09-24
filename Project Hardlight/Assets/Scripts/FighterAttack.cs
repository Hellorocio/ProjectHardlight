﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class implements a fighter's basic attacks
/// </summary>
public class FighterAttack : MonoBehaviour
{
    public List<CombatInfo.TargetPreference> targetPrefs;

    // Basic attacking
    public MonoBehaviour basicAttackAction;
    public BasicAttackStats basicAttackStats;
    private bool doBasicAttack;
    private IEnumerator basicAttackLoop;

    [HideInInspector]
    public GameObject currentTarget;
    private BasicAttackAction attack;

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
        attack = (BasicAttackAction)basicAttackAction;

        if (fighter.team == CombatInfo.Team.Hero)
        {
            attackParent = GameObject.Find("Enemies");
        }
        else
        {
            attackParent = GameObject.Find("Players");
        }
    }

    /// <summary>
    /// Subscribe to BattleStart event
    /// </summary>
    private void OnEnable()
    {
        BattleManager battleManager = GameObject.Find("BattleManager").GetComponent<BattleManager>();

        if (battleManager != null)
        {
            battleManager.OnLevelStart += SetCurrentTarget;
        }
    }

    /// <summary>
    /// Unsubscribe from all events to avoid memory leaks
    /// </summary>
    private void OnDisable()
    {
        if (fighter.battleManager != null)
        {
            fighter.battleManager.OnLevelStart -= SetCurrentTarget;
        }
    }

    /// <summary>
    /// Called by FighterMove when this fighter is in range of its current attack target
    /// </summary>
    public void StartBasicAttacking()
    {
        StartCoroutine(BasicAttackLoop());
    }

    IEnumerator BasicAttackLoop()
    {
        while (currentTarget != null && currentTarget.activeSelf)
        {
            //check we are still in range
            if (!InRangeOfTarget())
            {
                fighterMove.StartMoving(currentTarget.transform);
                break;
            }

            //attack
            attack.DoBasicAttack(currentTarget);
            if (fighter.anim != null)
            {
                fighter.anim.Play("Attack");
            }

            AudioSource audioSource = gameObject.GetComponent<AudioSource>();
            if (audioSource != null && basicAttackStats.sfx != null)
            {
                audioSource.clip = basicAttackStats.sfx;
                audioSource.Play();
            }
            yield return new WaitForSeconds(fighter.GetAttackSpeed(basicAttackStats.attackSpeed));
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
    public bool InRangeOfTarget ()
    {
        return Vector2.Distance(transform.position, currentTarget.transform.position) < basicAttackStats.range + attackRangeAllowance;
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
        //make sure everything has been initialized
        if (attackParent == null)
        {
            Start();
        }

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

        //invoke OnSwitchTarget event
        OnSwitchTarget?.Invoke();

        //start moving toward target
        if (currentTarget != null)
        {
            fighterMove.StartMoving(currentTarget.transform);
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
        float minDist = 1000f;
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
        float minDist = 1000f;
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
        float minDist = 1000f;
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
    void SetOptimalHealingTarget()
    {
        Fighter[] currentTargets = transform.parent.GetComponentsInChildren<Fighter>();
        float maxHealth = 1000f;
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

    bool HasHealingBasicAttack ()
    {
        return targetPrefs.Count > 0 && targetPrefs[0] == CombatInfo.TargetPreference.WeakestTeamate;
    }
}
