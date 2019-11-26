using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivateButton : MonoBehaviour
{
    public Sprite activatedButton;
    public Sprite deactivateButton;

    public bool useActivateColor = false;
    public Color inactiveColor = Color.white;
    public Color activateColor = Color.white;

    [Tooltip("Animator must be attached with active and inactive states")]
    public bool animateButton;

    public bool buttonActive;

    private Image buttonImage;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        buttonImage = GetComponent<Image>();
        animator = GetComponent<Animator>();

        //init button sprite
        SetButtonActivation(buttonActive);
    }    

    public void SetButtonActivation (bool active)
    {
        buttonActive = active;
        if (buttonImage == null || animator == null)
        {
            buttonImage = GetComponent<Image>();
            animator = GetComponent<Animator>();
        }
        
        if (buttonActive)
        {
            buttonImage.sprite = activatedButton;

            if (useActivateColor)
            {
                buttonImage.color = activateColor;
            }

            if (animateButton && animator != null)
            {
                animator.Play("Active");
            }
        }
        else
        {
            buttonImage.sprite = deactivateButton;

            if (useActivateColor)
            {
                buttonImage.color = inactiveColor;
            }

            if (animateButton && animator != null)
            {
                animator.Play("Inactive");
            }
        }
    }
}
