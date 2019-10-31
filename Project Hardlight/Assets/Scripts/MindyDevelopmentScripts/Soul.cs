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
    public int currentValue;
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

    /// <summary>
    /// Adds to the currentValue by amount (clamped to allightAttribute's base value)
    /// Checks level up
    /// </summary>
    /// <param name="index"></param>
    /// <returns>Amount actually added</returns>
    public int AddAllightValue (int index, int amount)
    {
        int fragmentsAdded = Mathf.Clamp(amount, 0, allightAttributes[index].baseValue);
        allightAttributes[index].currentValue += fragmentsAdded;

        if (CheckLevelUp())
        {
            LevelUp();
        }

        return fragmentsAdded;
    }

    public bool CheckLevelUp()
    {
        bool levelUp = true;
        foreach (AllightAttribute a in allightAttributes)
        {
            if (a.currentValue != a.baseValue)
            {
                levelUp = false;
                break;
            }
        }
        return levelUp;
    }

    public void LevelUp()
    {
        level++;

        foreach (AllightAttribute a in allightAttributes)
        {
            a.baseValue = (int)(a.baseValue * SoulManager.Instance.baseValueIncPerLevel);
        }
    }

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
            description += "Gives bonus mana regen speed\n";
        }

        return description.Substring(0, description.Length - 1);
    }

    /// <summary>
    /// Returns health boost at current level or testLevel if it is not -1
    /// </summary>
    /// <param name="testLevel"></param>
    /// <returns></returns>
    public float GetPercentHealthBoost(int testLevel = -1)
    {
        float boost = 0;
        float modifierLevel = level;
        if (testLevel != -1)
        {
            modifierLevel = testLevel;
        }

        if (statFocuses.Contains(StatFocusType.HEALTH))
        {
            boost += modifierLevel * SoulManager.Instance.percentHealthScale;
        }
        return boost;
    }

    public float GetFlatHealthBoost(int testLevel = -1)
    {
        float boost = 0;
        float modifierLevel = level;
        if (testLevel != -1)
        {
            modifierLevel = testLevel;
        }

        if (statFocuses.Contains(StatFocusType.HEALTH))
        {
            boost += modifierLevel * SoulManager.Instance.flatHealthScale;
        }
        return boost;
    }

    public float GetPercentAbilityBoost(int testLevel = -1)
    {
        float boost = 0;
        float modifierLevel = level;
        if (testLevel != -1)
        {
            modifierLevel = testLevel;
        }

        if (statFocuses.Contains(StatFocusType.ABILITY))
        {
            boost += modifierLevel * SoulManager.Instance.percentAbilityScale;
        }
        return boost;
    }

    public float GetFlatAbilityBoost(int testLevel = -1)
    {
        float boost = 0;
        float modifierLevel = level;
        if (testLevel != -1)
        {
            modifierLevel = testLevel;
        }

        if (statFocuses.Contains(StatFocusType.ABILITY))
        {
            boost += modifierLevel * SoulManager.Instance.flatAbilityScale;
        }
        return boost;
    }

    public float GetPercentAttackBoost(int testLevel = -1)
    {
        float boost = 0;
        float modifierLevel = level;
        if (testLevel != -1)
        {
            modifierLevel = testLevel;
        }

        if (statFocuses.Contains(StatFocusType.ATTACK))
        {
            boost += modifierLevel * SoulManager.Instance.percentAttackDamgeScale;
        }
        return boost;
    }

    public float GetFlatAttackBoost(int testLevel = -1)
    {
        float boost = 0;
        float modifierLevel = level;
        if (testLevel != -1)
        {
            modifierLevel = testLevel;
        }

        if (statFocuses.Contains(StatFocusType.ATTACK))
        {
            boost += modifierLevel * SoulManager.Instance.percentAttackDamgeScale;
        }
        return boost;
    }

    public float GetPercentAttackSpeedBoost(int testLevel = -1)
    {
        float boost = 0;
        float modifierLevel = level;
        if (testLevel != -1)
        {
            modifierLevel = testLevel;
        }

        if (statFocuses.Contains(StatFocusType.ATTACKSPEED))
        {
            boost += modifierLevel * SoulManager.Instance.percentAttackSpeedScale;
        }
        return boost;
    }

    public float GetFlatAttackSpeedBoost(int testLevel = -1)
    {
        float boost = 0;
        float modifierLevel = level;
        if (testLevel != -1)
        {
            modifierLevel = testLevel;
        }

        if (statFocuses.Contains(StatFocusType.ATTACKSPEED))
        {
            boost += modifierLevel * SoulManager.Instance.percentAttackSpeedScale;
        }
        return boost;
    }

    public int GetMaxHealthBonus(int baseMaxHealth)
    {
        float maxHealthBonus = 0;
        maxHealthBonus += baseMaxHealth * GetPercentHealthBoost();
        maxHealthBonus += GetFlatHealthBoost();
        return (int)maxHealthBonus;
    }

    public int GetAbilityBonus(int baseAbility)
    {
        float abilityBonus = 0;
        abilityBonus += baseAbility * GetPercentAbilityBoost();
        abilityBonus += GetFlatAbilityBoost();
        return (int)abilityBonus;
    }

    public int GetAttackBonus(int baseAttack)
    {
        float attackBonus = 0;
        attackBonus += baseAttack * GetPercentAttackBoost();
        attackBonus += GetFlatAttackBoost();
        return Mathf.CeilToInt(attackBonus);
    }

    public int GetAttackSpeedBonus (int baseAttackSpeed)
    {
        float attackSpeedBonus = 0;
        attackSpeedBonus += baseAttackSpeed * GetPercentAttackSpeedBoost();
        attackSpeedBonus += GetPercentAttackSpeedBoost();
        return Mathf.CeilToInt(attackSpeedBonus);
    }
}
