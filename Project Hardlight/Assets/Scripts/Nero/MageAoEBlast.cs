using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageAoEBlast : Ability
{

    [Header("Base Stats")]
    public float baseEffectRadius;

    public GameObject rangeIndicatorPrefab;
    public GameObject radiusIndicatorPrefab;

    // Indicator prefabs
    [Header("Indicators")]
    public GameObject lightBlastPrefab;

    [Header("Sunlight Augments")]
    public GameObject dotSpotPrefab;
    public float spotDuration;
    public float dpsScale;

    [Header("Moonlight Augments")]
    public Buff disableMonsterBuff;
    public float disableDurationScale;
    
    [Header("Starlight Augments")]

    [Header("Donut touch")]
    // Prefab instances
    public GameObject rangeIndicator;
    public GameObject radiusIndicator;
    public GameObject pointRef;

    private bool targeting;
   
    public void Update()
    {
        if (targeting)
        {
            // Update positions
            rangeIndicator.transform.position = gameObject.transform.position;
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            radiusIndicator.transform.position = new Vector3(mousePos.x, mousePos.y, radiusIndicator.transform.position.z);
        }
    }

    public override bool StartTargeting()
    {
        targeting = true;

        rangeIndicator = Instantiate(rangeIndicatorPrefab);
        rangeIndicator.name = "Range";
        rangeIndicator.transform.localScale *=  GetRange();

        radiusIndicator = Instantiate(radiusIndicatorPrefab);
        radiusIndicator.name = "Radius";
        radiusIndicator.transform.localScale *= GetRadius();

        return true;
    }

    public override void StopTargeting()
    {
        targeting = false;

        Destroy(rangeIndicator);
        Destroy(radiusIndicator);
    }

    public override bool DoAbility()
    {
        Augment();

        // Check that selectedPosition (set by BM) is in range
        if (Vector2.Distance(selectedPosition, gameObject.transform.position) < GetRange())
        {
            // Hit enemies
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(selectedPosition, GetRadius());
            foreach (Collider2D collider in hitColliders)
            {
                Attackable attackable = collider.gameObject.GetComponent<Attackable>();
                if (attackable != null)
                {
                    if (attackable.team == CombatInfo.Team.Enemy)
                    {
                        attackable.TakeDamage(GetDamage());

                        // Moonlight disable
                        if (moonlight > 0)
                        {
                            disableMonsterBuff.buffDuration = moonlight*disableDurationScale;
                            attackable.AddBuff(disableMonsterBuff);
                        }
                    }
                }
                
            }

            //display boom!
            GameObject boom = Instantiate(lightBlastPrefab);
            Vector3 boomPos = selectedPosition;
            boomPos.z = 2;
            boom.transform.position = boomPos;

            // Sunlight DoT spot
            if (sunlight > 0)
            {
                GameObject dotSpot = Instantiate(dotSpotPrefab);
                // Set spot to expire
                dotSpot.GetComponent<DoTSpot>().duration = spotDuration;
                // Set DoT debuff stats
                dotSpot.GetComponent<DoTBuff>().damagePerTick = sunlight*dpsScale;
                dotSpot.transform.position = new Vector3(selectedPosition.x, selectedPosition.y, dotSpot.transform.position.z);
            }
            if (gameObject.GetComponent<Fighter>().anim.HasState(0, Animator.StringToHash("Ability1")))
            {
                //Debug.Log("Ability1 anim is played");
                gameObject.GetComponent<Fighter>().anim.Play("Ability1");
            }
            return true;
        }
        else
        {
            //Debug.Log("AoE blast out of range");
            
            Vector3 rangePoint = Vector3.MoveTowards(transform.position, selectedPosition, GetRange());
            float moveDist = Vector3.Distance(rangePoint, selectedPosition);
            Vector3 InRangePoint = Vector3.MoveTowards(transform.position, selectedPosition, moveDist);
            GameObject refObj = Instantiate(pointRef);
            refObj.transform.position = rangePoint;
            GameObject newMoveLoc = Instantiate(GameObject.Find("BattleManager").GetComponent<BattleManager>().moveLoc);
            newMoveLoc.SetActive(true);
            newMoveLoc.transform.position = new Vector3(InRangePoint.x, InRangePoint.y, 2);

            //init line
            LineRenderer line = newMoveLoc.GetComponentInChildren<LineRenderer>();
            line.positionCount = 2;
            line.SetPosition(0, newMoveLoc.transform.position);
            line.SetPosition(1, selectedPosition);
            gameObject.GetComponent<FighterMove>().StartMovingCommandHandle(newMoveLoc.transform);
            
            return false;
        }
        return true;
    }

    public float GetRadius()
    {
        return baseEffectRadius;
    }
}
