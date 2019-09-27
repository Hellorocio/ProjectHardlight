using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour
{
    public string abilityName;
    public string abilityDescription;

    public int baseDamage = 0;

    public Targeting.Type targetingType;
    public Vector3 selectedPosition;
    public GameObject selectedTarget;

    public virtual bool StartTargeting()
    {
        Debug.Log("Default StartTargeting()");
        return true;
    }

    public virtual void StopTargeting()
    {
        Debug.Log("Default StopTargeting()");
    }

    public virtual bool DoAbility()
    {
        Debug.Log("Default DoAbility()");
        return true;
    }

    public virtual float GetDamage()
    {
        return GetComponent<Fighter>().GetDamage(baseDamage);
    }
}
