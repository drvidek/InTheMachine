using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
	[SerializeField] private Vector2 resolution = new(4,3);
    public static float screenWidth, screenHeight;
	private Camera _camera;
   

    new public Camera camera => _camera;

	#region Singleton + Awake
	private static CameraController _singleton;
	public static CameraController main
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
				Debug.LogWarning("CameraController instance already exists, destroy duplicate!");
				Destroy(value);
			}
		}
	}

	private void Awake()
	{
		main = this;
	}
    #endregion

    private void Start()
    {
		_camera = GetComponent<Camera>();
		screenHeight = camera.orthographicSize * 2;
		screenWidth = screenHeight / resolution.y * resolution.x;
    }

    private void FixedUpdate()
    {
		float xDist = QMath.Difference(Player.main.X, transform.position.x);
		float yDist = QMath.Difference(Player.main.Y, transform.position.y);

		MoveCameraHorizontal(xDist > screenWidth / 2);
		MoveCameraVertical(yDist > screenHeight / 2);
		
    }

	private void MoveCameraHorizontal(bool move)
	{
		if (!move)
			return;
		Vector2 direction = new Vector2(Mathf.Sign(Player.main.X - transform.position.x),0) * screenWidth;
		MoveCamera(direction);
	}

    private void MoveCameraVertical(bool move)
    {
        if (!move)
            return;
        Vector2 direction = new Vector2(0,Mathf.Sign(Player.main.Y - transform.position.y)) * screenHeight;
        MoveCamera(direction);
    }

    private void MoveCamera(Vector2 direction)
	{
		transform.position += (Vector3)direction;
	}
}
