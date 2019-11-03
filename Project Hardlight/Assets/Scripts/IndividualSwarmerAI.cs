﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndividualSwarmerAI : GenericMonsterAI
{
    [Header("Basic Attributes")]
    public string characterName;
    public CombatInfo.Team team;
    //public float maxHealth;
    public float maxMana;
    public float moveSpeed;
    public float alertedRange;
    public float maxAggroRange;
    public List<CombatInfo.TargetPreference> targetPrefs;
    [Space(10)]

    [Header("Basic Attack Stats")]
    public float basicAttackRange;
    public float timeBetweenAttacks;
    public int numJabsInAttack;
    public GameObject basicAttackPrefab;
    public GameObject spawnPoint;
    public int basicAttackDamage;
    public float basicAttackProjectileSpeed;
    public AudioClip basicAttackSfx;
    [Space(10)]

   

    [Header("Animation Info")]
    public AnimationClip basicAttackClip;
    public float basicAttackClipSpeedMultiplier;
    public float basicAttackHitTime; //How long into the animation before the hit should be displayed to the player
    [Space(10)]

    [HideInInspector]
    public Vector3 swarmLoc;
    public bool isAttacking = false;
    protected BattleManager battleManager;
    protected float realBasicAttackHitTime;
    protected Animator animator;
    protected Color defaultColor;
    protected Color hitColor;
    protected float currentHealth;
    protected float currentMana;
    protected Coroutine attackCoroutine;
    protected GameObject currentTarget;

    void Start()
    {
        battleManager = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        animator = gameObject.GetComponentInChildren<Animator>();
        animator.SetFloat("basicAttackSpeedMultiplier", basicAttackClipSpeedMultiplier);
        realBasicAttackHitTime = basicAttackHitTime / basicAttackClipSpeedMultiplier;
        currentHealth = maxHealth;
        currentMana = 0;
        defaultColor = gameObject.GetComponentInChildren<SpriteRenderer>().color;
        hitColor = new Color(1f, .5235f, .6194f);
    }


    private void Update()
    {
        while (attackCoroutine == null && !InBodyRangeOfTarget(swarmLoc))
        {
            transform.position = Vector3.MoveTowards(transform.position, swarmLoc, moveSpeed / 100 * Time.deltaTime);
        }
        currentTarget = transform.parent.GetComponent<SwarmMasterAI>().currentTarget;
    }

    /// <summary>
    /// Starts the basic attacking coroutine
    /// </summary>
    public void StartBasicAttacking()
    {
        
        if (attackCoroutine == null)
        {
            isAttacking = true;
            attackCoroutine = StartCoroutine(BasicAttack());
        }

    }

    /// <summary>
    /// Stops the basicAttack early. Currently used for when the target has moved out of range before the attack is finished
    /// Stops the basic attack in the event of an interruption?? (future case)
    /// </summary>
    public void StopBasicAttacking()
    {
        StopCoroutine(attackCoroutine);
        attackCoroutine = null;
        isAttacking = false;
    }


    public void DoBasicAttack(GameObject target)
    {

        target.GetComponent<Fighter>().TakeDamage(basicAttackDamage);

    }

    /// <summary>
    /// Plays the attack. This includes sync'ing the animator and sounds with dealing damage.
    /// If the player has moved out of range before the damage is dealt then the coroutine is ended early
    /// </summary>
    /// <returns></returns>
    public IEnumerator BasicAttack()
    {

        while (currentTarget != null && currentTarget.activeSelf && !InBasicRangeOfTarget(currentTarget.transform.position)){
            Vector3 enemyLoc = new Vector3(currentTarget.transform.position.x, currentTarget.transform.position.y, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, enemyLoc, moveSpeed/100 * Time.deltaTime);
            yield return null;
        }

        if (currentTarget != null && currentTarget.activeSelf)
        {
            DoBasicAttack(currentTarget);
            yield return null;
        }


        while (!InBodyRangeOfTarget(swarmLoc))
        {
            transform.position = Vector3.MoveTowards(transform.position, swarmLoc, moveSpeed / 100 * Time.deltaTime);
            yield return null;
        }

        StopBasicAttacking();
        
    }



    /// <summary>
    /// Returns true if this fighter is in range of their currentTarget
    /// </summary>
    /// <returns></returns>
    public bool InBasicRangeOfTarget(Vector3 p)
    {
        
        return Vector2.Distance(transform.position, p) < basicAttackRange;

    }

    public bool InBodyRangeOfTarget(Vector3 p)
    {
        //Debug.Log(Vector2.Distance(transform.position, p).ToString() + " " + basicAttackStats.range);
        return Vector2.Distance(transform.position, p) < 0.1;

    }




    public override void TakeDamage(float damage)
    {
        currentHealth -= damage;
        IEnumerator colorThing = HitColorChanger();
        StartCoroutine(colorThing);
        CheckHealth(currentHealth);
        if (currentHealth <= 0)
        {
            OnDeath();
        }
    }


    public void OnDeath()
    {
        gameObject.SetActive(false);
    }

    public float GetHealth()
    {
        return currentHealth;
    }

    IEnumerator HitColorChanger()
    {

        gameObject.GetComponentInChildren<SpriteRenderer>().color = hitColor;
        yield return new WaitForSeconds((float)0.25);
        gameObject.GetComponentInChildren<SpriteRenderer>().color = defaultColor;
    }

    
}
