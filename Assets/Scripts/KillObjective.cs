using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillObjective : QuestObjective {

    public int killsNeeded;
    public string targetName;

    int killCount;

    public void AddKill()
    {
        killCount++;
    }
}
