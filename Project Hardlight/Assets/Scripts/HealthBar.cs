using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Text healthText;
    public Image healthBar;

    Fighter fighter;
    float maxHealth;
    float maxBarWidth;

    private void OnEnable()
    {
        maxBarWidth = healthBar.GetComponent<RectTransform>().sizeDelta.x;
        


        fighter = transform.parent.parent.GetComponent<Fighter>();
        if (fighter != null)
        {
            maxHealth = fighter.maxHealth;
            UpdateHealthBar(maxHealth);


            fighter.OnHealthChanged += UpdateHealthBar;
        }
    }

    private void OnDisable()
    {
        if (fighter != null)
        {
            fighter.OnHealthChanged -= UpdateHealthBar;
        }
    }

    //update text and health bar image
    void UpdateHealthBar (float health)
    {
        healthText.text = health + "/" + maxHealth;

        Vector2 sizeDelta = healthBar.GetComponent<RectTransform>().sizeDelta;
        sizeDelta.x = (health * maxBarWidth) / maxHealth;
        healthBar.GetComponent<RectTransform>().sizeDelta = sizeDelta;
    }
}
