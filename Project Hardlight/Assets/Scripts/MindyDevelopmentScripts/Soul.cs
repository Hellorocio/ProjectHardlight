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
    public int level = 0;
    
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
        int bonusHealth = (int) ((level * SoulManager.Instance.flatHealthScale) + (level * SoulManager.Instance.percentHealthScale));
        return bonusHealth;
    }
}
