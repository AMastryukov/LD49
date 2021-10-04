using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Canvas pauseMenuCanvas;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject mainPauseMenu;

    private bool _isPaused = false;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        if(settingsMenu.activeSelf)
        {
            settingsMenu.SetActive(false);
            mainPauseMenu.SetActive(true);
        }
        else
        {
            pauseMenuCanvas.enabled = !_isPaused;
            Time.timeScale = _isPaused ? 1f : 0f;

            _isPaused = !_isPaused;
        }
     
    }
}
