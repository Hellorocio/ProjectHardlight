using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

public class ProjectileMovement: MonoBehaviour
{

    public float speed = 1.0f;
    public GameObject source;
    public GameObject target;
    Vector3 movementVector;

    IEnumerator moveLoop;

    // Update is called once per frame
    void Update()
    {
        if (moveLoop != null)
        {
            if (target == null)
            {
                StopCoroutine(moveLoop);
            }
        }
        else
        {
            if (target != null)
            {
                moveLoop = MoveLoop();
                StartCoroutine(moveLoop);
            }
        }
    }

    /// <summary>
    /// Move projectile in a straight line towards target (and past target, if it misses)
    /// </summary>
    /// <returns></returns>
    IEnumerator MoveLoop()
    {
        while (true)
        {
            transform.position += movementVector * speed * Time.deltaTime;
            yield return null;
        }
    }

    /// <summary>
    /// Sets the projectile's targetPosition based on the given target and calculates movementVector
    /// </summary>
    public void SetTarget (GameObject s, GameObject t)
    {
        source = s;
        target = t;

        movementVector = (t.transform.position - transform.position).normalized;
        gameObject.GetComponentInChildren<SpriteRenderer>().flipX = (target.transform.position.x < transform.position.x);
    }

    /*
    public void TurnToFace()
    {
        gameObject.GetComponentInChildren<SpriteRenderer>().flipX = (target.transform.position.x < transform.position.x);
    }

    */
}
