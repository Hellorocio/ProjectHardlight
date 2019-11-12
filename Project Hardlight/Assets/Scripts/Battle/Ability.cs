using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbilityAugment
{
    public string augmentTitle;
    public string augmentDescription;
}

public class Ability : MonoBehaviour
{
    public string abilityName;
    public string abilityDescription;
    public Sprite abilityIcon;

    public AbilityAugment sunlightAugment;
    public AbilityAugment moonlightAugment;
    public AbilityAugment starlightAugment;

    public AudioClip sfx;
    
    public int baseDamage = 0;
    public float baseEffectRange;
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
        return baseDamage;
    }

    public virtual float GetRange()
    {
        return baseEffectRange;
    }
}
