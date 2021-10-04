using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Canvas pauseMenuCanvas;
    [SerializeField] private Canvas settingsMenu;
    [SerializeField] private Canvas mainPauseMenu;

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
        Time.timeScale = _isPaused ? 1f : 0f;
        _isPaused = !_isPaused;

        pauseMenuCanvas.enabled = _isPaused;
        mainPauseMenu.enabled = _isPaused;
        settingsMenu.enabled = false;
    }
}
