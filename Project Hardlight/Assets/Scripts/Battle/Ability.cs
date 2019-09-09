using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour
{
    public string abilityName;
    public string abilityDescription;

    public Targeting.Type targetingType;
    public Vector3 selectedPosition;
    public GameObject selectedTarget;

    public virtual void StartTargeting()
    {
        Debug.Log("Default StartTargeting()");
    }

    public virtual void DoAbility()
    {
        Debug.Log("Default DoAbility()");
    }
}
