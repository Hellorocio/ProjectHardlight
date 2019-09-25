﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulManager : Singleton<SoulManager>
{
    public List<Sprite> soulAppearances;

    public Soul GenerateSoul()
    {
        Soul soul = gameObject.AddComponent(typeof(Soul)) as Soul;

        // Give random appearance for now
        soul.appearance = soulAppearances[Random.Range(0, soulAppearances.Count)];

        soul.statFocuses = GetRandomStatFocuses();
        soul.allightAttributes = GetRandomAllightTypes();

        return soul;
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
        int allightNumber = Random.Range(0, 6);
        List<AllightAttribute> allightAttributes = new List<AllightAttribute>();

        switch (allightNumber)
        {
            case 0:
                allightAttributes.Add(new AllightAttribute(AllightType.SUNLIGHT, 20));
                break;
            case 1:
                allightAttributes.Add(new AllightAttribute(AllightType.MOONLIGHT, 20));
                break;
            case 2:
                allightAttributes.Add(new AllightAttribute(AllightType.STARLIGHT, 20));
                break;
            case 3:
                allightAttributes.Add(new AllightAttribute(AllightType.SUNLIGHT, 10));
                allightAttributes.Add(new AllightAttribute(AllightType.MOONLIGHT, 10));
                break;
            case 4:
                allightAttributes.Add(new AllightAttribute(AllightType.MOONLIGHT, 10));
                allightAttributes.Add(new AllightAttribute(AllightType.STARLIGHT, 10));
                break;
            case 5:
                allightAttributes.Add(new AllightAttribute(AllightType.STARLIGHT, 10));
                allightAttributes.Add(new AllightAttribute(AllightType.SUNLIGHT, 10));
                break;
        }

        return allightAttributes;
    }

}