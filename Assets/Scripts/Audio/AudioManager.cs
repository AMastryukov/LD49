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
        TileDestory,
        Typewriter,
        GameOver
    }

    public float CurrentMusicVolume => musicSource.volume;
    public float CurrentEffectsVolume => effectsSource.volume;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource effectsSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip buttonHover;
    [SerializeField] private AudioClip buttonClick;
    [SerializeField] private AudioClip tilePlace;
    [SerializeField] private AudioClip tileDestroy;
    [SerializeField] private AudioClip typewriter;
    [SerializeField] private AudioClip gameOver;

    [Header("Music Clips")]
    [SerializeField] private AudioClip[] gameMusic;
    [SerializeField] private AudioClip mainMenuMusic;

    private int currentGameMusicIndex = 0;
    private bool shouldPlayMusic = true;

    private void Start()
    {
        // Get random game music
        currentGameMusicIndex = Random.Range(0, gameMusic.Length);
        PlayCurrentMusic();
    }

    void Update()
    {
        if (!musicSource.isPlaying && shouldPlayMusic)
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

            case Sounds.TileDestory:
                effectsSource.PlayOneShot(tileDestroy);
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
        StopMusic();
        shouldPlayMusic = true;

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

    public void StopMusic()
    {
        musicSource.Stop();
        shouldPlayMusic = false;
    }

    public void UpdateMusicVolume(float newVolume)
    {
        musicSource.volume = newVolume;
    }

    public void UpdateEffectsVolume(float newVolume)
    {
        effectsSource.volume = newVolume;
    }
}
