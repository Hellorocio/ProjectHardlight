using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Battle/Vessel", order = 1)]
public class VesselData : ScriptableObject
{
    public Sprite appearance;

    public string vesselName = "Tony Slimeslayer";
    public string title = "Basic Fighter";

    public string description;

    public int baseHealth = 100;
    public int baseMana = 100;
    public int baseMovementSpeed = 100;

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
