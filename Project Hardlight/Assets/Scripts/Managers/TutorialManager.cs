using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

// Keep track of state and things related to the tutorial, like events
// Tutorial dialogue is all handled in GameManager and BattleManager and Fighter, which uses TutorialManager to check status
public class TutorialManager : Singleton<TutorialManager>
{

    public bool tutorialEnabled = true;

    public bool inTutorialBattle = false;
    public bool usedAbility = false;

}
