using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDialogueTrigger : MonoBehaviour
{
    public string tutorialPopupName;
    private bool activated;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !activated)
        {
            TutorialManager.Instance.ActivateTutorialPopup(tutorialPopupName);
            activated = true;
        }
    }
}
