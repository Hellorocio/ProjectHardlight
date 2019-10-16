using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Battle/BattleConfig")]
public class BattleConfig : ScriptableObject
{
    public Texture2D defaultCursor;
    public Texture2D targetPositionCursor;
    public Texture2D targetUnitCursor;
    public Texture2D changeTargetCursor;
}
