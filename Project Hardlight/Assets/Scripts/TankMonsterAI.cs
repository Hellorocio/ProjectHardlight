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
    private Sprite targetSprite;
    private Color defaultZoneColor;
    public Sprite crackedSprite;
    public ContactFilter2D myFilter;
    private Transform defaultAttackZoneTransform;

    private Coroutine cracksCoroutine;
    
    protected override void Start()
    {
        targetSprite = attackZone.GetComponent<SpriteRenderer>().sprite;
        defaultZoneColor = attackZone.GetComponent<SpriteRenderer>().color;
        battleManager = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        animator = gameObject.GetComponentInChildren<Animator>();
        animator.SetFloat("basicAttackSpeedMultiplier", basicAttackClipSpeedMultiplier);
        realBasicAttackHitTime = basicAttackHitTime / basicAttackClipSpeedMultiplier;
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
            attackZone.transform.localPosition = new Vector3(0, 0, 0);
            attackZone.SetActive(true);
            Vector3 direction = new Vector3(currentTarget.transform.position.x, currentTarget.transform.position.y, transform.position.z);
            direction -= transform.position;
            direction = direction.normalized;
            attackZone.transform.localPosition = direction*2;
            
            yield return new WaitForSeconds(1.2f);

            Debug.Log(1);
            
            float modifiedAttackSpeed = .1f;
            animator.SetFloat("basicAttackSpeedMultiplier", modifiedAttackSpeed);
            
            yield return new WaitForSeconds(1.5f);
            Debug.Log(2);
            
            modifiedAttackSpeed = basicAttackClipSpeedMultiplier;
            animator.SetFloat("basicAttackSpeedMultiplier", modifiedAttackSpeed);
            
            yield return new WaitForSeconds(.4f);
            Debug.Log(3);
            
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
            
            cracksCoroutine = StartCoroutine(DisplayCracks());
            animator.SetFloat("basicAttackSpeedMultiplier", basicAttackClipSpeedMultiplier);
            yield return new WaitForSeconds(.75f);
            Debug.Log(4);
            
        }
        ShowTiredUI(true);
        yield return new WaitForSeconds(timeBetweenAttacks);
        ShowTiredUI(false);
        jabsDone = 0;
        moveState = MoveState.stopped;
        attackCoroutine = null;
    }

    public IEnumerator DisplayCracks()
    {
        attackZone.GetComponent<SpriteRenderer>().sprite = crackedSprite;
        attackZone.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, .9f);
        for (float i = 1; i > 0; i -= .01f)
        {
            attackZone.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, i);
            yield return new WaitForSeconds(.02f);
        }
        attackZone.SetActive(false);
        attackZone.GetComponent<SpriteRenderer>().sprite = targetSprite;
        attackZone.GetComponent<SpriteRenderer>().color = defaultZoneColor;
        

    }

    /// <summary>
    /// Stops the basicAttack early. Currently used for when the target has moved out of range before the attack is finished
    /// Stops the basic attack in the event of an interruption?? (future case)
    /// </summary>
    public override void StopBasicAttacking()
    {
        animator.Play("Idle");
        base.StopBasicAttacking();
        animator.SetFloat("basicAttackSpeedMultiplier", basicAttackClipSpeedMultiplier);
        attackZone.SetActive(false);
        if (cracksCoroutine != null)
        {
            StopCoroutine(cracksCoroutine);
            cracksCoroutine = null;
        }
    }


}
