using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class TileSounds : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{
    // Start is called before the first frame update
    [SerializeField] int clipIndexHover = 0;
    [SerializeField] int clipIndexClick = 0;
    private AudioManager _masterAudio;
    private Tile _tile;
    private void Awake()
    {
        _tile = gameObject.GetComponent<Tile>();
    }
    void Start()
    {
        _masterAudio = FindObjectOfType<AudioManager>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(_tile.tileState == ETileState.Neutral)
        {
            PlaySound(clipIndexHover);
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if(_tile.tileState==ETileState.Neutral)
        {
            PlaySound(clipIndexClick);
        }
    }

    private void PlaySound(int index)
    {
        if(index>=0)
        {
            _masterAudio.playAudioClip(index);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
