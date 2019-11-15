using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatLevel : MonoBehaviour
{
    // TODO Can you implement this? @Amber, From Mindy
    
    // Note: Prebattle and Postbattle should only play once. The GameManager should track if these have already been played.
    // Lose dialogue is short and always plays.
    
    // Plays before the battle begins
    public TextAsset preBattleDialogue;
    // Plays after the battle ends and vessels win
    public TextAsset postBattleDialogue;
    // Plays if vessels lose
    public TextAsset loseDialogue;
}
