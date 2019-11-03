using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenericHealthBar : MonoBehaviour
{
    public Text healthText;
    public Image healthBar;

    GenericMonsterAI monster;
    float maxHealth;
    float maxBarWidth;

    private void OnEnable()
    {
        maxBarWidth = healthBar.GetComponent<RectTransform>().sizeDelta.x;



        monster = transform.parent.parent.GetComponent<GenericMonsterAI>();
        if (monster != null)
        {
            maxHealth = monster.maxHealth;
            UpdateHealthBar(maxHealth);


            monster.OnHealthChanged += UpdateHealthBar;
        }


    }

    private void OnDisable()
    {
        if (monster != null)
        {
            monster.OnHealthChanged -= UpdateHealthBar;
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
