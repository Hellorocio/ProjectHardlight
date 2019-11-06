using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialVisibilityTrigger : MonoBehaviour
{
    public string activatePopup;
    public string tutorialPopupName;
    private bool activated;

    void Update()
    {
        if (GetComponent<Renderer>().isVisible && !activated && TutorialManager.Instance.GetCurrentTutorialPopupName() == activatePopup)
        {
            TutorialManager.Instance.ActivateTutorialPopup(tutorialPopupName);
            activated = true;
        }
    }
}
