using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Attackable attackable;
    public Text healthText;
    public Image healthBar;

    float maxHealth;
    float maxBarWidth;

    private void OnEnable()
    {
        maxBarWidth = healthBar.GetComponent<RectTransform>().sizeDelta.x;

        if (attackable != null)
        {
            maxHealth = attackable.GetMaxHealth();
            UpdateHealthBar(maxHealth);


            attackable.OnHealthChanged += UpdateHealthBar;
        }
        else
        {
            Debug.Log("WARNING: Health Bar doesn't have an attackable set to track'");
        }
    }

    private void OnDisable()
    {
        if (attackable != null)
        {
            attackable.OnHealthChanged -= UpdateHealthBar;
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
