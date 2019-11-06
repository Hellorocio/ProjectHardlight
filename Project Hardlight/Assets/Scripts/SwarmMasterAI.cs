using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmMasterAI : MonoBehaviour
{

    public int initalSpawnCount;
    public GameObject swarmer;
    public List<GameObject> mySwarmers;

    [Header("Basic Attributes")]
    public string characterName;
    public CombatInfo.Team team;
    public float moveSpeed;
    public float alertedRange;
    public float maxAggroRange;
    public GameObject alertedMark;
    public List<CombatInfo.TargetPreference> targetPrefs;
    [Space(10)]

    [Header("Basic Attack Stats")]
    public float newSpawnChance;
    public float basicAttackRange;
    public float timeBetweenAttacks;
    public AudioClip basicAttackSfx;
    [Space(10)]

    [Header("Patrol Info")]
    public PatrolType patrolType;
    public float idleTime;
    public int maxWanderPoints;
    public int minWanderPoints;
    public List<Transform> patrolRoute;
    [Space(10)]

    protected int numWanderPoints;
    protected int wanderPointCounter;
    protected Vector3 currentWanderPoint;
    protected float currentIdleTime = 0;
    protected BattleManager battleManager;
    protected bool startedLevel;
    protected Vector3 startPos;
    protected Coroutine attackCoroutine;
    [HideInInspector]
    public GameObject currentTarget;
    protected GameObject attackParent;
    protected bool anyValidTargets;
    protected int patrolIndex = -1;
    public enum MoveState { stopped, moving, patrolling, interrupted, basicAttacking, advancedAttacking }
    protected MoveState moveState = MoveState.stopped;
    public enum PatrolType { none, looping, reverse, random }
    private bool justAlerted = true;

    private void Start()
    {
        battleManager = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        attackParent = GameObject.Find("Vessels");
        startPos = transform.position;


        InitSpawnSwarmers();
    }

    void FixedUpdate()
    {
        if (startedLevel)
        {
            CleanSwarmers();
            GenerateNewSpacing();
            UpdateTarget();
            if (anyValidTargets)
            {
                if (justAlerted)
                {
                    justAlerted = false;
                    StartCoroutine(SetAlertedUI());
                }
                DecideAttack();
            }
            else
            {
                justAlerted = true;
                if (currentIdleTime > 0)
                {
                    currentIdleTime -= Time.deltaTime;

                }
                else
                {
                    if (wanderPointCounter < numWanderPoints)
                    {
                        Wander();
                    }
                    else
                    {
                        DoPatrol();
                    }

                }

            }
        }

    }


    /// <summary>
    /// Checks that all Individuals are active, otherwise remove them
    /// Also handles death event when all swarmers are dead
    /// </summary>
    public void CleanSwarmers()
    {
        for(int i = 0; i < mySwarmers.Count; i++)
        {
            if (!mySwarmers[i].activeSelf)
            {
                mySwarmers.RemoveAt(i);
                i--;
            }
        }
        if (mySwarmers.Count <= 0)
        {
            OnDeath();
        }

    }

    public void InitSpawnSwarmers()
    {
        for(int i = 0; i < initalSpawnCount; i++)
        {
            mySwarmers.Add(Instantiate(swarmer, transform));
        }
        GenerateNewSpacing();

    }

    public void SpawnNewIndividual()
    {
        mySwarmers.Add(Instantiate(swarmer, transform));
        GenerateNewSpacing();
    }

    public void GenerateNewSpacing()
    {
        int j = 0;
        for (int i = 0; i < mySwarmers.Count; i++)
        {
            if(mySwarmers.Count > 7)
            {
                if (j == 5 || j == 12)
                {
                    j = i + 1;
                }

                Vector3 newVec = new Vector3((transform.position.x - 1.2f) + (Mathf.Sin(j + .2f) + .2f * j), (transform.position.y - 1.2f) + (Mathf.Cos(j + .2f) + .2f * j), transform.position.z);
                mySwarmers[i].GetComponent<IndividualSwarmerAI>().swarmLoc = newVec;
                j++;
            } else
            {
                if (j == 5 || j == 12)
                {
                    j = i + 1;
                }

                Vector3 newVec = new Vector3((transform.position.x - 1.2f) + (Mathf.Sin(j + .2f) + .2f * j), (transform.position.y - 1.2f) + (Mathf.Cos(j + .2f) + .2f * j), transform.position.z);
                mySwarmers[i].GetComponent<IndividualSwarmerAI>().swarmLoc = newVec;
                j+=2;
            }
            
        }

        
    }

    /// <summary>
    /// Plays the attack. This includes sync'ing the animator and sounds with dealing damage.
    /// If the player has moved out of range before the damage is dealt then the coroutine is ended early
    /// </summary>
    /// <returns></returns>
    public IEnumerator BasicAttack()
    {
        List<IndividualSwarmerAI> swarmers = new List<IndividualSwarmerAI>();
        foreach(GameObject obj in mySwarmers)
        {
            if(obj != null && obj.activeSelf)
            {
                swarmers.Add(obj.GetComponent<IndividualSwarmerAI>());
            }
            
        }
        foreach(IndividualSwarmerAI minion in swarmers)
        {
            if(minion != null && minion.gameObject.activeSelf)
            {
                if (!InBasicRangeOfTarget(currentTarget.transform.position))
                {
                    StopBasicAttacking();
                }
                minion.StartBasicAttacking();
                yield return new WaitForSeconds(timeBetweenAttacks);
                if(minion != null && minion.gameObject.activeSelf)
                {
                    if(Random.value <= newSpawnChance)
                    {
                        SpawnNewIndividual();
                    }
                }
            }
        }

        moveState = MoveState.stopped;
        attackCoroutine = null;
    }

    /// <summary>
    /// Starts the basic attacking coroutine
    /// </summary>
    void StartBasicAttacking()
    {
        if (moveState != MoveState.basicAttacking)
        {
            moveState = MoveState.basicAttacking;
            if (attackCoroutine == null)
            {
                attackCoroutine = StartCoroutine(BasicAttack());
            }

        }

    }

    /// <summary>
    /// Stops the basicAttack early. Currently used for when the target has moved out of range before the attack is finished
    /// Stops the basic attack in the event of an interruption?? (future case)
    /// </summary>
    public void StopBasicAttacking()
    {
        StopCoroutine(attackCoroutine);
        
        moveState = MoveState.stopped;
        attackCoroutine = null;
    }

    public IEnumerator SetAlertedUI()
    {
        alertedMark.SetActive(true);
        yield return new WaitForSeconds(2);
        alertedMark.SetActive(false);
    }

    /// <summary>
    /// Checks if the target is active and within the maximum aggro boundary, updating bool anyValidTargets in the process
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    bool IsValidTarget(GameObject target)
    {
        anyValidTargets = (target != null && target.activeSelf && InMaxAgroRange(target.transform.position));
        if (!anyValidTargets)
        {
            currentTarget = null;
        }
        return anyValidTargets;
    }


    /// <summary>
    /// Returns a list of fighters that are non-null, active, and within this monster's aggro range
    /// </summary>
    /// <returns></returns>
    List<Attackable> GetValidTargets()
    {
        List<Attackable> fighters = new List<Attackable>();
        Attackable[] enemyListTMP = attackParent.GetComponentsInChildren<Attackable>();
        for (int i = 0; i < enemyListTMP.Length; i++)
        {
            if (IsValidTarget(enemyListTMP[i].gameObject) && InAlertedRange(enemyListTMP[i].transform.position))
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
            if (moveState == MoveState.moving)
            {
                moveState = MoveState.stopped;
            }
            SetCurrentTarget();
        }
    }

    /// <summary>
    /// Decides if the target is valid, if so then decide to move closer or attack.
    /// If the target is not valid then check for new ones
    /// </summary>
    void DecideAttack()
    {
        if (IsValidTarget(currentTarget))
        {
            if (!InBasicRangeOfTarget(currentTarget.transform.position) && moveState != MoveState.basicAttacking)
            {
                foreach (GameObject individual in mySwarmers)
                {
                    individual.GetComponentInChildren<SpriteRenderer>().flipX = currentTarget.transform.position.x < transform.position.x;
                }
                transform.position = Vector3.MoveTowards(transform.position, currentTarget.transform.position, moveSpeed / 100 * Time.deltaTime);
                moveState = MoveState.moving;


            }
            else
            {
                //Send an IndividualSwarmer to attack
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

    /// <summary>
    /// Checks if we are in attack range of the current target and if we arn't then we move closer
    /// </summary>
    void MoveToTarget()
    {

        if (IsValidTarget(currentTarget) && !InBasicRangeOfTarget(currentTarget.transform.position))
        {
            foreach (GameObject individual in mySwarmers)
            {
                individual.GetComponentInChildren<SpriteRenderer>().flipX = currentTarget.transform.position.x < transform.position.x;
            }
            transform.position = Vector3.MoveTowards(transform.position, currentTarget.transform.position, moveSpeed / 100 * Time.deltaTime);
            moveState = MoveState.moving;


        }
        else
        {
            
            moveState = MoveState.stopped;
        }

    }

    /// <summary>
    /// Used in the patrol system, this function moves the monster to a position in the same style as MoveToTarget
    /// </summary>
    /// <param name="pos"></param>
    void MoveToPosition(Vector3 pos)
    {
        if (!InBodyRangeOfTarget(pos))
        {

            foreach(GameObject individual in mySwarmers)
            {
                individual.GetComponentInChildren<SpriteRenderer>().flipX = pos.x < transform.position.x;
            }
            
            //Debug.Log("My loc = " + transform.position.ToString() + " | Pos loc = " + pos.ToString());
            transform.position = Vector3.MoveTowards(transform.position, pos, moveSpeed / 100 * Time.deltaTime);
            moveState = MoveState.patrolling;


        }
        else
        {
            
            moveState = MoveState.stopped;
            currentIdleTime = idleTime;
            numWanderPoints = Random.Range(minWanderPoints, maxWanderPoints);
            wanderPointCounter = 0;
        }
    }

    void MoveToWanderPoint(Vector3 pos)
    {
        if (!InBodyRangeOfTarget(pos))
        {
            foreach (GameObject individual in mySwarmers)
            {
                individual.GetComponentInChildren<SpriteRenderer>().flipX = pos.x < transform.position.x;
            }

            //Debug.Log("My loc = " + transform.position.ToString() + " | Pos loc = " + pos.ToString());
            transform.position = Vector3.MoveTowards(transform.position, pos, moveSpeed / 100 * Time.deltaTime);
            moveState = MoveState.patrolling;


        }
        else
        {
            
            moveState = MoveState.stopped;
            currentIdleTime = idleTime;
            wanderPointCounter++;
            if (wanderPointCounter == numWanderPoints)
            {
                numWanderPoints = 0;
            }
        }
    }

    /// <summary>
    /// Does a patrol based off the selected patrol type.
    /// Looping will go from the last index back to the first 1-2-3-1-2-3-1-2-3
    /// reverse will go down to the last index and then decrement back up to the top 1-2-3-2-1-2-3-2-1
    /// if you don't know what random means then I can't help you xD
    /// </summary>
    void DoPatrol()
    {
        if (patrolType != PatrolType.none && moveState == MoveState.stopped)
        {

            if (patrolType == PatrolType.looping)
            {
                ++patrolIndex;
                if (patrolIndex >= patrolRoute.Count)
                {
                    patrolIndex = 0;
                }
                Vector3 realLoc = patrolRoute[patrolIndex].position;
                realLoc.z = transform.position.z;
                MoveToPosition(realLoc);
                wanderPointCounter = 0;

            }
            else if (patrolType == PatrolType.reverse)
            {

            }
            else if (patrolType == PatrolType.random)
            {

            }
        }
        else if (moveState == MoveState.patrolling)
        {

            Vector3 realLoc = patrolRoute[patrolIndex].position;
            realLoc.z = transform.position.z;
            MoveToPosition(realLoc);
        }
    }

    void Wander()
    {

        if (patrolType != PatrolType.none && moveState == MoveState.stopped)
        {
            float ranX = Random.Range((-transform.position.x * .05f), (transform.position.x * .05f));
            float ranY = Random.Range((-transform.position.y * .05f), (transform.position.y * .05f));
            currentWanderPoint = new Vector3(transform.position.x + ranX, transform.position.y + ranY, transform.position.z);
            MoveToWanderPoint(currentWanderPoint);
        }
        else if (moveState == MoveState.patrolling)
        {
            MoveToWanderPoint(currentWanderPoint);
        }
    }

    /// <summary>
    /// Returns true if this fighter is in range of their currentTarget
    /// </summary>
    /// <returns></returns>
    public bool InBasicRangeOfTarget(Vector3 p)
    {
        //Debug.Log(Vector2.Distance(transform.position, p).ToString() + " " + basicAttackStats.range);
        return Vector2.Distance(transform.position, p) < basicAttackRange;

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

    /// <summary>
    /// MaxAgroRange represents the maximum boundary that the monster will chase a target to
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public bool InMaxAgroRange(Vector3 p)
    {
        return Vector2.Distance(startPos, p) < maxAggroRange;
    }

    /// <summary>
    /// Alerted range represents the maximum distance before the monster will select the vessel as their target
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public bool InAlertedRange(Vector3 p)
    {
        return Vector2.Distance(transform.position, p) < alertedRange;
    }


    public void LevelStart()
    {
        startedLevel = true;
    }

    public void OnDeath()
    {

        //Tell Battle manager that an enemy has died
        battleManager.OnDeath(GetComponent<Attackable>());
        gameObject.SetActive(false);
    }


    /// <summary>
    /// Sets the current target, if there is a valid one within maxAggroRange. Uses the preference enums if provided to select a specific type of fighter
    /// </summary>
    public void SetCurrentTarget()
    {
        //This code chuck below checks if any enemies are active in the scene before calling a targeting function
        Attackable[] enemyListTMP = GetValidTargets().ToArray();
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
        Attackable[] currentTargets = GetValidTargets().ToArray();
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
        Attackable[] currentTargets = GetValidTargets().ToArray();
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
        Attackable[] currentTargets = GetValidTargets().ToArray();
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
        Attackable[] currentTargets = GetValidTargets().ToArray();
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
        Attackable[] currentTargets = GetValidTargets().ToArray();
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
        Attackable[] currentTargets = GetValidTargets().ToArray();
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
        Attackable[] currentTargets = GetValidTargets().ToArray();
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
