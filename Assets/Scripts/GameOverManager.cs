using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private Canvas gameOverCanvas;
    [SerializeField] private TextMeshProUGUI gameStateTitle;
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
        }
        else
        {
            gameStateTitle.text = "DEFEAT";
        }

        // Display advice over lose or win condition
        switch (_gameManager.CurrentGameOverState)
        {
            case GameManager.GameOverState.Victory:
                break;
            case GameManager.GameOverState.LowMilitary:
                break;
            case GameManager.GameOverState.LowEconomy:
                break;
            case GameManager.GameOverState.LowCulture:
                break;
            case GameManager.GameOverState.ExcessMilitary:
                break;
            case GameManager.GameOverState.ExcessEconomy:
                break;
            case GameManager.GameOverState.ExcessCulture:
                break;
            default:
                // Matched GameManager.GameState.None, so do nothing
                break;
        }
    }

    private void OnDestroy()
    {
        GameManager.OnGameOver -= OnGameOver;
    }
}
