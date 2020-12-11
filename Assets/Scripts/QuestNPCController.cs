using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestNPCController : MonoBehaviour {
    
    public Quest quest;

    GameObject UICanvas;
    GameObject HUDCanvas;
    
    public void Interact()
    {
        if (HUDCanvas == null)
        {
            HUDCanvas = GameObject.FindGameObjectWithTag("HUDCanvas");
        }

        HUDCanvas.GetComponent<HUDController>().StartDialogue(quest.objectives[quest.getCurrentObjectiveIndex()].dialogueLines);
    }

    public void BestowQuest()
    {
        if (UICanvas == null)
        {
            UICanvas = GameObject.FindGameObjectWithTag("UICanvas");
        }

        UICanvas.GetComponent<UIController>();
    }
}
