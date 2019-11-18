using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawnPointFlipper : MonoBehaviour
{
    public GameObject spawnPointObj;
    public SpriteRenderer renderer;
    private Vector3 startPos;

    private void Start()
    {
        startPos = spawnPointObj.transform.localPosition;
    }

    private void Update()
    {
        if (!renderer.flipX)
        {
            spawnPointObj.transform.localPosition = startPos;
        } else
        {
            spawnPointObj.transform.localPosition = new Vector3(-startPos.x, startPos.y, startPos.z);
        }
    }
}
