﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroSelectionAndOrdering : MonoBehaviour
{
    private SpriteRenderer spriteRend;
    private Color unselectedColor;
    private Color selectedColor = new Color(0, 1, 0);
    public bool isHeroSelected = false;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Is HHeroSelection and Ordering actually used??");
        spriteRend = gameObject.GetComponentInChildren<SpriteRenderer>();
        unselectedColor = spriteRend.material.color;

    }


    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 pos = Input.mousePosition;
            Collider2D hitCollider = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(pos));
            if (hitCollider != null && hitCollider.gameObject.Equals(gameObject))
            {
                spriteRend.material.color = selectedColor;
                isHeroSelected = true;
                //BattleManager.Instance.SetSelectedHero(gameObject);
                //Debug.Log(BattleManager.Instance.selectedHero.GetComponent<Fighter>().characterName);
            } else
            {
                //if (BattleManager.Instance.selectedHero == gameObject)
                {
                    //BattleManager.Instance.DeselectHero();
                }
                isHeroSelected = false;
                spriteRend.material.color = unselectedColor;
            }
        }
    }
}
