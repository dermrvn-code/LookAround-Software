using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DebugConsole : MonoBehaviour
{
    public static TMP_Text consoleText;

    public static TMP_Text hudText;

    private static bool consoleActive = false;

    void Start()
    {
        consoleText = GetComponent<TMP_Text>();
        hudText = GameObject.Find("HUD").GetComponent<TMP_Text>();

    }

    public static void ToggleActivation()
    {
        consoleActive = !consoleActive;
        consoleText.enabled = consoleActive;
        hudText.enabled = consoleActive;

        if (consoleActive) Log("Console activated");
    }

    public static void Log(string text)
    {
        if (consoleActive)
        {
            var clearAfterLines = 25;
            var oldText = consoleText.text;
            var oldLines = oldText.Split("\n");
            if (oldLines.Length > clearAfterLines)
            {
                oldLines = oldLines.Skip(clearAfterLines).ToArray();
                oldText = string.Join("\n", oldLines);
            }
            consoleText.text = oldText + "\n" + "[" + DateTime.Now.ToString() + "] " + text;
        }
    }


    public static void LogValues(int rotation, bool interaction1, bool interaction2, int zoom)
    {
        if (consoleActive)
        {
            hudText.text = "Rotation: " + rotation + "\n";
            hudText.text += "Zoom: " + zoom + "\n";
            hudText.text += "Interaction 1: " + interaction1 + "\n";
            hudText.text += "Interaction 2: " + interaction2;
        }
    }
}
