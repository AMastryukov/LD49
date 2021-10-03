using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private Canvas dialogueCanvas;
    [SerializeField] private TextMeshProUGUI advisorName;
    [SerializeField] private TextMeshProUGUI advisorDialogue;
    [SerializeField] private Image advisorImage;
    [SerializeField] private Sprite[] advisorSprites;

    [SerializeField] private float textRenderingSpeed = 0.025f;
    private Queue<string> loadedDialogue;

    // TODO: REMOVE AFTER TESTING:
    public DialogueData testDialogue;

    private void Awake()
    {
        dialogueCanvas.enabled = false;
        loadedDialogue = new Queue<string>();
    }

    private void Start()
    {
        // TODO: REMOVE AFTER TESTING
        LoadDialogue(testDialogue);
    }

    public void LoadDialogue(DialogueData dialogue)
    {
        // Set name of advisor
        string advisorType = Enum.GetName(typeof(DialogueData.Advisor), dialogue.advisor);
        advisorName.text = advisorType + " Advisor";

        // Load all dialogue sentences into a queue
        foreach (string sentence in dialogue.sentences)
        {
            loadedDialogue.Enqueue(sentence);
        }

        SetAdvisorSprite(dialogue.advisor);
        dialogueCanvas.enabled = true;
        LoadNextSentence();
    }

    public void LoadNextSentence()
    {
        StopAllCoroutines();
        if (loadedDialogue.Count > 0)
        {
            StartCoroutine(TypeSentence(loadedDialogue.Dequeue()));
        }
        else
        {
            EndDialogue();
        }
    }

    // Animates the display of sentence as if it was being typed
    IEnumerator TypeSentence(string sentence)
    {
        // TODO: Play typewriter audio

        advisorDialogue.text = "";
        foreach (char character in sentence.ToCharArray())
        {
            advisorDialogue.text += character;
            yield return new WaitForSeconds(textRenderingSpeed);
        }
    }

    private void SetAdvisorSprite(DialogueData.Advisor advisor)
    {
        advisorImage.sprite = advisorSprites[(int)advisor];
    }

    private void EndDialogue()
    {
        StopAllCoroutines();
        loadedDialogue.Clear();
        dialogueCanvas.enabled = false;
    }
}
