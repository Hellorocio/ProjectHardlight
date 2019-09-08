using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

public class ProjectileMovement: MonoBehaviour
{

    public float speed = 1.0f;
    public GameObject source;
    public GameObject target;

    IEnumerator moveLoop;

    // Start is called before the first frame update
    void Start()
    {

    }

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

    IEnumerator MoveLoop()
    {
        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
            yield return null;
        }
    }
}
