using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMonsterAI : MonsterAI
{
    // Start is called before the first frame update
    /// <summary>
    /// Plays the attack. This includes sync'ing the animator and sounds with dealing damage.
    /// If the player has moved out of range before the damage is dealt then the coroutine is ended early
    /// </summary>
    /// <returns></returns>
    /// 
    public GameObject attackZone;
    public ContactFilter2D myFilter;
    private Transform defaultAttackZoneTransform;
    protected override void Start()
    {
        battleManager = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        animator = gameObject.GetComponentInChildren<Animator>();
        animator.SetFloat("basicAttackSpeedMultiplier", basicAttackClipSpeedMultiplier);
        realBasicAttackHitTime = basicAttackHitTime / basicAttackClipSpeedMultiplier;
        currentHealth = maxHealth;
        currentMana = 0;
        attackParent = GameObject.Find("Vessels");
        startPos = transform.position;
        GameObject wayPt = new GameObject("GolemWayPt");
        wayPt.transform.position = startPos;
        patrolRoute.Add(wayPt.transform);
        defaultAttackZoneTransform = attackZone.transform;

    }

    public override IEnumerator BasicAttack()
    {
        while (jabsDone < numJabsInAttack)
        {
            jabsDone++;
            animator.Play("BasicAttack");
            AudioSource audioSource = gameObject.GetComponent<AudioSource>();
            if (audioSource != null && basicAttackSfx != null)
            {
                audioSource.clip = basicAttackSfx;
                audioSource.Play();
            }
            else
            {
                Debug.Log("No valid audioSource or sfx for this enemy's attack!!");
            }

            //if (currentTarget != null && !InBasicRangeOfTarget(currentTarget.transform.position))
            //{
            //    StopBasicAttacking();
            //}
            attackZone.SetActive(true);
            //attackZone.transform.LookAt(currentTarget.transform);
            attackZone.transform.position = Vector3.MoveTowards(transform.position, currentTarget.transform.position, 8f);
            yield return new WaitForSeconds(2f);
            float tmp = basicAttackClipSpeedMultiplier;
            basicAttackClipSpeedMultiplier = .75f;
            animator.SetFloat("basicAttackSpeedMultiplier", basicAttackClipSpeedMultiplier);
            realBasicAttackHitTime = basicAttackHitTime / basicAttackClipSpeedMultiplier;
            yield return new WaitForSeconds(realBasicAttackHitTime);
            
            Collider2D[] hitColliders = new Collider2D[1000];
            Physics2D.OverlapCollider(attackZone.GetComponent<CircleCollider2D>(), myFilter, hitColliders);
            List<GameObject> hitFighters = new List<GameObject>();
            foreach(Collider2D collider in hitColliders)
            {
                if(collider != null && collider.gameObject.GetComponent<Fighter>() != null)
                {
                    DoBasicAttack(collider.gameObject);
                }
            }
            attackZone.transform.position = transform.position;
            attackZone.transform.rotation = defaultAttackZoneTransform.rotation;
            attackZone.SetActive(false);
            basicAttackClipSpeedMultiplier = tmp;
            animator.SetFloat("basicAttackSpeedMultiplier", basicAttackClipSpeedMultiplier);
            realBasicAttackHitTime = basicAttackHitTime / basicAttackClipSpeedMultiplier;
            yield return new WaitForSeconds(basicAttackClip.length / basicAttackClipSpeedMultiplier - realBasicAttackHitTime);
            
        }
        
        yield return new WaitForSeconds(timeBetweenAttacks);
        jabsDone = 0;
        moveState = MoveState.stopped;
        attackCoroutine = null;
    }

    /// <summary>
    /// Stops the basicAttack early. Currently used for when the target has moved out of range before the attack is finished
    /// Stops the basic attack in the event of an interruption?? (future case)
    /// </summary>
    public override void StopBasicAttacking()
    {
        StopCoroutine(attackCoroutine);
        attackZone.SetActive(false);
        if (jabsDone >= numJabsInAttack)
        {
            jabsDone = 0;
        }
        moveState = MoveState.stopped;
        attackCoroutine = null;
    }

    /// <summary>
    /// Check the monster's current target and if it is not valid then check if there are any valid targets
    /// </summary>
    protected override void UpdateTarget()
    {
        if (!IsValidTarget(currentTarget)) // Needs to also check if there are multiple enemies in alerted dist
        {
            
            if (moveState == MoveState.moving)
            {
                moveState = MoveState.stopped;
            }
            SetCurrentTarget();
        }
    }
    /// <summary>
    /// Returns a list of fighters that are non-null, active, and within this monster's aggro range
    /// </summary>
    /// <returns></returns>
    protected override List<Attackable> GetValidTargets()
    {
        List<Attackable> fighters = new List<Attackable>();
        Attackable[] enemyListTMP = attackParent.GetComponentsInChildren<Attackable>();
        for (int i = 0; i < enemyListTMP.Length; i++)
        {
            if (IsValidTarget(enemyListTMP[i].gameObject) && InAlertedRange(enemyListTMP[i].transform.position) && InMaxAgroRange(enemyListTMP[i].transform.position))
            {
                fighters.Add(enemyListTMP[i]);
            }
        }
        anyValidTargets = fighters.Count > 0;
        return fighters;
    }


}
