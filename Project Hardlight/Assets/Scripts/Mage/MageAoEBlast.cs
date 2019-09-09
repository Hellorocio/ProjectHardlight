using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageAoEBlast : Ability
{
    public float baseEffectRange;
    public float baseEffectRadius;

    public GameObject rangeIndicatorPrefab;
    public GameObject radiusIndicatorPrefab;

    public GameObject rangeIndicator;
    public GameObject radiusIndicator;

    private bool targeting;

    public void Start()
    {
        targeting = false;
    }

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
        rangeIndicator.transform.localScale *= 2 * GetRange();

        radiusIndicator = Instantiate(radiusIndicatorPrefab);
        radiusIndicator.name = "Radius";
        radiusIndicator.transform.localScale *= 2 * GetRadius();

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
        Debug.Log("MAGE PEW PEW");

        return true;
    }

    public float GetRange()
    {
        return baseEffectRange;
    }

    public float GetRadius()
    {
        return baseEffectRadius;
    }
}
