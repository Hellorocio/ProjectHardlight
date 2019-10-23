using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenericHealthBar : MonoBehaviour
{
    public Text healthText;
    public Image healthBar;

    GenericMeleeMonster fighter;
    GenericRangedMonster fighter2;
    float maxHealth;
    float maxBarWidth;

    private void OnEnable()
    {
        maxBarWidth = healthBar.GetComponent<RectTransform>().sizeDelta.x;



        fighter = transform.parent.parent.GetComponent<GenericMeleeMonster>();
        if (fighter != null)
        {
            maxHealth = fighter.maxHealth;
            UpdateHealthBar(maxHealth);


            fighter.OnHealthChanged += UpdateHealthBar;
        }

        fighter2 = transform.parent.parent.GetComponent<GenericRangedMonster>();
        if (fighter2 != null)
        {
            maxHealth = fighter2.maxHealth;
            UpdateHealthBar(maxHealth);


            fighter2.OnHealthChanged += UpdateHealthBar;
        }
    }

    private void OnDisable()
    {
        if (fighter != null)
        {
            fighter.OnHealthChanged -= UpdateHealthBar;
        }

        if (fighter2 != null)
        {
            fighter2.OnHealthChanged -= UpdateHealthBar;
        }
    }

    //update text and health bar image
    void UpdateHealthBar(float health)
    {
        healthText.text = health + "/" + maxHealth;

        Vector2 sizeDelta = healthBar.GetComponent<RectTransform>().sizeDelta;
        sizeDelta.x = (health * maxBarWidth) / maxHealth;
        healthBar.GetComponent<RectTransform>().sizeDelta = sizeDelta;
    }
}
