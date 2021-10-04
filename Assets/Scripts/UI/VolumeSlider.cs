using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    private Slider _slider;
    private AudioManager _masterAudio;
    // Start is called before the first frame update
    private void Awake()
    {
        _slider = GetComponent<Slider>();
        _masterAudio = FindObjectOfType<AudioManager>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeVolumeSFX()
    {
        //Add Functionality Here
        Debug.Log("Volume Changed SFX");
    }
    public void ChangeVolumeMusic()
    {
        //Add Functionality Here
        Debug.Log("Volume Changed Music");
    }
}
