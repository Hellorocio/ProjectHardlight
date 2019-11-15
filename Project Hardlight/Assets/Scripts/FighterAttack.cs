using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class implements a fighter's basic attacks
/// </summary>
public class FighterAttack : MonoBehaviour
{
    public List<CombatInfo.TargetPreference> targetPrefs;

    public bool allowAutoMovement = false;
    public bool autoMoveOnTarget = true;

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

        if (GetComponent<Attackable>().team == CombatInfo.Team.Hero)
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
        if (GetComponent<Attackable>().team == CombatInfo.Team.Hero || (GetComponent<Attackable>().team == CombatInfo.Team.Enemy && GetComponentInParent<EnemyTrigger>() == null))
        {
            StartBasicAttacking();
        }
    }

    /// <summary>
    /// Called by FighterMove when this fighter is in range of its current attack target
    /// </summary>
    public void StartBasicAttacking()
    {
        if (basicAttackLoop == null)
        {
            basicAttackLoop = BasicAttackLoop();
            StartCoroutine(basicAttackLoop);
        }
    }

    public void StopBasicAttacking()
    {
        if (basicAttackLoop != null)
        {
            StopCoroutine(basicAttackLoop);
            basicAttackLoop = null;
        }
    }

    /// <summary>
    /// Repeatedly attacks on a cooldown timer
    /// If gamestate is PAUSED or if the figher is moving, timer is still active, but attack won't happen
    /// The fighter moving check is used to prevent the player from stopping and starting moving to get free basic attacks without the timer
    /// This is still exploitable if the fighter goes in an out of range, but I think that won't be an issue as long as movement speed stays pretty slow
    /// </summary>
    /// <returns></returns>
    IEnumerator BasicAttackLoop()
    {
        while (IsTargetInAggroRange())
        {
            // check we are still in range
            if (!InRangeOfTarget(currentTarget.transform))
            {
                if (allowAutoMovement)
                {
                    fighterMove.StartMoving(currentTarget.transform);
                }
                break;
            }
            fighterMove.sprite.flipX = (currentTarget.transform.position.x < transform.position.x);
            // attack
            if (GameManager.Instance.gameState != GameState.PAUSED && fighterMove.GetMoveState() != FighterMove.MoveState.moving)
            {
                attack.DoBasicAttack(GetComponent<Fighter>(), currentTarget);
                //Debug.Log("Current anime" + fighter.anim.GetCurrentAnimatorStateInfo(0).shortNameHash);
                //Debug.Log("ability 2" + Animator.StringToHash("Ability2"));
                //Debug.Log("ability 1" + Animator.StringToHash("Ability1"));
                if (fighter.anim != null) {
                    int nameHash = fighter.anim.GetCurrentAnimatorStateInfo(0).shortNameHash;
                    if ((nameHash != Animator.StringToHash("Ability1"))
                        && (nameHash != Animator.StringToHash("Ability2")))
                    {
                        fighter.anim.Play("Attack");
                    }
                }

                AudioSource audioSource = gameObject.GetComponent<AudioSource>();
                if (audioSource != null && attack.sfx != null && !audioSource.isPlaying)
                {
                    audioSource.clip = attack.sfx;
                    audioSource.Play();
                }
            }
           
            yield return new WaitForSeconds(fighter.GetAttackSpeed(1/attack.frequency));
        }

        /*
        if (fighterMove.GetMoveState() != FighterMove.MoveState.moving && fighter.anim.HasState(0, Animator.StringToHash("Idle")))
        {
            print(gameObject.name + ": BasicAttackLoop idle");
            fighter.anim.Play("Idle");
        }
        */

        // if current fight is gone and the battle isn't over, find a new target
        if ((currentTarget == null || !currentTarget.activeSelf) && BattleManager.Instance.inputState != BattleManager.InputState.BattleOver)
        {
            SetCurrentTarget();
        }
        else
        {
            StopBasicAttacking();
        }        
    }

    /// <summary>
    /// Returns true if this fighter is in range of their currentTarget
    /// </summary>
    /// <returns></returns>
    public bool InRangeOfTarget (Transform t, bool useRange = true)
    {
        if(attack == null)
        {
            Debug.Log(gameObject.name);
            Debug.Log("BasicAttackAction is null");
        }
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
    public void SetIssuedCurrentTarget(Attackable target)
    {
        if (target != null && ((target.team == CombatInfo.Team.Enemy && !HasHealingBasicAttack())
                || (target.team == CombatInfo.Team.Hero && HasHealingBasicAttack())))
        {
            //Updates the current target
            currentTarget = target.gameObject;

            //invoke OnSwitchTarget event
            OnSwitchTarget?.Invoke();
            
            // interrupt movement
            if (fighterMove.GetMoveState() == FighterMove.MoveState.moving)
            {
                
                fighterMove.StopMovingCommandHandle(false);
            }

            if (InRangeOfTarget(currentTarget.transform))
            {
                StartBasicAttacking();
            }

            // auto-move to target
            if (autoMoveOnTarget && currentTarget != null && !InRangeOfTarget(currentTarget.transform))
            {
                fighterMove.StartMoving(currentTarget.transform);
            }
        }
    }

    public void SetIssuedCurrentTarget(MonsterAI target)
    {
        if (target != null)
        {
            //Updates the current target
            currentTarget = target.gameObject;

            //invoke OnSwitchTarget event
            OnSwitchTarget?.Invoke();

            // interrupt movement
            if (fighterMove.GetMoveState() == FighterMove.MoveState.moving)
            {
                fighterMove.StopMovingCommandHandle(false);
            }

            // auto-move to target
            if (autoMoveOnTarget && currentTarget != null && !InRangeOfTarget(currentTarget.transform))
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
        Fighter[] enemyListTMP = attackParent.GetComponentsInChildren<Fighter>();
        MonsterAI[] monsters = attackParent.GetComponentsInChildren<MonsterAI>();
        
        bool enemiesActive = false;
        //bool newTargetWasSelected = false;

        for (int i = 0; i < enemyListTMP.Length; i++)
        {
            if (enemyListTMP[i].gameObject.activeSelf)
            {
                enemiesActive = true;
                break;
            }
        }


        for (int i = 0; i < monsters.Length; i++)
        {
            if (monsters[i].gameObject.activeSelf)
            {
                enemiesActive = true;
                break;
            }
        }


        
        if (enemiesActive)
        {
            //Default if no preferences exist
            if (!HasHealingBasicAttack())
            {
                SetClosestAttackTarget();
            }
            else
            {
                SetOptimalHealingTarget();
            }
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

        // reset basic attack loop
        // we need this because if we set the current target in the attack loop, we can't start a new attack loop
        // until we get rid of the old one
        StopBasicAttacking();

        if (currentTarget != null && currentTarget.activeSelf)
        {
            StartBasicAttacking();
        }
    }

    bool IsTargetInAggroRange()
    {
        return currentTarget != null && currentTarget.activeSelf && Vector3.Distance(transform.position, currentTarget.transform.position) < gameObject.GetComponent<VesselData>().maxAggroRange;
    }

    /// <summary>
    /// Finds the closest enemy and sets current target
    /// </summary>
    void SetClosestAttackTarget()
    {
        Fighter[] currentTargets = attackParent.GetComponentsInChildren<Fighter>();
        MonsterAI[] monsters = attackParent.GetComponentsInChildren<MonsterAI>();
        
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

        for (int i = 0; i < monsters.Length; i++)
        {
            if (monsters[i].gameObject.activeSelf)
            {
                float dist = (transform.position - monsters[i].transform.position).sqrMagnitude;
                if (dist < minDist)
                {
                    minDist = dist;
                    tempcurrentTarget = monsters[i].gameObject;
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
        Attackable[] currentTargets = transform.parent.GetComponentsInChildren<Attackable>();
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
