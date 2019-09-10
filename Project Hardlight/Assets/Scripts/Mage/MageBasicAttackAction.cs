using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageBasicAttackAction : BasicAttackAction
{

    public GameObject mageBasicAttackPrefab;


    public void Start()
    {
    }

    public override void DoBasicAttack()
    {
        GameObject mageBasicAttack = Instantiate(mageBasicAttackPrefab);
        mageBasicAttack.transform.position = transform.position;
        mageBasicAttack.GetComponent<ProjectileMovement>().source = gameObject;
        mageBasicAttack.GetComponent<ProjectileMovement>().target = GetComponent<Fighter>().currentTarget.gameObject;
    }
}
