using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

public class SceneManager : SceneManagerBase
{
    Settings settings;

    public override void Start()
    {
        base.Start();

        SceneChanger sceneChangerExt = (SceneChanger)sceneChanger;

        settings = FindFirstObjectByType<Settings>();
        sceneChangerExt.ToMainScene();

        bool isLoaded = LoadWorldOverview();

        if (!isLoaded)
        {
            Debug.LogWarning("The world overview file is not valid");
            return;
        }
        else if (Settings.loadWorldOnBoot)
        {
            sceneChangerExt.LoadWorld();
        }
        Debug.Log("World overview loaded!");
    }

    public static Dictionary<string, string> worldsList = new Dictionary<string, string>();
    public static string currentWorld = "";
    bool LoadWorldOverview()
    {
        if (!File.Exists(Settings.worldsOverviewFile)) return false;
        worldOverview = XDocument.Load(Settings.worldsOverviewFile);
        var worlds = worldOverview.Descendants("World");

        worldsList = new Dictionary<string, string>();
        // Loop through every Scene
        foreach (var world in worlds)
        {
            string path = world.Attribute("path").Value;
            string name = world.Attribute("name").Value;

            string folder = Path.GetDirectoryName(Settings.worldsOverviewFile);
            string wholePath = Path.Combine(folder, path);

            worldsList.Add(name, wholePath);
        }
        if (currentWorld == "") currentWorld = worldsList.First().Key;
        settings.PopulateWorldDropdown();
        return true;
    }
}
