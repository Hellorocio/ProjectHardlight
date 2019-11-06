using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenericHealthBar : MonoBehaviour
{
    public Attackable attackable;
    public Text healthText;
    public Image healthBar;

    float maxHealth;
    float maxBarWidth;

    private void OnEnable()
    {
        maxBarWidth = healthBar.GetComponent<RectTransform>().sizeDelta.x;

        
        maxHealth = attackable.maxHealth;
        UpdateHealthBar(maxHealth);


        attackable.OnHealthChanged += UpdateHealthBar;


    }

    private void OnDisable()
    {
        attackable.OnHealthChanged -= UpdateHealthBar;


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
