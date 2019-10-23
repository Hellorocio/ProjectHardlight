using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

// TargetUnit follows, TargetPosition goes in one dir
public enum ProjectileType { Unknown, TargetUnit, TargetPosition };

public class ProjectileMovement: MonoBehaviour
{
    public float speed = 1.0f;
    // One of these is populated based on what type of projectile
    public Vector3 targetPos;
    public GameObject targetObject;
    public ProjectileType type;

    // While moving, where it should go. Static for pos, dynamic for unit.
    public Vector3 movementDirection;
    public bool shouldRotate = true;
    
    IEnumerator moveLoop;

    IEnumerator MoveLoop()
    {
        while (true)
        {
            if (type == ProjectileType.TargetUnit)
            {
                // If unit disappears, destroy self
                // TODO Do something more elegant?
                if (targetObject == null)
                {
                    StopCoroutine(moveLoop);
                }
                
                // Update movement
                //movementDirection = targetObject.transform.position - transform.position;
                // Update rotation
                if (shouldRotate)
                {
                    RotateTowards(targetObject.transform.position);
                }
            }

            transform.position = Vector3.MoveTowards(transform.position, targetObject.transform.position, speed);
           
            yield return null;
        }
    }

    // Follow an object
    public void SetTarget(GameObject targetObject)
    {
        this.targetObject = targetObject;
        type = ProjectileType.TargetUnit;
        //movementDirection = targetObject.transform.position;

        //if (shouldRotate)
        //{
        //    RotateTowards(movementDirection);
        //}
        StartMovement();
    }

    // Go towards set destination
    public void SetTarget (Vector3 targetPos)
    {
        this.targetPos = targetPos;
        type = ProjectileType.TargetPosition;
        movementDirection = targetPos;
        
        if (shouldRotate)
        {
            RotateTowards(targetPos);
        }
    }
    
    // Set if should rotate
    public void SetShouldRotate(bool shouldRotate)
    {
        this.shouldRotate = shouldRotate;
    }

    public void StartMovement()
    {
        if (moveLoop != null)
        {
            Debug.Log("WARNING: Starting a projectile move loop when one already exists");
        }

        moveLoop = MoveLoop();
        StartCoroutine(moveLoop);
    }

    private void RotateTowards(Vector3 pos)
    {
        Vector3 dir = pos - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
