using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Quest {

    public string title;
    public string description;
    public QuestObjective[] objectives;

    int objectiveIndex;

    public int getCurrentObjectiveIndex()
    {
        return objectiveIndex;
    }
}
