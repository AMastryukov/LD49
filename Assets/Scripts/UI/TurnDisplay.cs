using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TurnDisplay : MonoBehaviour
{
    private TextMeshProUGUI _turnText;
    private GameManager _gameManager;

    private void Awake()
    {
        _turnText = GetComponent<TextMeshProUGUI>();
        _gameManager = FindObjectOfType<GameManager>();

        GameManager.OnTurnComplete += UpdateText;
        GameManager.OnGameSetup += UpdateText;
    }

    private void OnDestroy()
    {
        GameManager.OnTurnComplete -= UpdateText;
        GameManager.OnGameSetup -= UpdateText;
    }

    private void UpdateText()
    {
        _turnText.text = "Month " + _gameManager.CurrentTurn;
    }
}
