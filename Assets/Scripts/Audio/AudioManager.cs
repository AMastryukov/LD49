using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public enum Sounds
    {
        ButtonHover,
        ButtonClick,
        TilePlace,
        Typewriter,
        GameOver
    }

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource effectsSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip buttonHover;
    [SerializeField] private AudioClip buttonClick;
    [SerializeField] private AudioClip tileGrab;
    [SerializeField] private AudioClip tilePlace;
    [SerializeField] private AudioClip typewriter;
    [SerializeField] private AudioClip gameOver;

    [Header("Music Clips")]
    [SerializeField] private AudioClip[] gameMusic;
    [SerializeField] private AudioClip mainMenuMusic;

    private int currentGameMusicIndex = 0;

    private void Start()
    {
        // Get random game music
        currentGameMusicIndex = Random.Range(0, gameMusic.Length);
        PlayCurrentMusic();
    }

    void Update()
    {
        if (!musicSource.isPlaying)
        {
            // This is dumb but we only have two music clips so fuck off
            if (currentGameMusicIndex == 0)
            {
                currentGameMusicIndex = 1;
                return;
            }
            
            currentGameMusicIndex = 0;

            PlayCurrentMusic();
        }
    }

    public void PlaySound(Sounds sound)
    {
        switch (sound)
        {
            case Sounds.ButtonHover:
                effectsSource.PlayOneShot(buttonHover);
                break;

            case Sounds.ButtonClick:
                effectsSource.PlayOneShot(buttonClick);
                break;

            case Sounds.TilePlace:
                effectsSource.PlayOneShot(tilePlace);
                break;

            case Sounds.Typewriter:
                effectsSource.PlayOneShot(typewriter);
                break;

            case Sounds.GameOver:
                effectsSource.PlayOneShot(gameOver);
                break;
        }
    }

    private void PlayCurrentMusic()
    {
        musicSource.Stop();

        if (SceneManager.GetActiveScene().name.Contains("Menu"))
        {
            // Play main menu theme
            musicSource.clip = mainMenuMusic;
        }
        else
        {
            // Play game theme
            musicSource.clip = gameMusic[currentGameMusicIndex];
        }

        musicSource.Play();
    }
}
