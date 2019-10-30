using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PortraitHealthBar : MonoBehaviour
{
    public TextMeshProUGUI healthText;
    public Image healthBar;
    public bool isDead;

    Fighter fighter;
    float maxHealth;
    public float maxBarWidth;


    public void InitHero(Fighter f)
    {
        fighter = f;
        if (fighter != null)
        {
            maxHealth = fighter.GetMaxHealth();
            UpdateHealthBar(maxHealth);


            fighter.OnHealthChanged += UpdateHealthBar;
        }
    }

    private void OnEnable()
    {
        //maxBarWidth = healthBar.GetComponent<RectTransform>().sizeDelta.x;



        //fighter = transform.parent.parent.GetComponent<Fighter>();
        if (fighter != null)
        {
            maxHealth = fighter.GetMaxHealth();
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
    void UpdateHealthBar(float health)
    {
        healthText.text = health + "/" + maxHealth;
        isDead = health <= 0;
        Vector2 sizeDelta = healthBar.GetComponent<RectTransform>().sizeDelta;
        sizeDelta.x = (health * maxBarWidth) / maxHealth;
        healthBar.GetComponent<RectTransform>().sizeDelta = sizeDelta;
    }
}
