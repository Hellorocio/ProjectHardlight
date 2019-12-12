using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCompletion : MonoBehaviour
{
    public VesselCompletion merrha;
    public VesselCompletion nero;
    public VesselCompletion taurin;
    
    public void SetWin(string vesselName, Difficulty difficulty)
    {
        if (vesselName == "Merrha")
        {
            merrha.DifficultyWin(difficulty);
        } else if (vesselName == "Nero")
        {
            nero.DifficultyWin(difficulty);
        } else if (vesselName == "Taurin")
        {
            taurin.DifficultyWin(difficulty);
        }
    }
}
