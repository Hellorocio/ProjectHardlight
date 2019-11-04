using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueBoxController : MonoBehaviour
{
    public float duration;
    public GameObject background;
    public GameObject textMesh;
    Animator animator;

    IEnumerator disappearLoop;

    // Start is called before the first frame update
    void Start()
    {
        Show(false);
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Displays popup with no timer
    /// </summary>
    /// <param name="text"></param>
    public void PopupDialogue(string text)
    {
        Show(true);
        textMesh.GetComponent<TextMeshProUGUI>().text = text;
    }

    /// <summary>
    /// Displays popup for time seconds
    /// </summary>
    /// <param name="text"></param>
    /// <param name="time"></param>
    public void PopupDialogue(string text, float time)
    {
        duration = time;
        Show(true);
        if (disappearLoop != null)
        {
            StopCoroutine(disappearLoop);
            disappearLoop = null;
        }
        textMesh.GetComponent<TextMeshProUGUI>().text = text;
        disappearLoop = Disappear();
        StartCoroutine(disappearLoop);
    }

    public void CancelPopupDialogue ()
    {
        StopAllCoroutines();
        disappearLoop = null;
        Show(false);
    }

    IEnumerator Disappear()
    {
        yield return new WaitForSeconds(duration);
        Show(false);
    }

    private void Show(bool shouldShow)
    {
        background.SetActive(shouldShow);
        textMesh.SetActive(shouldShow);

        if (shouldShow && animator != null)
        {
            animator.Play("PopupStart");
        }
    }
}