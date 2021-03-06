using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TutorialDisplay : MonoBehaviour
{
    public static bool SeenTutorial { get; set; } = false;

    [SerializeField] private List<Canvas> tutorialPages;
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;

    private GameManager _gameManager;
    private Canvas canvas;
    private int currentPage = 0;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();

        _gameManager = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        if (SeenTutorial) return;

        OpenTutorial();
    }

    public void OpenTutorial()
    {
        canvas.enabled = true;

        currentPage = 0;

        HideAllPages();

        tutorialPages[currentPage].enabled = true;

        UpdateButtons();

        _gameManager.GameActive = false;
    }

    public void CloseTutorial()
    {
        canvas.enabled = false; 
        
        _gameManager.GameActive = true;

        SeenTutorial = true;
    }

    public void PreviousPage()
    {
        if (currentPage == 0) { return; }

        HideAllPages();

        currentPage--;
        tutorialPages[currentPage].enabled = true;

        UpdateButtons();
    }

    public void NextPage()
    {
        if (currentPage == tutorialPages.Count - 1) 
        {
            CloseTutorial();
            return; 
        }

        HideAllPages();

        currentPage++;
        tutorialPages[currentPage].enabled = true;

        UpdateButtons();
    }

    public void HideAllPages()
    {
        foreach (var page in tutorialPages)
        {
            page.enabled = false;
        }
    }

    private void UpdateButtons()
    {
        previousButton.gameObject.SetActive(currentPage != 0);
        if(currentPage==tutorialPages.Count - 1)
        {
            nextButton.gameObject.GetComponent<TextMeshProUGUI>().text = "Continue";
        }
        else
        {
            nextButton.gameObject.GetComponent<TextMeshProUGUI>().text = "Next";
        }
        
    }
}