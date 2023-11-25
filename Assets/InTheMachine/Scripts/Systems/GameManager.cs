using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static int pixelsPerUnit = 16;

    public static ColorPalette currentColorPalette;


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
        currentColorPalette = Resources.Load("MainPalette") as ColorPalette;
    }

}
