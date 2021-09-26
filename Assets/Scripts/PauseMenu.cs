using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Canvas pauseMenuCanvas;

    private bool _isPaused = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    private void TogglePauseMenu()
    {
        pauseMenuCanvas.enabled = !_isPaused;
        Time.timeScale = _isPaused ? 1f : 0f;

        _isPaused = !_isPaused;
    }
}
