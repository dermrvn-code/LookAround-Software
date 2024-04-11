using UnityEngine;

public class HardwareEmulator : MonoBehaviour
{
    public EyesHandler eyes;
    public Settings settings;
    public InteractionHandler interaction;



    void Start()
    {
        if (eyes == null) Debug.LogError("No eyes were given in the Hardware Emulator");
        if (settings == null) Debug.LogError("No settings were given in the Hardware Emulator");
        if (interaction == null) Debug.LogError("No interaction were given in the Hardware Emulator");
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            eyes.LeftMove();
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            eyes.RightMove();
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            eyes.ZoomIn();
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            eyes.ZoomOut();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            settings.ToggleView();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            interaction.Interact();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            DebugConsole.ToggleActivation();
        }
    }
}
