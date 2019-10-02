using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Keep track of state and anything related to the tutorial
public class TutorialManager : Singleton<TutorialManager>
{

    public bool tutorialEnabled = true;

    public bool inTutorialBattle = false;
}
