using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueBoxController : MonoBehaviour
{
    public float defaultDuration = 2.0f;
    public float duration;
    public GameObject background;
    public GameObject textMesh;

    IEnumerator disappearLoop;

    // Start is called before the first frame update
    void Start()
    {
        Show(false);
    }

    // Make some text appear in the bottom right for a couple seconds
    public void PopupDialogue(string text)
    {
        PopupDialogue(text, defaultDuration);
    }

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

    IEnumerator Disappear()
    {
        yield return new WaitForSeconds(duration);
        Show(false);
        disappearLoop = null;
    }

    private void Show(bool shouldShow)
    {
        background.SetActive(shouldShow);
        textMesh.SetActive(shouldShow);
    }
}