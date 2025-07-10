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
    ModelManager modelManager;

    XDocument worldOverview;
    XDocument scenesOverview;
    ProgressLoader progressLoader;
    LogoLoadingOverlay logoLoadingOverlay;
    public Dictionary<string, Scene> sceneList = new Dictionary<string, Scene>();

    void Start()
    {
        settings = FindObjectOfType<Settings>();
        sc = FindObjectOfType<SceneChanger>();
        textureManager = FindObjectOfType<TextureManager>();
        modelManager = FindObjectOfType<ModelManager>();
        progressLoader = FindObjectOfType<ProgressLoader>();
        logoLoadingOverlay = FindObjectOfType<LogoLoadingOverlay>();

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
    Dictionary<string, string> modelPaths = new Dictionary<string, string>();
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

        modelPaths = new Dictionary<string, string>();
        modelManager.UnloadAllModels();

        sceneList = new Dictionary<string, Scene>();
        string mainFolder = Path.GetDirectoryName(Settings.worldsOverviewFile);
        string scenesOverviewPath = Path.Combine(mainFolder, path);
        if (!File.Exists(scenesOverviewPath)) Debug.LogWarning("The scene overview file does not exist: " + scenesOverviewPath);
        scenesOverview = XDocument.Load(scenesOverviewPath);

        LoadScenes(scenesOverviewPath);
        LoadLogos(scenesOverviewPath);
        LoadModels(scenesOverviewPath);

        if (settings != null) settings.CloseView();

        int maxLoadingSteps = texturePaths.Count + modelPaths.Count;

        progressLoader.OnFull(() =>
        {
            onComplete?.Invoke();
        });

        StartCoroutine(textureManager.LoadAllTextures(texturePaths,
        (float progress, string path) => // onProgress
        {
            progressLoader.UpdateBarIncreaseSteps(1, maxLoadingSteps, Path.GetFileName(path));
        },
        () => // onComplete
        {
            Debug.Log("Textures preloaded!");
        }));

        foreach (var model in modelPaths)
        {
            string modelName = model.Key;
            string modelPath = model.Value;

            modelManager.LoadModel(modelPath, modelName, () =>
            {
                progressLoader.UpdateBarIncreaseSteps(1, maxLoadingSteps, Path.GetFileName(modelPath));
            });
        }

    }


    void LoadScenes(string scenesOverviewPath)
    {
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
    }

    void LoadLogos(string scenesOverviewPath)
    {
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
                        logoLoadingOverlay.LoadLogo(id, logoPath, backgroundColor);
                    }
                    else
                    {
                        Debug.LogWarning("Logo file does not exist: " + logoPath);
                    }
                }
            }
        }
    }

    void LoadModels(string scenesOverviewPath)
    {
        var modelsList = scenesOverview.Root.Element("Models");
        if (modelsList != null)
        {
            var models = modelsList.Descendants("Model");
            foreach (var model in models)
            {
                string modelSource = model.Attribute("source").Value;
                string modelName = model.Attribute("name").Value;

                string modelPath = Path.Combine(Path.GetDirectoryName(scenesOverviewPath), modelSource);

                if (File.Exists(modelPath))
                {
                    modelPaths.Add(modelName, modelPath);
                    return;
                }
                Debug.LogWarning("Model file does not exist: " + modelPath);
            }
        }
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
                se = new SceneElementText(
                        text: text,
                        x: x, y: y,
                        distance: distance,
                        xRotationOffset: xRotationOffset,
                        action: action
                    );
            }
            else if (elementType == "textbox")
            {
                string icon = element.Attribute("icon").Value;
                se = new SceneElementTextbox(
                        text: text, icon: icon,
                        x: x, y: y,
                        distance: distance,
                        xRotationOffset: xRotationOffset
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

                string icon = "info";
                if (element.Attribute("icon") != null)
                {
                    icon = element.Attribute("icon").Value;
                }

                se = new SceneElementArrow(
                        x: x, y: y,
                        distance: distance,
                        xRotationOffset: xRotationOffset,
                        icon: icon, rotation: rotation,
                        color: color, action: action
                    );

            }
            else if (elementType == "model")
            {
                string name = element.Attribute("name").Value;
                string action = element.Attribute("action").Value;

                se = new SceneElementModel(
                        modelName: name,
                        x: x, y: y,
                        distance: distance,
                        xRotationOffset: xRotationOffset,
                        action: action
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
