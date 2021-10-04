using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class PillarDisplay : MonoBehaviour
{
    [Header("Pillars")]
    [SerializeField] private CounterText militaryText;
    [SerializeField] private CounterText economyText;
    [SerializeField] private CounterText cultureText;

    [Header("Deltas")]
    [SerializeField] private CounterText militaryDeltaText;
    [SerializeField] private CounterText economyDeltaText;
    [SerializeField] private CounterText cultureDeltaText;

    [Header("Colors")]
    [SerializeField] private Color positive;
    [SerializeField] private Color neutral;
    [SerializeField] private Color negative;

    private GameManager _gameManager;

    private void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();

        GameManager.OnGameSetup += SetPillarTexts;
        GameManager.OnGameSetup += SetDeltaTexts;
        GameManager.OnTurnComplete += UpdatePillarTexts;
        GameManager.OnPillarDeltasUpdated += UpdateDeltaTexts;

        militaryDeltaText.Counter.color = neutral;
        economyDeltaText.Counter.color = neutral;
        cultureDeltaText.Counter.color = neutral;
    }

    private void OnDestroy()
    {
        GameManager.OnGameSetup -= SetPillarTexts;
        GameManager.OnGameSetup -= SetDeltaTexts;
        GameManager.OnTurnComplete -= UpdatePillarTexts;
        GameManager.OnPillarDeltasUpdated -= UpdateDeltaTexts;
    }

    private void SetPillarTexts()
    {
        militaryText.SetCounter(_gameManager.Pillars.Military);
        economyText.SetCounter(_gameManager.Pillars.Economy);
        cultureText.SetCounter(_gameManager.Pillars.Culture);
    }

    private void UpdatePillarTexts()
    {
        militaryText.UpdateCounter(_gameManager.Pillars.Military);
        economyText.UpdateCounter(_gameManager.Pillars.Economy);
        cultureText.UpdateCounter(_gameManager.Pillars.Culture);
    }

    private void SetDeltaTexts()
    {
        militaryDeltaText.SetCounter(_gameManager.PillarDeltas.Military, _gameManager.PillarDeltas.Military > 0 ? "+" : "");
        economyDeltaText.SetCounter(_gameManager.PillarDeltas.Economy, _gameManager.PillarDeltas.Economy > 0 ? "+" : "");
        cultureDeltaText.SetCounter(_gameManager.PillarDeltas.Culture, _gameManager.PillarDeltas.Culture > 0 ? "+" : "");
    }

    private void UpdateDeltaTexts()
    {
        militaryDeltaText.UpdateCounter(_gameManager.PillarDeltas.Military, _gameManager.PillarDeltas.Military > 0 ? "+" : "");
        economyDeltaText.UpdateCounter(_gameManager.PillarDeltas.Economy, _gameManager.PillarDeltas.Economy > 0 ? "+" : "");
        cultureDeltaText.UpdateCounter(_gameManager.PillarDeltas.Culture, _gameManager.PillarDeltas.Culture > 0 ? "+" : "");

        militaryDeltaText.Counter.color = GetColorByValue(_gameManager.PillarDeltas.Military);
        economyDeltaText.Counter.color = GetColorByValue(_gameManager.PillarDeltas.Economy);
        cultureDeltaText.Counter.color = GetColorByValue(_gameManager.PillarDeltas.Culture);
    }

    private Color GetColorByValue(int value)
    {
        if (value > 0)
        {
            return positive;
        }
        else if (value < 0)
        {
            return negative;
        }

        return neutral;
    }
}
