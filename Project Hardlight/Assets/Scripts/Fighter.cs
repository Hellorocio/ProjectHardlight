using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fighter : MonoBehaviour
{
    public GameObject healthUI;
    public int baseHealth = 5;
    public int baseSpeed = 1;
    public int baseDamage = 1;
    public int baseMana = 1;
    public float range = 1;
    public bool isPlayer;

    private int health;
    private int speed;
    private int damage;
    private int mana;

    private enum State {Moving, Attacking};
    private State currentState = State.Moving;

    private GameObject attackParent;
    private GameObject attackObj;


    // Start is called before the first frame update
    void Start()
    {
        health = baseHealth;
        speed = baseSpeed;
        damage = baseDamage;
        mana = baseMana;

        if (isPlayer)
        {
            attackParent = GameObject.Find("Enemies");
        }
        else
        {
            attackParent = GameObject.Find("Players");
        }

        SetHealthUI();
        SetAttackObj();
    }

    // Update is called once per frame
    void Update()
    {
        if (attackObj != null)
        {
            switch (currentState)
            {
                case State.Moving:
                {
                    if ((transform.position - attackObj.transform.position).sqrMagnitude > range)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, attackObj.transform.position, speed * Time.deltaTime);
                    }
                    else
                    {
                        currentState = State.Attacking;
                        StartCoroutine(AttackTimer());
                    }
                    break;
                }
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// Calls attack on attackObj every second until it is defeated
    /// </summary>
    /// <returns></returns>
    IEnumerator AttackTimer()
    {
        while (attackObj.activeSelf)
        {
            attackObj.GetComponent<Fighter>().Attack(damage);
            mana++;
            yield return new WaitForSeconds(1);
        }
        SetAttackObj();
        currentState = State.Moving;
    }

    /// <summary>
    /// Called by other fighters when they attack this one
    /// </summary>
    /// <param name="dmg"></param>
    public void Attack (int dmg)
    {
        health -= dmg;

        if (health == 0)
        {
            gameObject.SetActive(false);
        }

        SetHealthUI();
    }

    /// <summary>
    /// Searches enemies or players gameObject for the closest thing to attack and sets attackObj
    /// Sets attackObj to null if there are no more things to attack
    /// </summary>
    void SetAttackObj()
    {
        Fighter[] attackObjs = attackParent.GetComponentsInChildren<Fighter>();
        float minDist = 1000f;
        GameObject tempAttackObj = null;

        for (int i = 0; i < attackObjs.Length; i++)
        {
            if (attackObjs[i].gameObject.activeSelf)
            {
                float dist = (transform.position - attackObjs[i].transform.position).sqrMagnitude;
                if (dist < minDist)
                {
                    minDist = dist;
                    tempAttackObj = attackObjs[i].gameObject;
                }
            }
        }
        attackObj = tempAttackObj;
    }

    /// <summary>
    /// Right now this sets the HPText, probably change later to a health bar
    /// </summary>
    void SetHealthUI()
    {
        healthUI.GetComponent<Text>().text = health.ToString();
    }
}
