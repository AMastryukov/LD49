using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState { InProgress, Defeat, Victory }
    public enum GameOverState
    {
        Victory,
        LowMilitary,
        LowEconomy,
        LowCulture,
        ExcessMilitary,
        ExcessEconomy,
        ExcessCulture,
        None
    }

    public static Action OnGameSetup;
    public static Action OnTurnComplete;
    public static Action OnGameOver;

    public GameState CurrentGameState = GameState.InProgress;
    public GameOverState CurrentGameOverState = GameOverState.None;
    public int CurrentTurn { get; set; } = 1;
    public Pillars Pillars { get; set; }

    private bool _isEnabled = false;
    public bool GameActive
    {
        get
        {
            return _isEnabled;
        }
        set
        {
            _isEnabled = value;
            _tileTray.IsEnabled = value;
        }
    }

    [SerializeField] private int winTurn = 25;
    [SerializeField] private int maximumPillar = 100;
    [SerializeField] private int minimumPillar = 0;
    [SerializeField] private List<Tile> tilePrefabs;

    private GridManager _gridManager;
    private TileTray _tileTray;
    private AudioManager _masterAudio;

    private void Awake()
    {
        Pillars = new Pillars();
        _gridManager = FindObjectOfType<GridManager>();
        _tileTray = FindObjectOfType<TileTray>();
        _masterAudio = FindObjectOfType<AudioManager>();

        GridManager.OnTilePlacementConfirmed += ProcessTurn;
        TileTray.OnTilePlaced += PopulateTileTray;
    }

    private void OnDestroy()
    {
        GridManager.OnTilePlacementConfirmed -= ProcessTurn;
        TileTray.OnTilePlaced -= PopulateTileTray;
    }

    private void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        PlaceStartingTile();
        PopulateTileTray();
        ResetPillars();

        _tileTray.IsEnabled = true;

        OnGameSetup?.Invoke();
    }

    private void ResetPillars()
    {
        Pillars.Military = (maximumPillar - minimumPillar) / 2;
        Pillars.Economy = (maximumPillar - minimumPillar) / 2;
        Pillars.Culture = (maximumPillar - minimumPillar) / 2;
    }

    private void PlaceStartingTile()
    {
        // We start with a random tile for now
        var randomTilePrefab = tilePrefabs[UnityEngine.Random.Range(0, tilePrefabs.Count)];
        Tile newTile = Instantiate(randomTilePrefab, _tileTray.SpawnPosition, Quaternion.identity, transform).GetComponent<Tile>();

        _gridManager.RegisterAndPlaceTile(newTile, new Hex(0,0), true);
    }

    private void PopulateTileTray()
    {
        StartCoroutine(PopulateTileTrayCoroutine());
    }

    private IEnumerator PopulateTileTrayCoroutine()
    {
        while (_tileTray.IsSpaceOnTray())
        {
            var randomTilePrefab = tilePrefabs[UnityEngine.Random.Range(0, tilePrefabs.Count)];

            Tile newTile = Instantiate(randomTilePrefab, _tileTray.SpawnPosition, Quaternion.identity, transform).GetComponent<Tile>();

            _tileTray.TryAddTileToTray(newTile);

            yield return new WaitForSeconds(0.25f);
        }
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

        foreach (Tile tile in _gridManager.GetTiles())
        {
            deltaPillars += tile.Pillars;
        }

        Pillars += deltaPillars;
    }

    private void CheckLoseConditions()
    {
        
        if (Pillars.Military <= minimumPillar)
        {
            Debug.Log("Your military was too weak and you were overthrown by the people.");

            CurrentGameOverState = GameOverState.LowMilitary;
            CurrentGameState = GameState.Defeat;
        }

        if (Pillars.Military >= maximumPillar)
        {
            Debug.Log("Your military was too strong and you were overthrown in a military coup.");

            CurrentGameOverState = GameOverState.ExcessMilitary;
            CurrentGameState = GameState.Defeat;
        }

        if (Pillars.Economy <= minimumPillar)
        {  
            Debug.Log("Your failed to maintain a minimum economic supply and your population starved.");

            CurrentGameOverState = GameOverState.LowEconomy;
            CurrentGameState = GameState.Defeat;
        }

        if (Pillars.Economy >= maximumPillar)
        {
            Debug.Log("Your planned economy collapsed due to an overabundance of supply.");

            CurrentGameOverState = GameOverState.ExcessEconomy;
            CurrentGameState = GameState.Defeat;
        }

        if (Pillars.Culture <= minimumPillar)
        {
            Debug.Log("Your influence over your population dwindled and your state slowly dissolved.");

            CurrentGameOverState = GameOverState.LowCulture;
            CurrentGameState = GameState.Defeat;
        }

        if (Pillars.Culture >= maximumPillar)
        {
            Debug.Log("Your grip on the population became too tight and rebel groups staged a coup. Long live the resistance!");

            CurrentGameOverState = GameOverState.ExcessCulture;
            CurrentGameState = GameState.Defeat;
        }

        if (CurrentGameState == GameState.Defeat)
        {
            _tileTray.IsEnabled = false;
            GameActive = false;

            _masterAudio.gameOverSound();
            OnGameOver?.Invoke();
        }
    }

    private void CheckWinConditions()
    {
        if (CurrentTurn >= winTurn && CurrentGameState == GameState.InProgress)
        {
            CurrentGameState = GameState.Victory;
            GameActive = false;

            OnGameOver?.Invoke();
        }
    }
}
