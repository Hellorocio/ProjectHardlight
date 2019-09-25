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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
