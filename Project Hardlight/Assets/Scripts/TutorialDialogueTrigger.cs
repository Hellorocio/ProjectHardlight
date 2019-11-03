using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDialogueTrigger : MonoBehaviour
{
    public string tutorialPopupName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            TutorialManager.Instance.ActivateTutorialPopup(tutorialPopupName);
        }
    }
}
