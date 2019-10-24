using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatFocusType {
    HEALTH,
    ABILITY,
    ATTACK,
    ATTACKSPEED
}

public enum AllightType
{
    SUNLIGHT,
    MOONLIGHT,
    STARLIGHT
}

public class AllightAttribute
{
    public AllightType allightType;
    public int baseValue;

    public AllightAttribute(AllightType type, int value)
    {
        allightType = type;
        baseValue = value;
    }
}

public class Soul : MonoBehaviour
{
    public Sprite appearance;
    public string title = "Some Soul";
    public int level = 1;
    
    public List<StatFocusType> statFocuses;
    public List<AllightAttribute> allightAttributes; 

    public string GetDescription()
    {
        string description = "";
        if (statFocuses.Contains(StatFocusType.HEALTH))
        {
            description += "Gives bonus health\n";
        }
        if (statFocuses.Contains(StatFocusType.ABILITY))
        {
            description += "Gives bonus ability power\n";
        }
        if (statFocuses.Contains(StatFocusType.ATTACK))
        {
            description += "Gives bonus attack damage\n";
        }
        if (statFocuses.Contains(StatFocusType.ATTACKSPEED))
        {
            description += "Gives bonus attack speed\n";
        }

        return description.Substring(0, description.Length - 1);
    }

    public int GetMaxHealthBonus(int baseMaxHealth)
    {
        float maxHealthBonus = 0;
        if (statFocuses.Contains(StatFocusType.HEALTH))
        {
            maxHealthBonus += baseMaxHealth * level * SoulManager.Instance.percentHealthScale;
            maxHealthBonus += level * SoulManager.Instance.flatHealthScale;
        }
        return (int) maxHealthBonus;
    }

    public int GetAbilityBonus(int baseAbility)
    {
        float abilityBonus = 0;
        if (statFocuses.Contains(StatFocusType.ABILITY))
        {
            abilityBonus += baseAbility * level * SoulManager.Instance.percentAbilityScale;
            abilityBonus += level * SoulManager.Instance.flatAbilityScale;
        }
        return (int)abilityBonus;
    }

    public int GetAttackBonus(int baseAttack)
    {
        float attackBonus = 0;
        if (statFocuses.Contains(StatFocusType.ATTACK))
        {
            attackBonus += baseAttack * level * SoulManager.Instance.percentAttackDamgeScale;
            attackBonus += level * SoulManager.Instance.flatAttackDamageScale;
        }
        return (int)attackBonus;
    }

    public int GetAttackSpeedBonus (int baseAttackSpeed)
    {
        float attackSpeedBonus = 0;
        if (statFocuses.Contains(StatFocusType.ATTACKSPEED))
        {
            attackSpeedBonus += baseAttackSpeed * level * SoulManager.Instance.percentAttackSpeedScale;
            attackSpeedBonus += level * SoulManager.Instance.flatAttackSpeedScale;
        }
        return (int)attackSpeedBonus;
    }
}
