using UnityEditor;
using UnityEngine;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class SceneEditorWindow : EditorWindow
{
    public static string exportFolder = "SceneBuilder/Exports/";
    public static string worldName = "WorldName";
    public static string overviewFilePath = exportFolder + worldName.Replace(" ", "").ToLower() + ".xml";
    private List<SceneData> scenes = new List<SceneData>();
    private Dictionary<string, Sprite> availableIcons = new Dictionary<string, Sprite>();

    [MenuItem("Tools/Scene Editor")]
    public static void ShowWindow()
    {
        GetWindow<SceneEditorWindow>("Scene Editor");
    }

    private void OnEnable()
    {
        LoadAvailableIcons();
    }

    private void LoadAvailableIcons()
    {
        Debug.Log("Load icons");
        availableIcons.Clear();
        foreach (var icon in SpriteHolder.icons)
        {
            Debug.Log("Loading " + icon.Key);
            availableIcons[icon.Key] = icon.Value;
        }
    }


    private void OnGUI()
    {
        GUILayout.Label("Scene Overview", EditorStyles.boldLabel);

        worldName = EditorGUILayout.TextField("World Name", worldName).Replace(" ", "");
        GUILayout.Space(10);

        foreach (var scene in scenes)
        {
            GUILayout.BeginVertical("box");
            GUILayout.Label("Scene Configuration", EditorStyles.boldLabel);

            scene.name = EditorGUILayout.TextField("Scene Name", scene.name).Replace(" ", "");
            scene.folder = scene.name.Replace(" ", "").ToLower();
            scene.path = scene.folder + "/scene.xml";
            EditorGUILayout.LabelField("XML Path: " + scene.path);


            GUILayout.Space(10);
            // File Select Button
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Select Source File"))
            {
                scene.sourceImageToCopy = EditorUtility.OpenFilePanel("Select Scene Source File", "", "jpg,png,mp4"); // Adjust file filters if necessary
                if (!string.IsNullOrEmpty(scene.sourceImageToCopy))
                {
                    scene.filename = Path.GetFileName(scene.sourceImageToCopy);
                    scene.source = scene.folder + "/" + scene.filename.Replace(" ", "").ToLower();

                    string fileExtension = Path.GetExtension(scene.sourceImageToCopy);
                    if (fileExtension == ".jpg" || fileExtension == ".png")
                    {
                        scene.type = "image";
                    }
                    else if (fileExtension == ".mp4")
                    {
                        scene.type = "video";
                    }
                    else
                    {
                        scene.type = "unknown";
                    }
                }
            }
            EditorGUILayout.LabelField("File Path: " + scene.source);
            GUILayout.EndHorizontal();
            EditorGUILayout.LabelField("Type: " + scene.type);

            GUILayout.Space(10);

            scene.xOffset = Mathf.Clamp(EditorGUILayout.FloatField("X Offset", scene.xOffset), 0f, 1f);
            scene.yOffset = Mathf.Clamp(EditorGUILayout.FloatField("Y Offset", scene.yOffset), 0f, 1f);

            bool isStartScene = EditorGUILayout.Toggle("Start Scene", scene.startScene);
            if (isStartScene && !scene.startScene)
            {
                DeselectOtherStartScenes();
                scene.startScene = true;
            }
            else if (!isStartScene && scene.startScene)
            {
                scene.startScene = false;
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Edit Scene Elements"))
            {
                SceneElementEditorWindow.ShowWindow(scene, availableIcons, scenes);
            }
            if (GUILayout.Button("Remove"))
            {
                scenes.Remove(scene);
                break;
            }
            GUILayout.EndHorizontal();


            if (GUILayout.Button("Load Scene"))
            {
                scene.LoadScene();
            }

            GUILayout.EndVertical();

            GUILayout.Space(30); // Space between scenes
        }

        if (GUILayout.Button("Add New Scene"))
        {
            SceneData sd = new SceneData();
            sd.name = "Scene " + scenes.Count + 1;
            scenes.Add(sd);
        }

        GUILayout.Space(10); // Space before scenes

        if (GUILayout.Button("Save Overview"))
        {
            SaveSceneOverview();
        }
    }


    private void DeselectOtherStartScenes()
    {
        foreach (var scene in scenes)
        {
            scene.startScene = false;
        }
    }

    private void SaveSceneOverview()
    {
        XmlDocument doc = new XmlDocument();
        XmlElement root = doc.CreateElement("Scenes");

        foreach (var scene in scenes)
        {
            XmlElement sceneElement = doc.CreateElement("Scene");
            sceneElement.SetAttribute("path", scene.path);
            sceneElement.SetAttribute("name", scene.name);
            if (scene.startScene)
            {
                sceneElement.SetAttribute("startScene", "true");
            }
            root.AppendChild(sceneElement);

            scene.SaveScene();
        }

        doc.AppendChild(root);

        string directoryPath = Path.GetDirectoryName(overviewFilePath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        doc.Save(overviewFilePath);

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Scene Overview Saved", "Scene overview saved successfully!", "OK");
    }

    public class SceneData
    {
        public string name;
        public string path;
        public float xOffset;
        public float yOffset;
        public bool startScene;
        public string type;
        public string sourceImageToCopy;
        public string source;
        public string filename;
        public string folder;
        public List<SceneElement> elements = new List<SceneElement>();

        public void SaveScene()
        {
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("Scene");
            root.SetAttribute("type", type);
            root.SetAttribute("source", filename); // Default source, can be customized
            root.SetAttribute("xOffset", "" + xOffset);
            root.SetAttribute("yOffset", "" + yOffset);

            foreach (var element in elements)
            {
                XmlElement elementNode = doc.CreateElement("Element");
                elementNode.SetAttribute("x", element.x.ToString());
                elementNode.SetAttribute("y", element.y.ToString());
                elementNode.SetAttribute("type", element.type.ToString());

                switch (element.type)
                {
                    case SceneElement.ElementType.DirectionArrow:
                        elementNode.SetAttribute("rotation", element.rotation.ToString());
                        elementNode.SetAttribute("action", element.action);
                        break;

                    case SceneElement.ElementType.Textbox:
                        elementNode.SetAttribute("icon", element.icon);
                        elementNode.InnerText = element.text;
                        break;

                    case SceneElement.ElementType.Text:
                        elementNode.SetAttribute("action", element.action);
                        elementNode.InnerText = element.text;
                        break;
                }

                root.AppendChild(elementNode);
            }

            doc.AppendChild(root);

            string directoryPath = Path.GetDirectoryName(overviewFilePath);
            string sceneFile = Path.Combine(directoryPath, path);
            string sceneFileFolder = Path.GetDirectoryName(sceneFile);
            if (!Directory.Exists(sceneFileFolder))
            {
                Directory.CreateDirectory(sceneFileFolder);
            }

            doc.Save(sceneFile);
            CopyImage(this);

            string xmlString = doc.OuterXml;
            Debug.Log(xmlString);
        }

        void CopyImage(SceneData scene)
        {
            string directoryPath = Path.GetDirectoryName(overviewFilePath);
            string destinationPath = Path.Combine(directoryPath, scene.source);

            File.Copy(scene.sourceImageToCopy, destinationPath, true);
        }

        public void LoadScene()
        {
            SceneChanger sc = FindObjectOfType<SceneChanger>();
            Scene.MediaType type = this.type == "image" ? Scene.MediaType.Photo : Scene.MediaType.Video;
            Scene s = new Scene(type, this.name, this.sourceImageToCopy, this.elements, false, xOffset: this.xOffset, yOffset: this.yOffset);
            sc.SwitchScene(s);
        }
    }
}

public class SceneElementEditorWindow : EditorWindow
{
    private SceneEditorWindow.SceneData scene;
    private Dictionary<string, Sprite> availableIcons;
    private List<SceneEditorWindow.SceneData> allScenes;

    public static void ShowWindow(SceneEditorWindow.SceneData scene, Dictionary<string, Sprite> availableIcons, List<SceneEditorWindow.SceneData> allScenes)
    {
        SceneElementEditorWindow window = GetWindow<SceneElementEditorWindow>("Scene Elements Editor");
        window.scene = scene;
        window.availableIcons = availableIcons;
        window.allScenes = allScenes;
        window.Show();
    }

    private void ActionHandler(SceneElement element)
    {
        int selectedActionIndex = 0;
        if (element.action != null)
        {
            string oldActionType = element.action.Split("(")[0];
            selectedActionIndex = System.Array.IndexOf(SceneChanger.actionTypes, oldActionType);
        }

        // Action Type Dropdown
        selectedActionIndex = Mathf.Max(selectedActionIndex, 0);
        selectedActionIndex = EditorGUILayout.Popup("Action Type", selectedActionIndex, SceneChanger.actionTypes);
        string actionType = SceneChanger.actionTypes[selectedActionIndex];
        // Scene Dropdown for "toScene" action
        if (actionType == "toScene")
        {
            string[] sceneNames = allScenes.ConvertAll(sceneData => sceneData.name).ToArray();
            int selectedSceneIndex = 0;
            if (element.action != null)
            {
                string oldActionScene = element.action.Split("(")[1].Replace(")", "");
                selectedSceneIndex = System.Array.IndexOf(sceneNames, oldActionScene);
            }
            selectedSceneIndex = Mathf.Max(selectedSceneIndex, 0);
            selectedSceneIndex = EditorGUILayout.Popup("Scene", selectedSceneIndex, sceneNames);
            string actionScene = sceneNames[selectedSceneIndex];
            element.action = actionType + "(" + actionScene + ")";
        }
        else
        {
            element.action = "";
        }
    }

    private void OnGUI()
    {
        if (scene == null || availableIcons == null || allScenes == null) return;

        GUILayout.Label("Scene: " + scene.name, EditorStyles.boldLabel);

        if (GUILayout.Button("Add New Element"))
        {
            SceneElement se = new SceneElement();
            se.distance = 10;
            scene.elements.Add(se);
            scene.LoadScene();  // Reload scene when a new element is added
        }

        GUILayout.Space(10); // Space before elements

        EditorGUI.BeginChangeCheck(); // Begin tracking changes

        foreach (var element in scene.elements)
        {
            GUILayout.BeginVertical("box");
            GUILayout.Label("Element Configuration", EditorStyles.boldLabel);

            element.x = EditorGUILayout.IntField("X Position", element.x);
            element.y = EditorGUILayout.IntField("Y Position", element.y);
            element.distance = EditorGUILayout.IntField("Distance", element.distance);
            element.type = (SceneElement.ElementType)EditorGUILayout.EnumPopup("Type", element.type);

            switch (element.type)
            {
                case SceneElement.ElementType.DirectionArrow:
                    element.rotation = Mathf.Clamp(EditorGUILayout.IntField("Rotation", element.rotation), 0, 360);
                    ActionHandler(element);
                    break;

                case SceneElement.ElementType.Textbox:
                    GUILayout.BeginHorizontal();
                    string[] iconNames = new string[availableIcons.Count];
                    availableIcons.Keys.CopyTo(iconNames, 0);
                    int selectedIconIndex = System.Array.IndexOf(iconNames, element.icon);
                    selectedIconIndex = Mathf.Max(selectedIconIndex, 0); // Ensure a valid index
                    selectedIconIndex = EditorGUILayout.Popup("Icon", selectedIconIndex, iconNames);

                    element.icon = iconNames[selectedIconIndex];

                    if (availableIcons.TryGetValue(element.icon, out Sprite icon))
                    {
                        GUILayout.Label(icon.texture, GUILayout.Width(50), GUILayout.Height(50)); // Preview the selected icon
                    }
                    GUILayout.EndHorizontal();

                    EditorGUILayout.LabelField("Textbox Content:");
                    element.text = EditorGUILayout.TextArea(element.text, GUILayout.Height(100)); // Adjust height as needed
                    break;

                case SceneElement.ElementType.Text:
                    element.text = EditorGUILayout.TextField("Text", element.text);
                    ActionHandler(element);
                    break;
            }

            if (GUILayout.Button("Remove"))
            {
                scene.elements.Remove(element);
                scene.LoadScene();  // Reload scene when an element is removed
                break;
            }

            GUILayout.EndVertical();

            GUILayout.Space(20); // Space between elements
        }

        if (EditorGUI.EndChangeCheck()) // Check if any changes occurred
        {
            scene.LoadScene();  // Reload the scene if changes were detected
        }
    }

}
