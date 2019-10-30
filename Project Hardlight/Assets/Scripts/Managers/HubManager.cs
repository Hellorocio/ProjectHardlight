using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubManager : MonoBehaviour
{
    public FighterMove selectedFighter;
    public GameObject moveLoc;
    public GameObject hubUI;

    private void Start()
    {
        GameManager.Instance.SetCameraControls(true);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            // move vessel to clicked position
            Vector3 pos = Input.mousePosition;
            pos = Camera.main.ScreenToWorldPoint(pos);
            GameObject newMoveLoc = Instantiate(moveLoc);
            newMoveLoc.transform.position = new Vector3(pos.x, pos.y, 2);
            newMoveLoc.SetActive(true);
            //init line
            LineRenderer line = newMoveLoc.GetComponentInChildren<LineRenderer>();
            line.positionCount = 2;
            line.SetPosition(0, newMoveLoc.transform.position);
            line.SetPosition(1, selectedFighter.transform.position);
            selectedFighter.StartMovingCommandHandle(newMoveLoc.transform);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.Instance.soulUpgradeUI.SetActive(false);
            GameManager.Instance.EnterMap();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            UIManager.Instance.ToggleSoulUpgradeUI();
            //hubUI.SetActive(!hubUI.activeSelf);
        }
    }
}
