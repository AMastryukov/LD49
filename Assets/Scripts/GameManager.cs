using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    public static Action OnPillarDeltasUpdated;

    public GameState CurrentGameState = GameState.InProgress;
    public GameOverState CurrentGameOverState = GameOverState.None;
    public int CurrentTurn { get; set; } = 1;
    public Pillars Pillars { get; set; }
    public Pillars PillarDeltas { get; set; }
    public int TurnUntilDecay { get; set; } = 2;

    private bool _isEnabled = true;
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

    [SerializeField] private int winTurn = 24;
    [SerializeField] private int maximumPillar = 100;
    public int MaximumPillar => maximumPillar;

    [SerializeField] private int minimumPillar = 0;
    public int MinimumPillar => minimumPillar;

    [SerializeField] private List<Tile> tilePrefabs;

    private GridManager _gridManager;
    private TileTray _tileTray;
    private AudioManager _audioManager;
    private DialogueManager _dialogueManager;

    [SerializeField]
    private DialogueData EconomyStrong;
    private bool economyStrongPlayed = false;
    [SerializeField]
    private DialogueData EconomyWeak;
    private bool economyWeakPlayed = false;
    [SerializeField]
    private DialogueData MilitaryStrong;
    private bool militaryStrongPlayed = false;
    [SerializeField]
    private DialogueData MilitaryWeak;
    private bool militaryWeakPlayed = false;
    [SerializeField]
    private DialogueData CultureStrong;
    private bool cultureStrongPlayed = false;
    [SerializeField]
    private DialogueData CultureWeak;
    private bool cultureWeakPlayed = false;

    [SerializeField]
    public float criticalThreshold { get; private set; } = 0.35f;

    private void Awake()
    {
        Pillars = new Pillars();
        PillarDeltas = new Pillars();

        _gridManager = FindObjectOfType<GridManager>();
        _tileTray = FindObjectOfType<TileTray>();
        _audioManager = FindObjectOfType<AudioManager>();
        _dialogueManager = FindObjectOfType<DialogueManager>();

        GridManager.OnTilePlacementConfirmed += ProcessTurn;
        TileTray.OnTilePlaced += PopulateTileTray;
        TileTray.OnTilePlaced += UpdatePillarDeltaValues;
        GridManager.OnTileEnterHex += UpdatePillarDeltaValues;
        GridManager.OnTileLeaveHex += UpdatePillarDeltaValues;
    }

    private void OnDestroy()
    {
        GridManager.OnTilePlacementConfirmed -= ProcessTurn;
        TileTray.OnTilePlaced -= PopulateTileTray;
        TileTray.OnTilePlaced -= UpdatePillarDeltaValues;
        GridManager.OnTileEnterHex -= UpdatePillarDeltaValues;
        GridManager.OnTileLeaveHex -= UpdatePillarDeltaValues;
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
        UpdatePillarDeltaValues();
        UpdatePillarValues();

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
        DecayTiles();

        CurrentTurn++;
        TurnUntilDecay--;

        CheckAdvisors();

        OnTurnComplete?.Invoke();
    }

    private void CheckAdvisors()
    {
        if(Pillars.Military < maximumPillar * criticalThreshold && !militaryWeakPlayed)
        {
            _dialogueManager.LoadDialogue(MilitaryWeak);
            militaryWeakPlayed = true;
        }
        else if(Pillars.Military > maximumPillar * (1 - criticalThreshold) && !militaryStrongPlayed)
        {
            _dialogueManager.LoadDialogue(MilitaryStrong);
            militaryStrongPlayed = true;
        }else if(Pillars.Culture < maximumPillar * criticalThreshold && !cultureWeakPlayed)
        {
            _dialogueManager.LoadDialogue(CultureWeak);
            cultureWeakPlayed = true;
        }
        else if (Pillars.Culture > maximumPillar * (1 - criticalThreshold) && !cultureStrongPlayed)
        {
            _dialogueManager.LoadDialogue(CultureStrong);
            cultureStrongPlayed = true;
        }
        else if (Pillars.Economy < maximumPillar * criticalThreshold && !economyWeakPlayed)
        {
            _dialogueManager.LoadDialogue(EconomyWeak);
            economyWeakPlayed = true;
        }
        else if (Pillars.Economy > maximumPillar * (1 - criticalThreshold) && !economyStrongPlayed)
        {
            _dialogueManager.LoadDialogue(EconomyStrong);
            economyStrongPlayed = true;
        }
    }

    private void DecayTiles()
    {
        if (TurnUntilDecay == 0)
        {
            var tiles = _gridManager.GetTiles().Where(t => t.Type != ETileType.None && t != _gridManager.LastTilePlaced);
            var randomTile = tiles.ElementAt(UnityEngine.Random.Range(0, tiles.Count()));

            if (randomTile != null) 
            {
                // Replace tile with empty land
                var newTilePrefab = tilePrefabs.Where(t => t.Type == ETileType.None).FirstOrDefault();
                var newTile = Instantiate(newTilePrefab);

                newTile.Pillars.Military = 0;
                newTile.Pillars.Economy = 0;
                newTile.Pillars.Culture = 0;

                _gridManager.ReplaceTile(randomTile, newTile);

                TurnUntilDecay = 3;
            }
            else
            {
                TurnUntilDecay = 0;
            }
        }
    }

    private void UpdatePillarDeltaValues()
    {
        PillarDeltas = new Pillars();

        foreach (Tile tile in _gridManager.GetTiles())
        {
            PillarDeltas += tile.Pillars;
        }

        // Sum the currently held tile as well
        if (_tileTray.GrabbedTile != null && _tileTray.GrabbedTile.Type != ETileType.None && _gridManager.IsTileHoveringAboveValidHex)
        {
            PillarDeltas += _tileTray.GrabbedTile.Pillars;
        }

        OnPillarDeltasUpdated?.Invoke();

    }

    private void UpdatePillarValues()
    {
        Pillars += PillarDeltas;
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
            Debug.Log("Your nation's lack of culture resulted in a loss of its identity. The nation split into numerous tribes that began to war with one another.");

            CurrentGameOverState = GameOverState.LowCulture;
            CurrentGameState = GameState.Defeat;
        }

        if (Pillars.Culture >= maximumPillar)
        {
            Debug.Log("Your nation's culture . Long live the resistance!");

            CurrentGameOverState = GameOverState.ExcessCulture;
            CurrentGameState = GameState.Defeat;
        }

        if (CurrentGameState == GameState.Defeat)
        {
            _tileTray.IsEnabled = false;
            GameActive = false;
            
            _audioManager.PlaySound(AudioManager.Sounds.GameOver);

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
