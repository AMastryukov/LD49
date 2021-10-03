using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class PillarDisplay : MonoBehaviour
{
    [SerializeField] private CounterText militaryText;
    [SerializeField] private CounterText economyText;
    [SerializeField] private CounterText cultureText;

    private GameManager _gameManager;

    private void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();

        GameManager.OnGameSetup += SetPillarTexts;
        GameManager.OnTurnComplete += UpdatePillarTexts;
    }

    private void OnDestroy()
    {
        GameManager.OnGameSetup -= SetPillarTexts;
        GameManager.OnTurnComplete -= UpdatePillarTexts;
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
}
