using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource masterAudio;
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] private AudioClip[] gameMusic;
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private AudioClip gameOver;

    private int currentGameMusicIndex = 0;

    private void Awake()
    {
        masterAudio = gameObject.GetComponent<AudioSource>();
    }

    private void Start()
    {
        // Get random game music
        currentGameMusicIndex = Random.Range(0, gameMusic.Length);
        PlayCurrentMusic();
    }

    void Update()
    {
        if (!masterAudio.isPlaying)
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

    private void PlayCurrentMusic()
    {
        masterAudio.Stop();

        if (SceneManager.GetActiveScene().name.Contains("Menu"))
        {
            // Play main menu theme
            masterAudio.clip = mainMenuMusic;
        }
        else
        {
            // Play game theme
            masterAudio.clip = gameMusic[currentGameMusicIndex];
        }

        masterAudio.Play();
    }

    public void playAudioClip(int index)
    {
        masterAudio.PlayOneShot(audioClips[index]);
    }

    public void playAudioClip(AudioClip clip)
    {
        masterAudio.PlayOneShot(clip);
    }

    public void gameOverSound()
    {
        masterAudio.PlayOneShot(gameOver);
    }
}
