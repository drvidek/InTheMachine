using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameManager : MonoBehaviour
{
    private bool gameStarted;
    public bool GameStarted => gameStarted;
    public static int pixelsPerUnit = 16;

    public static ColorPalette currentColorPalette;

    public enum GameState
    {
        Play,
        Pause
    }

    private static GameState currentState;

    public static GameState CurrentState => currentState;

    public static bool IsPlaying => currentState == GameState.Play;

    #region Singleton + Awake
    private static GameManager _singleton;
    public static GameManager main
    {
        get
        {
            if (_singleton == null)
                _singleton = FindObjectOfType<GameManager>();
            return _singleton;
        }
        private set
        {
            if (_singleton == null)
            {
                _singleton = value;
            }
            else if (_singleton != value)
            {
                Debug.LogWarning("GameManager instance already exists, destroy duplicate!");
                Destroy(value);
            }
        }
    }

    private void Awake()
    {
        main = this;
    }
    #endregion

    private void OnValidate()
    {
        LoadColorPalette();
    }

    private void Start()
    {
        gameStarted = true;
        LoadColorPalette();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            if (!Shop.main.IsOpen)
                PauseMenu.main.OpenPauseMenu();
            TogglePause();
        }
    }

    private void LoadColorPalette()
    {
        currentColorPalette = Resources.Load("MainPalette") as ColorPalette;
    }

    public void TogglePause()
    {
        if (currentState == GameState.Play)
        {
            currentState = GameState.Pause;

            Time.timeScale = 0;
        }
        else
        {
            currentState = GameState.Play;
            PauseMenu.main.ClosePauseMenu();
            Shop.main.CloseShop();
            Time.timeScale = 1;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
