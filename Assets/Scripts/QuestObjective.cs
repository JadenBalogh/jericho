using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QuestObjective {

    public string description;
    //A quest is associated with a QuestNPC and objectives 
    //store the dialogue for each part of the quest for that NPC
    [Tooltip("Max 70 characters per line.")]
    public string[] dialogueLines;
}
