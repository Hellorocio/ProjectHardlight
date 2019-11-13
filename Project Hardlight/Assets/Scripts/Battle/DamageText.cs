using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    public GameObject damageTextPrefab;

    private Color damageColor = Color.red;
    private Color healingColor = Color.cyan;

    private GameObject[] damageTextPool;
    private IEnumerator[] damageTextTimers;
    private int poolSize = 5;
    private int poolNum;

    public Attackable attackable;
    private float oldHealth;     // keeps track of health to tell what damage has been taken
                                 // we may want to change this later but I didn't want to clutter up fighter with more events

    private void Start()
    {
        // make a pool for these so we aren't instantiating and destroying things all the time

        damageTextPool = new GameObject[poolSize];
        damageTextTimers = new IEnumerator[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            damageTextPool[i] = Instantiate(damageTextPrefab, transform);
            damageTextPool[i].transform.position = transform.position;
        }
    }

    private void OnEnable()
    {
        oldHealth = attackable.GetMaxHealth();
        attackable.OnHealthChanged += SetDamageText;
    }

    private void OnDisable()
    {
        if (attackable != null)
        {
            attackable.OnHealthChanged -= SetDamageText;
        }
    }

    /// <summary>
    /// Calculates what damage text to display based on change in health
    /// Kind of dumb, I know, but I didn't want to clutter up fighter with more events so
    /// </summary>
    /// <param name="damage"></param>
    public void SetDamageText(float newHealth)
    {
        float damage = newHealth - oldHealth;
        oldHealth = newHealth;

        string damageString = ((int)damage).ToString();

        // allocate a damageText from the pool (just disable previous one if it hasn't finished yet)
        if (damageTextTimers[poolNum] != null)
        {
            StopCoroutine(damageTextTimers[poolNum]);
            damageTextPool[poolNum].SetActive(false);
        }
        damageTextTimers[poolNum] = DisableAfterAnimation(poolNum);

        // change color of text depending on damage/healing
        if (damage < 0)
        {
            damageTextPool[poolNum].GetComponent<TextMeshProUGUI>().color = damageColor;
        }
        else if (damage > 0)
        {
            
            damageTextPool[poolNum].GetComponent<TextMeshProUGUI>().color = healingColor;
            damageString = "+" + damageString;
        }

        damageTextPool[poolNum].GetComponent<TextMeshProUGUI>().text = damageString;
        StartCoroutine(damageTextTimers[poolNum]);
        poolNum++;

        if (poolNum >= poolSize)
        {
            poolNum = 0;
        }
    }

    IEnumerator DisableAfterAnimation (int num)
    {
        damageTextPool[num].SetActive(true);
        yield return new WaitForSeconds(0.9f);
        damageTextPool[num].SetActive(false);
    }
}
