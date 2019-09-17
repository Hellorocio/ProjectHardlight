using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageBasicAttackAction : BasicAttackAction
{

    public GameObject mageBasicAttackPrefab;
    public GameObject spawnPoint;
    // Used for offsetting to match animation
    public float animationDelay;

    public void Start()
    {
    }

    public override void DoBasicAttack()
    {
        StartCoroutine("BasicAttackWithAnimationDelay");
    }
    
    IEnumerator BasicAttackWithAnimationDelay()
    {
        // TODO(mchi) scale animation delay to attack speed changes
        yield return new WaitForSeconds(animationDelay);
        
        GameObject mageBasicAttack = Instantiate(mageBasicAttackPrefab);
        mageBasicAttack.transform.position = spawnPoint.transform.position;
        mageBasicAttack.GetComponent<ProjectileMovement>().source = gameObject;

        if (GetComponent<Fighter>().currentTarget != null)
        {
            mageBasicAttack.GetComponent<ProjectileMovement>().target = GetComponent<Fighter>().currentTarget.gameObject;
        }
        yield break;
    }
}
