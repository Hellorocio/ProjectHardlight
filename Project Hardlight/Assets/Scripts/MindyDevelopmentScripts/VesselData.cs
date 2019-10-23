using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VesselData : MonoBehaviour
{
    public Sprite appearance;

    public string vesselName = "Tony Slimeslayer";
    public string title = "Basic Fighter";

    public string description;

    public int baseHealth = 100;
    public int baseMana = 100;
    public int baseAbility = 100;
    public int baseMovementSpeed = 100;

    // TODO Make fighters use this
    public MonoBehaviour basicAttack;

    public List<MonoBehaviour> abilities;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
