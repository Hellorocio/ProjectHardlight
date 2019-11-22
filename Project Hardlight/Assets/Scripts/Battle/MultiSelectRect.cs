using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiSelectRect : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Fighter otherFighter = other.gameObject.GetComponent<Fighter>();
        if (otherFighter != null && BattleManager.Instance.inputState == BattleManager.InputState.DraggingSelect && BattleManager.Instance.selectedVessels.Contains(otherFighter.gameObject))
        {
            BattleManager.Instance.multiSelectedHeros.Add(otherFighter);
            otherFighter.SetSelectedUI(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Fighter otherFighter = other.gameObject.GetComponent<Fighter>();
        if (otherFighter != null && BattleManager.Instance.inputState == BattleManager.InputState.DraggingSelect && BattleManager.Instance.multiSelectedHeros.Contains(otherFighter))
        {
            BattleManager.Instance.multiSelectedHeros.Remove(otherFighter);
            otherFighter.SetSelectedUI(false);
        }
    }
}
