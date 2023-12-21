using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameManager : MonoBehaviour
{
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
        get => _singleton;
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
        LoadColorPalette();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
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
            Time.timeScale = 1;
        }
    }

}
