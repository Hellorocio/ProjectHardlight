using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatInfo
{
    public enum Range { Melee, Short, Medium, Long};
    public enum Team { Unknown, Hero, Enemy };

    public static int meleeRange = 100;
    public static int shortRange = 300;
    public static int mediumRange = 500;
    public static int longRange = 800;
}
