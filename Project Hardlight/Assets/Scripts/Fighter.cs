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
    public Animator anim;
    public FighterStats fighterStats;

    public MonoBehaviour basicAttackAction;

    private float health;
    private float speed;
    private float mana;

    private enum State {Idle, Move, BasicAttack};
    private State currentState = State.Idle;

    private GameObject attackParent;

    // Basic attacking
    public BasicAttackStats basicAttackStats;
    public GameObject currentTarget;
    public GameObject attackTarget;
    private IEnumerator basicAttackLoop;
    // This makes it so there's not weird jittering on the edge of your attack range
    private static float attackRangeAllowance = 0.2f;
    

    // Move
    private IEnumerator moveLoop;

    // Start is called before the first frame update
    void Start()
    {
        health = fighterStats.maxHealth + fighterStats.maxHealth * fighterStats.soul.healthBoost;
        speed = fighterStats.movementSpeed + fighterStats.movementSpeed * fighterStats.soul.movementSpeedBoost;
        mana = 0;

        if (team == CombatInfo.Team.Hero)
        {
            attackParent = GameObject.Find("Enemies");
            
        }
        else
        {
            attackParent = GameObject.Find("Players");
        }

        SetHealthUI();
        SetManaUI();
        SetCurrentTarget();
    }

    // Update is called once per frame
    void Update()
    {

        // Basic AI
        // TODO(Don't stop movement if player issued the Move command)

        // You have a target to go for
        if (currentTarget != null && currentTarget.activeSelf)
        {
            float distanceFromTarget = Vector3.Distance(transform.position, currentTarget.transform.position);

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
                if (attackTarget == null)
                {
                    attackTarget = currentTarget;
                    basicAttackLoop = BasicAttackLoop();
                    StartCoroutine(basicAttackLoop);
                    currentState = State.BasicAttack;
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
                    currentState = State.Move;
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
                currentState = State.Idle;
                // Stop basic attacking
                if (basicAttackLoop != null)
                {
                    StopCoroutine(basicAttackLoop);
                    basicAttackLoop = null;
                }
            }
        }
    }
    
    IEnumerator MoveLoop()
    {
        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, currentTarget.transform.position, speed * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator BasicAttackLoop()
    {
        while (true)
        {
            BasicAttackAction attack = (BasicAttackAction)basicAttackAction;
            attack.DoBasicAttack();
            anim.Play("Attack");
            yield return new WaitForSeconds(basicAttackStats.attackSpeed + basicAttackStats.attackSpeed * fighterStats.soul.attackSpeedBoost);
        }
    }

    /// <summary>
    /// Called by other fighters when they attack this one
    /// </summary>
    /// <param name="dmg"></param>
    public void Attack (float dmg)
    {
        health -= dmg - dmg * fighterStats.soul.defenseBoost;

        if (health <= 0)
        {
            gameObject.SetActive(false);
        }

        SetHealthUI();
    }

    // TODO cap at max mana, do something special when mana hits max
    public void GainMana (int manaGained)
    {
        mana += manaGained + manaGained * fighterStats.soul.manaGenerationBoost;
        SetManaUI();
    }

    /// <summary>
    /// Searches enemies or players gameObject for the closest thing to attack and sets currentTarget
    /// Sets currentTarget to null if there are no more things to attack
    /// </summary>
    void SetCurrentTarget()
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
    /// Right now this sets the HPText, probably change later to a health bar
    /// </summary>
    void SetHealthUI()
    {
        healthUI.GetComponent<Text>().text = ((int)health).ToString();
    }

    void SetManaUI()
    {
        manaUI.GetComponent<Text>().text = ((int)mana).ToString();
    }
}
