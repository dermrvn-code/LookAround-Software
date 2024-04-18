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

    [SerializeField]
    EyesHandler eyes;

    [SerializeField]
    Slider eyeSpacing;

    [SerializeField]
    Slider heightOffset;

    [SerializeField]
    TMP_Text sceneFolderPathText;

    [SerializeField]
    Toggle loadStartScreenOnBootToggle;

    [SerializeField]
    PopUp popupPrefab;
    bool visible = false;
    public static string sceneOverviewFile = "";
    public static bool loadStartSceenOnBoot = false;


    void Awake()
    {
        LoadValues();
        sceneFolderPathText.text = sceneOverviewFile;
        loadStartScreenOnBootToggle.isOn = loadStartSceenOnBoot;
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

    public void UpdateLoadStartSceenOnBoot(bool value)
    {
        loadStartSceenOnBoot = value;
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
        loadStartSceenOnBoot = PlayerPrefs.GetInt("loadStartSceenOnBoot", 0) == 1 ? true : false;
    }

    private void SaveValues()
    {
        PlayerPrefs.SetString("sceneOverviewFile", sceneOverviewFile);
        PlayerPrefs.SetInt("loadStartSceenOnBoot", loadStartSceenOnBoot ? 1 : 0);
        PlayerPrefs.Save();
    }

}
