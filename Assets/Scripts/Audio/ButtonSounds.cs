using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSounds : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{
    private AudioManager _audioManager;

    void Awake()
    {
        _audioManager = FindObjectOfType<AudioManager>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _audioManager.PlaySound(AudioManager.Sounds.ButtonHover);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _audioManager.PlaySound(AudioManager.Sounds.ButtonClick);
    }
}