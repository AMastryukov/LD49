using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState { InProgress, Loss, Victory }

    public static Action OnTurnComplete;

    public GameState CurrentGameState = GameState.InProgress;
    public int CurrentTurn { get; set; } = 0;
    public Pillars Pillars { get; set; }

    [SerializeField] private int winTurn = 25;
    [SerializeField] private int maximumPillar = 100;
    [SerializeField] private int minimumPillar = 0;

    private MapManager _mapManager;

    private void Awake()
    {
        Pillars = new Pillars();
        _mapManager = FindObjectOfType<MapManager>();

        MapManager.OnTilePlaced += ProcessTurn;

        ResetGame();
    }

    private void OnDestroy()
    {
        MapManager.OnTilePlaced -= ProcessTurn;
    }

    private void ResetGame()
    {
        Pillars.Military = (maximumPillar - minimumPillar) / 2;
        Pillars.Economy = (maximumPillar - minimumPillar) / 2;
        Pillars.Culture = (maximumPillar - minimumPillar) / 2;
    }

    private void ProcessTurn()
    {
        UpdatePillarValues();
        CheckLoseConditions();
        CheckWinConditions();

        CurrentTurn++;

        OnTurnComplete?.Invoke();
    }

    private void UpdatePillarValues()
    {
        Pillars deltaPillars = new Pillars();

        foreach (Cell cell in _mapManager.Cells)
        {
            if (cell.Tile != null)
            {
                deltaPillars += cell.Tile.Pillars;
            }
        }

        Pillars += deltaPillars;
    }

    private void CheckLoseConditions()
    {
        if (Pillars.Military <= minimumPillar)
        {
            Debug.Log("Your military was too weak and you were overthrown by the people.");
            CurrentGameState = GameState.Loss;
        }

        if (Pillars.Military >= maximumPillar)
        {
            Debug.Log("Your military was too strong and you were overthrown in a military coup.");
            CurrentGameState = GameState.Loss;
        }

        if (Pillars.Economy <= minimumPillar)
        {  
            Debug.Log("Your failed to maintain a minimum economic supply and your population starved.");
            CurrentGameState = GameState.Loss;
        }

        if (Pillars.Economy >= maximumPillar)
        {
            Debug.Log("Your planned economy collapsed due to an overabundance of supply.");
            CurrentGameState = GameState.Loss;
        }

        if (Pillars.Culture <= minimumPillar)
        {
            Debug.Log("Your influence over your population dwindled and your state slowly dissolved.");
            CurrentGameState = GameState.Loss;
        }

        if (Pillars.Culture >= maximumPillar)
        {
            Debug.Log("Your grip on the population became too tight and rebel groups staged a coup. Long live the resistance!");
            CurrentGameState = GameState.Loss;
        }
    }

    private void CheckWinConditions()
    {
        if (CurrentTurn >= winTurn && CurrentGameState == GameState.InProgress)
        {
            CurrentGameState = GameState.Victory;
        }
    }
}
