using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulManager : Singleton<SoulManager>
{
    public List<Sprite> soulAppearances;
    public List<Sprite> allightAppearances; //[0] = sunlight, [1] = moonlight, [2] = starlight

    public GameObject iconPrefab;
    public Sprite sunlightIcon;
    public Sprite moonlightIcon;
    public Sprite starlightIcon;

    public Sprite healthIcon;
    public Sprite abilityIcon;
    public Sprite attackDamageIcon;
    public Sprite attackSpeedIcon;

    // Adjust per level gains
    public float flatHealthScale;
    public float percentHealthScale;

    public float flatAbilityScale;
    public float percentAbilityScale;

    public float flatAttackDamageScale;
    public float percentAttackDamgeScale;

    public float flatAttackSpeedScale;
    public float percentAttackSpeedScale;

    public float baseValueIncPerLevel;
    
    // Base values
    public int oneAllightBaseValue = 100;
    public int twoAllightBaseValue = 40;

    /// <summary>
    /// Generates a random soul
    /// Note that this does not add the soul to GameManager's soul list, so we must refresh the loadoutUI elsewhere
    /// </summary>
    /// <returns></returns>
    public Soul GenerateSoul()
    {
        Soul soul = gameObject.AddComponent(typeof(Soul)) as Soul;

        // Give random appearance for now
        soul.appearance = soulAppearances[Random.Range(0, soulAppearances.Count)];

        soul.statFocuses = GetRandomStatFocuses();
        soul.allightAttributes = GetRandomAllightTypes();
        
        return soul;
    }

    /// <summary>
    /// Generates 3 souls with 3 different allight types and 4 different stat focus types (last allight is rng)
    /// </summary>
    /// <returns></returns>
    public Soul[] GenerateDiverseSouls ()
    {
        Soul[] souls = new Soul[4];
        for (int i = 0; i < 4; i++)
        {
            Soul soul = gameObject.AddComponent(typeof(Soul)) as Soul;
            soul.appearance = soulAppearances[Random.Range(0, soulAppearances.Count)];

            List<StatFocusType> statFocuses = new List<StatFocusType>();
            statFocuses.Add((StatFocusType)i);
            soul.statFocuses = statFocuses;

            List<AllightAttribute> allightAttributes = new List<AllightAttribute>();            
            if (i < 3)
            {
                allightAttributes.Add(new AllightAttribute((AllightType)i, oneAllightBaseValue));
            }
            else
            {
                allightAttributes = GetRandomAllightTypes();
            }
            soul.allightAttributes = allightAttributes;

            souls[i] = soul;
        }

        return souls;
    }

    private List<StatFocusType> GetRandomStatFocuses()
    {
        // Give random combo of stat focuses
        int statFocusNumber = Random.Range(0, 9);
        List<StatFocusType> statFocuses = new List<StatFocusType>();

        switch (statFocusNumber)
        {
            case 0:
                statFocuses.Add(StatFocusType.HEALTH);
                break;
            case 1:
                statFocuses.Add(StatFocusType.ABILITY);
                break;
            case 2:
                statFocuses.Add(StatFocusType.ATTACK);
                break;
            case 3:
                statFocuses.Add(StatFocusType.ATTACKSPEED);
                break;
            case 4:
                statFocuses.Add(StatFocusType.HEALTH);
                statFocuses.Add(StatFocusType.ABILITY);
                break;
            case 5:
                statFocuses.Add(StatFocusType.HEALTH);
                statFocuses.Add(StatFocusType.ATTACK);
                break;
            case 6:
                statFocuses.Add(StatFocusType.HEALTH);
                statFocuses.Add(StatFocusType.ATTACKSPEED);
                break;
            case 7:
                statFocuses.Add(StatFocusType.ABILITY);
                statFocuses.Add(StatFocusType.ATTACK);
                break;
            case 8:
                statFocuses.Add(StatFocusType.ABILITY);
                statFocuses.Add(StatFocusType.ATTACKSPEED);
                break;
            case 9:
                statFocuses.Add(StatFocusType.ATTACK);
                statFocuses.Add(StatFocusType.ATTACKSPEED);
                break;
        }

        return statFocuses;
    }

    
    private List<AllightAttribute> GetRandomAllightTypes()
    {
        // Give random combo of stat focuses
        int allightNumber = Random.Range(0, 3); //6);
        List<AllightAttribute> allightAttributes = new List<AllightAttribute>();

        switch (allightNumber)
        {
            case 0:
                allightAttributes.Add(new AllightAttribute(AllightType.SUNLIGHT, oneAllightBaseValue));
                break;
            case 1:
                allightAttributes.Add(new AllightAttribute(AllightType.MOONLIGHT, oneAllightBaseValue));
                break;
            case 2:
                allightAttributes.Add(new AllightAttribute(AllightType.STARLIGHT, oneAllightBaseValue));
                break;
            /*
            case 3:
                allightAttributes.Add(new AllightAttribute(AllightType.SUNLIGHT, twoAllightBaseValue));
                allightAttributes.Add(new AllightAttribute(AllightType.MOONLIGHT, twoAllightBaseValue));
                break;
            case 4:
                allightAttributes.Add(new AllightAttribute(AllightType.MOONLIGHT, twoAllightBaseValue));
                allightAttributes.Add(new AllightAttribute(AllightType.STARLIGHT, twoAllightBaseValue));
                break;
            case 5:
                allightAttributes.Add(new AllightAttribute(AllightType.STARLIGHT, twoAllightBaseValue));
                allightAttributes.Add(new AllightAttribute(AllightType.SUNLIGHT, twoAllightBaseValue));
                break;
                */
        }

        return allightAttributes;
    }
}
