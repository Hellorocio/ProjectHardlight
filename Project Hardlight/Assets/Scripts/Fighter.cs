using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fighter : MonoBehaviour
{
    public GameObject healthUI;
    public int baseHealth;
    public int baseSpeed;
    public int baseDamage;
    public bool isPlayer;

    private const float meleeRange = 1f;

    private int health;
    private int speed;
    private int damage;

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
                    if ((transform.position - attackObj.transform.position).sqrMagnitude > meleeRange)
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

    IEnumerator AttackTimer()
    {
        while (attackObj.activeSelf)
        {
            attackObj.GetComponent<Fighter>().Attack(damage);
            yield return new WaitForSeconds(1);
        }
        SetAttackObj();
        currentState = State.Moving;
    }

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
