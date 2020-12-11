using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour {

    public Text healthText;
    public RectTransform healthBarScale;
    public RectTransform dialogueBox;
    public Text dialogueText;
    public Button nextDialogueButton;
    public Button prevDialogueButton;
    public Button stopDialogueButton;

    string[] dialogueLines;
    int dialogueIndex;

	void Start ()
    {
        
    }
	
	void Update ()
    {

	}

    public void SetHealth(int health, int maxHealth)
    {
        float scale = (float)health / maxHealth;
        healthBarScale.localScale = new Vector3(scale, 1, 1);

        healthText.text = "" + health + " / " + maxHealth;
    }

    //max line length: 70
    public void StartDialogue(string[] dialogueLines)
    {
        this.dialogueLines = new string[dialogueLines.Length];

        for (int i = 0; i < dialogueLines.Length; i++)
        {
            this.dialogueLines[i] = dialogueLines[i];
        }

        ShowDialogueBox(true);

        dialogueIndex = 0;

        UpdateDialogue(this.dialogueLines[dialogueIndex]);
        UpdateDialogueButtons();
    }

    public void StopDialogue()
    {
        ShowDialogueBox(false);
    }

    public void NextDialogueLine()
    {
        if (dialogueIndex < dialogueLines.Length - 1)
        {
            dialogueIndex++;
        }

        UpdateDialogue(dialogueLines[dialogueIndex]);
        UpdateDialogueButtons();
    }

    public void PreviousDialogueLine()
    {
        if (dialogueIndex > 0)
        {
            dialogueIndex--;
        }

        UpdateDialogue(dialogueLines[dialogueIndex]);
        UpdateDialogueButtons();
    }

    public void ShowDialogueBox(bool show)
    {
        dialogueBox.gameObject.SetActive(show);
    }

    void UpdateDialogue(string dialogueLine)
    {
        dialogueText.text = dialogueLine;
    }

    void UpdateDialogueButtons()
    {
        nextDialogueButton.GetComponent<Button>().interactable = (dialogueIndex < dialogueLines.Length - 1);
        nextDialogueButton.GetComponent<Image>().enabled = (dialogueIndex < dialogueLines.Length - 1);
        stopDialogueButton.GetComponent<Button>().interactable = (dialogueIndex == dialogueLines.Length - 1);
        stopDialogueButton.GetComponent<Image>().enabled = (dialogueIndex == dialogueLines.Length - 1);
        prevDialogueButton.GetComponent<Button>().interactable = (dialogueIndex > 0);
        prevDialogueButton.GetComponent<Image>().enabled = (dialogueIndex > 0);
    }
}
