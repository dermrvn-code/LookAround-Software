using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

public class SceneManager : MonoBehaviour
{

    SceneChanger sc;
    Settings settings;
    TextureManager textureManager;

    XDocument worldOverview;
    XDocument scenesOverview;
    ProgressLoader progressLoader;
    LogoLoadingOverlay lolo;
    public Dictionary<string, Scene> sceneList = new Dictionary<string, Scene>();

    void Start()
    {
        settings = FindObjectOfType<Settings>();
        sc = FindObjectOfType<SceneChanger>();
        textureManager = FindObjectOfType<TextureManager>();
        progressLoader = FindObjectOfType<ProgressLoader>();
        lolo = FindObjectOfType<LogoLoadingOverlay>();

        if (isSceneBuilder()) return;
        sc.ToMainScene();


        bool isLoaded = LoadWorldOverview();

        if (!isLoaded)
        {
            Debug.LogWarning("The world overview file is not valid");
            return;
        }
        else if (Settings.loadWorldOnBoot)
        {
            sc.LoadWorld();
        }
        Debug.Log("World overview loaded!");
    }

    public static bool isSceneBuilder()
    {
        return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "SceneBuilder";
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

            worldsList.Add(name, path);
        }
        if (currentWorld == "") currentWorld = worldsList.First().Key;
        settings.PopulateWorldDropdown();
        return true;
    }

    List<string> texturePaths;
    string currentScenesOverviewPath;
    public void LoadScenesOverview(string path, Action onComplete)
    {
        if (currentScenesOverviewPath == path)
        {
            sc.ToStartScene();
            if (settings != null) settings.CloseView();
            return;
        }
        currentScenesOverviewPath = path;
        texturePaths = new List<string>();
        textureManager.ReleaseAllTextures();

        sceneList = new Dictionary<string, Scene>();
        string mainFolder = Path.GetDirectoryName(Settings.worldsOverviewFile);
        string scenesOverviewPath = Path.Combine(mainFolder, path);
        if (!File.Exists(scenesOverviewPath)) Debug.LogWarning("The scene overview file does not exist: " + scenesOverviewPath);
        scenesOverview = XDocument.Load(scenesOverviewPath);
        var scenesList = scenesOverview.Root.Element("Scenes");
        var scenes = scenesList.Descendants("Scene");

        int counter = 0;
        foreach (var scene in scenes)
        {
            string scenePath = scene.Attribute("path").Value;
            string sceneName = scene.Attribute("name").Value;

            var startScene = scene.Attribute("startScene");
            bool isStartScene = false;
            if (startScene != null)
            {
                if (startScene.Value.ToLower() == "true") isStartScene = true;
            }

            string sceneFolder = Path.GetDirectoryName(scenesOverviewPath);
            Scene s = LoadScene(sceneName, sceneFolder, scenePath, isStartScene);

            if (s.Type != Scene.MediaType.Photo) return;

            if (s.IsStartScene)
            {
                texturePaths.Insert(0, s.Source);
            }
            else
            {
                texturePaths.Add(s.Source);
            }
            counter++;
        }


        var logoList = scenesOverview.Root.Element("Logos");
        if (logoList != null)
        {
            var logos = logoList.Descendants("Logo");
            foreach (var logo in logos)
            {
                string logoSource = logo.Attribute("source").Value;
                string id_str = logo.Attribute("id").Value;
                string backgroundColor = logo.Attribute("backgroundColor")?.Value ?? "";

                if (int.TryParse(id_str, out int id))
                {
                    string logoPath = Path.Combine(Path.GetDirectoryName(scenesOverviewPath), logoSource);
                    if (File.Exists(logoPath))
                    {
                        lolo.LoadLogo(id, logoPath, backgroundColor);
                    }
                    else
                    {
                        Debug.LogWarning("Logo file does not exist: " + logoPath);
                    }
                }
            }
        }

        if (settings != null) settings.CloseView();

        StartCoroutine(textureManager.LoadAllTextures(texturePaths, () =>
        {
            Debug.Log("Textures preloaded!");
            onComplete?.Invoke();
        }));
    }

    Scene LoadScene(string sceneName, string mainFolder, string scenePath, bool isStartScene)
    {
        var sceneXML = XDocument.Load(mainFolder + "/" + scenePath);

        var sceneTag = sceneXML.Element("Scene");
        string type = sceneTag.Attribute("type").Value;
        string source = sceneTag.Attribute("source").Value;


        float xOffset = 0;
        float yOffset = 0;
        if (sceneTag.Attribute("xOffset") != null)
        {
            xOffset = float.Parse(sceneTag.Attribute("xOffset").Value);
        }
        if (sceneTag.Attribute("yOffset") != null)
        {
            yOffset = float.Parse(sceneTag.Attribute("yOffset").Value);
        }

        string sceneFolder = Path.GetDirectoryName(mainFolder + "/" + scenePath);
        source = Path.Combine(sceneFolder, source);


        var elements = sceneTag.Descendants("Element");

        var sceneElements = new List<SceneElement>();

        foreach (var element in elements)
        {
            string elementType = element.Attribute("type").Value.ToLower();

            string text = element.Value.Trim();
            if (text == "")
            {
                text = "No Text given";
            }

            int x = int.Parse(element.Attribute("x").Value);
            int y = int.Parse(element.Attribute("y").Value);

            int distance = 10;
            if (element.Attribute("distance") != null)
            {
                distance = int.Parse(element.Attribute("distance").Value);
            }

            int xRotationOffset = 0;
            if (element.Attribute("xRotationOffset") != null)
            {
                xRotationOffset = int.Parse(element.Attribute("xRotationOffset").Value);
            }


            SceneElement se;
            if (elementType == "text")
            {
                string action = element.Attribute("action").Value;
                se = new SceneElement(
                        SceneElement.ElementType.Text,
                        text, x, y,
                        distance, xRotationOffset,
                        action: action
                    );
            }
            else if (elementType == "textbox")
            {
                string icon = element.Attribute("icon").Value;
                se = new SceneElement(
                        SceneElement.ElementType.Textbox,
                        text, x, y,
                        distance, xRotationOffset,
                        icon: icon
                    );
            }
            else if (elementType == "directionarrow")
            {
                string action = element.Attribute("action").Value;
                int rotation = int.Parse(element.Attribute("rotation").Value);

                string color = "";
                if (element.Attribute("color") != null)
                {
                    color = element.Attribute("color").Value;
                }

                se = new SceneElement(
                        SceneElement.ElementType.DirectionArrow,
                        text, x, y,
                        distance, xRotationOffset,
                        action: action, rotation: rotation, color: color
                    );

            }
            else
            {
                Debug.Log("Element doesnt match any type : " + elementType);
                se = null;
            }
            if (se != null)
            {
                sceneElements.Add(se);
            }
        }
        Scene sceneObj = new Scene(type == "video" ? Scene.MediaType.Video : Scene.MediaType.Photo, sceneName, source, sceneElements, isStartScene, xOffset, yOffset);

        sceneList.Add(sceneName, sceneObj);
        return sceneObj;
    }


    void OnDestroy()
    {
        // Release all textures when done
        textureManager.ReleaseAllTextures();
    }
}
