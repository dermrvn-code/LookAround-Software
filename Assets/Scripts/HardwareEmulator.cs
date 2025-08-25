using UnityEngine;

public class HardwareEmulator : MonoBehaviour
{
    public EyesHandler eyes;
    public Settings settings;
    InteractionHandler interactionHandler;
    SceneChanger sceneChanger;

    void Start()
    {
        sceneChanger = FindFirstObjectByType<SceneChanger>();
        interactionHandler = FindFirstObjectByType<InteractionHandler>();
        if (eyes == null) Debug.LogError("No eyes were given in the Hardware Emulator");
        if (settings == null)
        {
            Debug.LogWarning("No settings were given in the Hardware Emulator, but it's a Scene Builder");
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (!settings.IsVisible)
            {
                eyes.LeftMove();
            }
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            if (!settings.IsVisible)
            {
                eyes.RightMove();
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (settings.IsVisible)
            {
                settings.ShiftElement(-1);
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (settings.IsVisible)
            {
                settings.ShiftElement(1);
            }
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            if (!settings.IsVisible)
            {
                eyes.ZoomIn();
            }
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            if (!settings.IsVisible)
            {
                eyes.ZoomOut();
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (settings.IsVisible)
            {
                settings.MoveSelector(1);
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (settings.IsVisible)
            {
                settings.MoveSelector(-1);
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            settings.ToggleView();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (settings.IsVisible)
            {
                settings.SelectElement();
            }
            else
            {
                interactionHandler.Interact();
            }
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (!settings.IsVisible)
            {
                sceneChanger.ToStartScene();
            }
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            DebugConsole.ToggleActivation();
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            eyes.ToggleSplitScreen();
        }
    }
}
