using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestController : MonoBehaviour {

    List<Quest> activeQuests = new List<Quest>();

    public void AddQuest(Quest quest)
    {
        activeQuests.Add(quest);
    }
}
