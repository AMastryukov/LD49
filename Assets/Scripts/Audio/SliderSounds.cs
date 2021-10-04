using UnityEngine;
using UnityEngine.EventSystems;

public class SliderSounds : MonoBehaviour,IPointerDownHandler
{
    private AudioManager _audioManager;

    void Awake()
    {
        _audioManager = FindObjectOfType<AudioManager>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _audioManager.PlaySound(AudioManager.Sounds.ButtonClick);
    }
}