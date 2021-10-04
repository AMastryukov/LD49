using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum ETileState
{
    Neutral,
    Grabbed,
    Placed
}

public class Tile : MonoBehaviour
{
    public Canvas TileCanvas;
    public ETileState tileState = ETileState.Neutral;
    public ETileType tileType => data.TileType;
    public string Name => data.Name;
    public Pillars Pillars { get; set; }

    [SerializeField] private AudioClip placedSound;
    [SerializeField] private AudioClip grabbedSound;
    [SerializeField] private TileData data;

    [Header("Pillar Texts")]
    [SerializeField] private TextMeshProUGUI militaryPillarText;
    [SerializeField] private TextMeshProUGUI economyPillarText;
    [SerializeField] private TextMeshProUGUI culturePillarText;

    private AudioManager _masterAudio;

    public void Awake()
    {
        _masterAudio = FindObjectOfType<AudioManager>();
        Pillars = new Pillars();

        // A Gulag is a special case where all pillar values are random
        if (string.IsNullOrEmpty(Name))
        {
            Pillars.Military = Random.Range(-3, 4);
            Pillars.Economy = Random.Range(-3, 4);
            Pillars.Culture = Random.Range(-3, 4);

            militaryPillarText.text = "?";
            economyPillarText.text = "?";
            culturePillarText.text = "?";

            return;
        }

        Pillars.Military = data.Military;
        Pillars.Economy = data.Economy;
        Pillars.Culture = data.Culture;

        militaryPillarText.gameObject.SetActive(Pillars.Military != 0);
        economyPillarText.gameObject.SetActive(Pillars.Economy != 0);
        culturePillarText.gameObject.SetActive(Pillars.Culture != 0);

        militaryPillarText.text = (Pillars.Military > 0 ? "+" : "") + Pillars.Military.ToString();
        economyPillarText.text = (Pillars.Economy > 0 ? "+" : "") + Pillars.Economy.ToString();
        culturePillarText.text = (Pillars.Culture > 0 ? "+" : "") + Pillars.Culture.ToString();
    }
}