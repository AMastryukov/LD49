using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PillarDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI militaryText;
    [SerializeField] private TextMeshProUGUI economyText;
    [SerializeField] private TextMeshProUGUI cultureText;

    private GameManager _gameManager;

    private void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();

        GameManager.OnTurnComplete += UpdatePillarTexts;
    }

    private void OnDestroy()
    {
        GameManager.OnTurnComplete -= UpdatePillarTexts;
    }

    private void UpdatePillarTexts()
    {
        militaryText.text = _gameManager.Pillars.Military.ToString();
        economyText.text = _gameManager.Pillars.Economy.ToString();
        cultureText.text = _gameManager.Pillars.Culture.ToString();
    }
}
