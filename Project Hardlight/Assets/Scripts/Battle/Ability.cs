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
    
    public float damageScale = 0;
    public float baseEffectRange;
    public Targeting.Type targetingType;
    public Vector3 selectedPosition;
    public GameObject selectedTarget;

    [Header("Allight Values")]
    public int sunlight = 0;
    public int moonlight = 0;
    public int starlight = 0;

    public virtual bool StartTargeting()
    {
        Debug.Log("Default StartTargeting()");
        return true;
    }

    public virtual void StopTargeting()
    {
        Debug.Log("Default StopTargeting()");
    }

    // Checks for the souls attached
    public void Augment()
    {
        sunlight = GetComponent<Fighter>().sunlight;
        moonlight = GetComponent<Fighter>().moonlight;
        starlight = GetComponent<Fighter>().starlight;
    }

    public virtual bool DoAbility()
    {
        Debug.Log("Default DoAbility()");
        return true;
    }

    public virtual float GetDamage()
    {
        return damageScale*GetComponent<Fighter>().GetAbility();
    }

    public virtual float GetRange()
    {
        return baseEffectRange;
    }
}
