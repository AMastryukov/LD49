using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private bool isMusic = false;

    private Slider _slider;
    private AudioManager _audioManager;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
        _audioManager = FindObjectOfType<AudioManager>();
    }

    private void Start()
    {
        if (isMusic)
        {
            _slider.value = _audioManager.CurrentMusicVolume;
            return;
        }

        _slider.value = _audioManager.CurrentEffectsVolume;
    }

    public void UpdateVolume(float vol)
    {
        if (isMusic)
        {
            _audioManager.UpdateMusicVolume(vol);
            return;
        }

        _audioManager.UpdateEffectsVolume(vol);
    }
}
