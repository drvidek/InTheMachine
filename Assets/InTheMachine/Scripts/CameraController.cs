using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [SerializeField] private Vector2 resolution = new(4, 3);
    [SerializeField] private float zoomTime, zoomDelay, screen;
    public static float screenWidth, screenHeight;
    private Camera _camera;

    bool specialCamLock = false;
    private Vector3 lockedPosition = new();
    private Vector3 lerpBasePosition = new();
    private Vector3 specialRoomSize;

    private Vector3 cameraOffset;

    private Alarm zoomAlarm;
    private Alarm zoomDelayAlarm;

    new public Camera camera => _camera;


    private Vector3 targetPosition;
    private Vector3 lastPosition;
    private float targetChangeTime;


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
        zoomAlarm = Alarm.Get(zoomTime, false, false);
        zoomDelayAlarm = Alarm.Get(zoomDelay, false, false);
        zoomDelayAlarm.onComplete += () => zoomAlarm.ResetAndPlay();

        _camera = GetComponent<Camera>();
        screenHeight = camera.orthographicSize * 2;
        screenWidth = screenHeight / resolution.y * resolution.x;
        cameraOffset = new Vector3(screenWidth / 2f, screenHeight / 2f, -50f);
        RoomManager.main.onPlayerMovedRoom += SetNewTarget;// SnapToRoom;

        SnapToRoom(RoomManager.main.currentRoom);
    }

    private void SetNewTarget(Vector3Int room)
    {
        targetPosition = RoomManager.main.RoomGrid.CellToWorld(room) + cameraOffset;
        lastPosition = transform.position;
        targetChangeTime = Time.time;

       
    }


    private void Update()
    {
        if (!specialCamLock)
        {
            transform.position = Vector3.Lerp(lastPosition, targetPosition, (Time.time - targetChangeTime) * 5f);
            if (Time.time - targetChangeTime >= 1)
            {
                BoxCollider2D collider = FindCameraVolume() as BoxCollider2D;
                if (collider)
                {
                    lockedPosition = collider.transform.position;
                    lerpBasePosition = transform.position;
                    zoomDelayAlarm.ResetAndPlay();
                    specialCamLock = true;
                    specialRoomSize = collider.size;
                }
            }
            return;
        }

        if (zoomAlarm.IsPlaying)
        {
            transform.position = Vector3.Lerp(lerpBasePosition, lockedPosition, zoomAlarm.PercentComplete);
            camera.orthographicSize = Mathf.Lerp(screenHeight / 2, screenHeight, zoomAlarm.PercentComplete);
        }

        if (QMath.Difference(lockedPosition.x, Player.main.X) > specialRoomSize.x / 2f || QMath.Difference(lockedPosition.y, Player.main.Y) > specialRoomSize.y / 2f)
        {
            specialCamLock = false;
            camera.orthographicSize = screenHeight / 2f;
            SnapToRoom(RoomManager.main.currentRoom);
            zoomDelayAlarm.Stop();
            zoomAlarm.Stop();
        }
    }

    public void SnapToRoom(Vector3Int room)
    {
        if (specialCamLock)
            return;

        transform.position = RoomManager.main.RoomGrid.CellToWorld(room) + cameraOffset;
        BoxCollider2D collider = FindCameraVolume() as BoxCollider2D;
        if (collider)
        {
            lockedPosition = collider.transform.position;
            lerpBasePosition = transform.position;
            zoomDelayAlarm.ResetAndPlay();
            specialCamLock = true;
            specialRoomSize = collider.size;
        }
    }

    private Collider2D FindCameraVolume()
    {
        return Physics2D.OverlapPoint(transform.position, 1 << 5);
    }



}
