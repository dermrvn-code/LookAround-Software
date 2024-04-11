using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{

    public EyesHandler eyes;
    public Slider eyeSpacing;
    public Slider heightOffset;
    public TMP_Text sceneFolderPathText;
    private bool visible = false;
    public static string scenesFolder = "";




    void Awake()
    {
        LoadValues();
        sceneFolderPathText.text = scenesFolder;
    }

    void Start()
    {
        ToggleView(!visible);
        if (eyes == null)
        {
            Debug.LogError("No eyes were given in the Hardware SettingsWrap");
            return;
        }
        eyeSpacing.value = eyes.eyeSpacing;
        heightOffset.value = eyes.heightOffset;
    }

    void OnApplicationQuit()
    {
        SaveValues();
    }

    public void UpdateFilePath(string path)
    {
        scenesFolder = path;
        sceneFolderPathText.text = scenesFolder;
    }

    public void SelectNewScenesFolder()
    {
        string path = EditorUtility.OpenFolderPanel("Wähle einen Ordner", "", "");

        if (path != null)
        {
            UpdateFilePath(path);
        }
    }

    public void ChangeEyeSpacing(float spacing)
    {
        eyes.eyeSpacing = spacing;
    }

    public void ChangeOffsetHeight(float offset)
    {
        eyes.heightOffset = offset;
    }

    public void CloseView()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        visible = false;
    }

    public void OpenView()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
        visible = true;
    }

    public void ToggleView()
    {
        ToggleView(visible);
    }

    private void ToggleView(bool visible)
    {
        if (visible)
        {
            CloseView();
            return;
        }
        OpenView();
    }

    private void LoadValues()
    {
        scenesFolder = PlayerPrefs.GetString("scenesFolder", "");
    }

    private void SaveValues()
    {
        PlayerPrefs.SetString("scenesFolder", scenesFolder);
        PlayerPrefs.Save();
    }

}
