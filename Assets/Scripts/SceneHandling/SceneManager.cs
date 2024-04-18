using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using UnityEngine;

public class SceneManager : MonoBehaviour
{

    SceneChanger sc;

    XDocument sceneOverview;

    public Dictionary<string, Scene> sceneList = new Dictionary<string, Scene>();

    void Start()
    {
        sc = FindObjectOfType<SceneChanger>();
        sc.ToMainScene();


        bool isLoaded = LoadSceneOverview();

        if (!isLoaded)
        {
            Debug.LogWarning("The scene overview file is not valid");
        }
        else if (Settings.loadStartSceenOnBoot)
        {
            sc.ToStartScene();
        }
    }


    bool LoadSceneOverview()
    {
        if (!File.Exists(Settings.sceneOverviewFile)) return false;
        sceneOverview = XDocument.Load(Settings.sceneOverviewFile);
        var scenes = sceneOverview.Descendants("Scene");

        // Loop through every Scene
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

            string mainFolder = Path.GetDirectoryName(Settings.sceneOverviewFile);
            LoadScene(sceneName, mainFolder, scenePath, isStartScene);
        }
        return true;
    }

    void LoadScene(string sceneName, string mainFolder, string scenePath, bool isStartScene)
    {
        var sceneXML = XDocument.Load(mainFolder + "/" + scenePath);

        var sceneTag = sceneXML.Element("Scene");
        string type = sceneTag.Attribute("type").Value;
        string source = sceneTag.Attribute("source").Value;

        string sceneFolder = Path.GetDirectoryName(mainFolder + "/" + scenePath);
        source = Path.Combine(sceneFolder, source);


        var elements = sceneTag.Descendants("Element");

        var sceneElements = new List<SceneElement>();

        foreach (var element in elements)
        {
            string elementType = element.Attribute("type").Value;
            string text = element.Value;
            int x = int.Parse(element.Attribute("x").Value);
            int y = int.Parse(element.Attribute("y").Value);
            string action = element.Attribute("action").Value;

            SceneElement se = new SceneElement(elementType == "text" ? SceneElement.ElementType.Text : SceneElement.ElementType.Image, text, x, y, action);
            sceneElements.Add(se);
        }

        Scene sceneObj = new Scene(type == "video" ? Scene.MediaType.Video : Scene.MediaType.Photo, sceneName, source, sceneElements, isStartScene);

        sceneList.Add(sceneName, sceneObj);
    }
}
