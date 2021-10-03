using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ButtonSound : MonoBehaviour,IPointerEnterHandler,IPointerDownHandler
{
    // Start is called before the first frame update
    [SerializeField] int clipIndexHover = 0;
    [SerializeField] int clipIndexClick = 0;
    private AudioManager _masterAudio;
    void Start()
    {
        _masterAudio = FindObjectOfType<AudioManager>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        PlaySound(clipIndexHover);

    }
    public void OnPointerDown(PointerEventData eventData)
    {
        PlaySound(clipIndexClick);
    }
    private void PlaySound(int index)
    {
        if (index >= 0)
        {
            _masterAudio.playAudioClip(index);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
