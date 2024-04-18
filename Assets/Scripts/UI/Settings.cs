using System;
using System.Collections;
using System.Collections.Generic;
using SFB;
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
    public PopUp popupPrefab;
    private bool visible = false;
    public static string sceneOverviewFile = "";




    void Awake()
    {
        LoadValues();
        sceneFolderPathText.text = sceneOverviewFile;
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
        sceneOverviewFile = path;
        sceneFolderPathText.text = sceneOverviewFile;
    }

    public void SelectNewScenesFolder()
    {
        ExtensionFilter[] extensionList = new[] {
            new ExtensionFilter("XML", "xml")
        };
        string[] path = StandaloneFileBrowser.OpenFilePanel("Wähle Szenen Datei", "", extensionList, false);

        if (path.Length == 1)
        {
            UpdateFilePath(path[0]);
            var popUp = Instantiate(popupPrefab, transform);
            popUp.SetMessage("Starte die App neu, um diese Änderung wirksam zu machen");
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
        sceneOverviewFile = PlayerPrefs.GetString("sceneOverviewFile", "");
        Debug.Log("Loaded Settings values: " + sceneOverviewFile);
    }

    private void SaveValues()
    {
        PlayerPrefs.SetString("sceneOverviewFile", sceneOverviewFile);
        Debug.Log("Stored Settings values: " + sceneOverviewFile);
        PlayerPrefs.Save();
    }

}
