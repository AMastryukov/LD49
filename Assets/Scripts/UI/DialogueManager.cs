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
    private GameManager _gameManager;
    private AudioManager _masterAudio;

    // TODO: REMOVE AFTER TESTING:
    public DialogueData testDialogue;

    private void Awake()
    {
        _masterAudio = FindObjectOfType<AudioManager>();
        dialogueCanvas.enabled = false;
        loadedDialogue = new Queue<string>();

        _gameManager = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        StartCoroutine(ShowTestDialogue());
    }

    private IEnumerator ShowTestDialogue()
    {
        yield return new WaitForSeconds(1f);

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

        _gameManager.GameActive = false;
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
        advisorDialogue.text = "";
        int maxCounts = 3;
        int counts = 0;
        foreach (char character in sentence.ToCharArray())
        {
            if(counts==maxCounts)
            {
                _masterAudio.playAudioClip(5);
                counts = 0;
            }
            advisorDialogue.text += character;
            yield return new WaitForSeconds(textRenderingSpeed);
            counts++;
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

        _gameManager.GameActive = true;
    }
}
