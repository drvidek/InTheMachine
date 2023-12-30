using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Camera mapCamera;
    [SerializeField] private float mapPanSpeed = 48f;
    [SerializeField] private GameObject defaultSelection, cancelButton, mapObject;
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private float zoomMin = 1f, zoomMax = 9f;
    private GameObject pauseMenu;
    private Vector3 mapCameraLocalHome;
    private float oneRoomZoom = 9f;
    private float zoomMultilpier = 3f;
    private float defaultZoom = 27f;

    #region Singleton + Awake
    private static PauseMenu _singleton;
    public static PauseMenu main
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
                Debug.LogWarning("PauseMenu instance already exists, destroy duplicate!");
                Destroy(value);
            }
        }
    }

    private void Awake()
    {
        main = this;
    }

    private void OnDisable()
    {
        if (main == this)
            _singleton = null;
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        mapCameraLocalHome = mapCamera.transform.localPosition;
        pauseMenu = transform.GetChild(0).gameObject;
    }

    private void Update()
    {
        if (!pauseMenu.activeInHierarchy)
            return;

        if (eventSystem.currentSelectedGameObject == mapObject)
        {
            Vector2 input = new(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            if (input.magnitude == 0)
            {

            }
            mapCamera.transform.position += (Vector3)input * mapPanSpeed * Time.unscaledDeltaTime;

            if (Input.GetButtonDown("Shoot"))
            {
                ResetMapCameraPosition();
            }
            if (Input.GetButtonDown("Heal"))
            {
                zoomMultilpier = Mathf.Min(zoomMultilpier + 2f, zoomMax);
                mapCamera.orthographicSize = zoomMultilpier * oneRoomZoom;
            }
            if (Input.GetButtonDown("Special"))
            {
                zoomMultilpier = Mathf.Max(zoomMultilpier - 2f, zoomMin);
                mapCamera.orthographicSize = zoomMultilpier * oneRoomZoom;
            }
            if (Input.GetButtonDown("Boost"))
            {
                eventSystem.SetSelectedGameObject(cancelButton);
            }
        }
    }

    private void ResetMapCameraPosition()
    {
        mapCamera.transform.localPosition = mapCameraLocalHome;
    }

    private void ResetMapCameraZoom()
    {
        mapCamera.orthographicSize = defaultZoom;
    }

    public void OpenPauseMenu()
    {
        pauseMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(defaultSelection);
    }

    public void ClosePauseMenu()
    {
        ResetMapCameraPosition();
        ResetMapCameraZoom();
        pauseMenu.SetActive(false);
    }
}
