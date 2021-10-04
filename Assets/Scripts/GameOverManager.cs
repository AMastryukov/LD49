using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private Canvas gameOverCanvas;
    [SerializeField] private TextMeshProUGUI gameStateTitle;
    [SerializeField] private TextMeshProUGUI gameStateDescription;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private DialogueData[] gameOverDialogues;

    private GameManager _gameManager;
    private DialogueManager _dialogueManager;

    private void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _dialogueManager = FindObjectOfType<DialogueManager>();

        GameManager.OnGameOver += OnGameOver;
        gameOverCanvas.enabled = false;
    }

    private void OnGameOver()
    {
        gameOverCanvas.enabled = true;

        // Set Game Over screen title
        if (_gameManager.CurrentGameState == GameManager.GameState.Victory)
        {
            gameStateTitle.text = "VICTORY";
            scoreText.text = "";
        }
        else
        {
            gameStateTitle.text = "DEFEAT";
            scoreText.text = "Months Survived: " + _gameManager.CurrentTurn.ToString();
        }

        // Display advice over lose or win condition
        // NOTE: gameOverDialogues[] indices should correspond to
        // the order in which the enum GameManager.GameOverState is defined
        if (_gameManager.CurrentGameOverState != GameManager.GameOverState.None)
        {
            int dialogueIndex = (int) _gameManager.CurrentGameOverState;
            string description = "";
            foreach (string sentence in gameOverDialogues[dialogueIndex].sentences)
            {
                description += sentence + " ";
            }
            gameStateDescription.text = description;
        }
    }

    private void OnDestroy()
    {
        GameManager.OnGameOver -= OnGameOver;
    }
}
