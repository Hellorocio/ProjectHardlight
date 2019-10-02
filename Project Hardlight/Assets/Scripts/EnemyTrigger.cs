using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTrigger : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D other)
    {
        Fighter fighter = other.GetComponent<Fighter>();
        if (GameManager.Instance.gameState == GameState.FIGHTING && fighter != null && fighter.team == CombatInfo.Team.Hero)
        {
            foreach (FighterAttack fighterAttack in GetComponentsInChildren<FighterAttack>())
            {
                if (fighterAttack.currentTarget == null)
                {
                    fighterAttack.SetCurrentTarget();
                }
            }
        }
    }
}
