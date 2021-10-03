using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource masterAudio;
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] private AudioClip gameOver;

    private void Awake()
    {
        masterAudio = gameObject.GetComponent<AudioSource>();
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
