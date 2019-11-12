using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivateButton : MonoBehaviour
{
    public Sprite activatedButton;
    public Sprite deactivateButton;

    public bool buttonActive;

    private Image buttonImage;

    // Start is called before the first frame update
    void Start()
    {
        buttonImage = GetComponent<Image>();

        //init button sprite
        SetButtonActivation(buttonActive);
    }
    

    public void SetButtonActivation (bool active)
    {
        buttonActive = active;
        if (buttonActive)
        {
            buttonImage.sprite = activatedButton;
        }
        else
        {
            buttonImage.sprite = deactivateButton;
        }
    }
}
