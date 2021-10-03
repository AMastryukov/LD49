using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ETileState
{
    Neutral,
    Grabbed,
    Placed
}

public class Tile : MonoBehaviour
{
    private AudioManager _masterAudio;

    [SerializeField] private AudioClip placedSound;
    [SerializeField] private AudioClip grabbedSound;
    [SerializeField] private TileData data;
    
    public ETileState tileState = ETileState.Neutral;
    public string Name => data.Name;
    public Pillars Pillars { get; set; }

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

            return;
        }

        Pillars.Military = data.Military;
        Pillars.Economy = data.Economy;
        Pillars.Culture = data.Culture;
    }

    public void PlayPlacedSound()
    {
        _masterAudio.playAudioClip(placedSound);
    }

    public void PlayGrabbedSound()
    {
        _masterAudio.playAudioClip(grabbedSound);
    }
}