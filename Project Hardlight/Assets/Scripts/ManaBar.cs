using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour
{
    public Text manaText;
    public Image manaBar;

    Fighter fighter;
    float maxMana;
    float maxBarWidth;

    private void OnEnable()
    {
        //only set once
        if (maxBarWidth == 0)
        {
            maxBarWidth = manaBar.GetComponent<RectTransform>().sizeDelta.x;
        }

        fighter = transform.parent.parent.GetComponent<Fighter>();
        if (fighter != null)
        {
            maxMana = fighter.GetMaxMana();
            UpdateManaBar(0);


            fighter.OnManaChanged += UpdateManaBar;
        }
    }

    private void OnDisable()
    {
        if (fighter != null)
        {
            fighter.OnManaChanged -= UpdateManaBar;
        }
    }

    //update text and health bar image
    void UpdateManaBar(float mana)
    {
        manaText.text = mana + "/" + maxMana;

        Vector2 sizeDelta = manaBar.GetComponent<RectTransform>().sizeDelta;
        sizeDelta.x = (mana * maxBarWidth) / maxMana;
        manaBar.GetComponent<RectTransform>().sizeDelta = sizeDelta;
    }
}
