using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewTargetAbility: Ability
{
    public float baseEffectRange;

    public GameObject rangeIndicatorPrefab;
    public GameObject rangeIndicator;

    public GameObject attackTargetUnit;

    private bool targeting;

    private void Update()
    {
        if (targeting)
        {
            // Update positions
            rangeIndicator.transform.position = gameObject.transform.position;
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }


    public override bool StartTargeting()
    {
        targeting = true;

        rangeIndicator = Instantiate(rangeIndicatorPrefab);
        rangeIndicator.name = "Range";
        rangeIndicator.transform.localScale *= GetRange();

        return true;
    }

    public override void StopTargeting()
    {
        Destroy(rangeIndicator);
        targeting = false;
    }

    public override bool DoAbility()
    {
        if (selectedTarget != null)
        {
            Debug.Log("Set New Target Ability called");
            Fighter selectedFighter = selectedTarget.GetComponent<Fighter>();
            if (selectedFighter != null)
            {
                // Set Issued Current Target handles the healer check
                gameObject.GetComponent<FighterAttack>().SetIssuedCurrentTarget(selectedFighter);


                return true;
            }
        }
        return false;
    }

    public float GetRange()
    {
        return baseEffectRange;
    }
}
