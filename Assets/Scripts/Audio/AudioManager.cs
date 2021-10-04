using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource masterAudio;
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] private AudioClip[] gameMusic;
    [SerializeField] private AudioClip gameOver;

    private int currentClipIndex = 0;

    private void Awake()
    {
        masterAudio = gameObject.GetComponent<AudioSource>();
    }

    private void Start()
    {
        // Get random game music
        currentClipIndex = Random.Range(0, gameMusic.Length);
        PlayCurrentMusic();
    }

    void Update()
    {
        if (!masterAudio.isPlaying)
        {
            // This is dumb but we only have two music clips so fuck off
            if (currentClipIndex == 0)
            {
                currentClipIndex = 1;
                return;
            }
            
            currentClipIndex = 0;
        }
    }

    private void PlayCurrentMusic()
    {
        masterAudio.Stop();
        masterAudio.clip = gameMusic[currentClipIndex];
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
