using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

public class SceneChanger : MonoBehaviour
{
    public GameObject particles;

    public MeshRenderer domeRenderer;

    public Material videoMaterial;
    public Material photoMaterial;

    public Texture mainScreenImage;

    public GameObject sceneElementsContainer;

    public VideoPlayer vp;
    SceneManager sm;
    InteractionHandler ih;
    Settings settings;

    Scene currentScene;

    void Start()
    {
        sm = FindObjectOfType<SceneManager>();
        ih = FindObjectOfType<InteractionHandler>();
        settings = FindObjectOfType<Settings>();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ToMainScene()
    {
        photoMaterial.mainTexture = mainScreenImage;
        photoMaterial.mainTextureOffset = new Vector2(0, 0);

        SwitchToFoto();
    }

    public void LoadWorld(bool animate = true)
    {
        string path = "";
        SceneManager.worldsList.TryGetValue(SceneManager.currentWorld, out path);
        if (path == "")
        {
            Debug.LogWarning("The current world is not in the worlds list");
            return;
        }
        sm.LoadSceneOverview(path);
        Debug.Log("Loading World: " + path);
        ToStartScene(animate);
    }

    public void ToStartScene(bool animate = true)
    {
        bool foundScene = false;
        foreach (var scene in sm.sceneList)
        {
            if (scene.Value.IsStartScene)
            {
                foundScene = true;
                if (animate)
                {
                    SwitchSceneAnimation(scene.Value);
                }
                else
                {
                    SwitchScene(scene.Value);
                }
            }
        }
        if (!foundScene) Debug.LogWarning("There is no start scene specified");
    }


    void SwitchToVideo()
    {
        domeRenderer.material = videoMaterial;
    }
    void SwitchToFoto()
    {
        domeRenderer.material = photoMaterial;
    }

    public void SwitchSceneAnimation(Scene scene)
    {
        if(scene == null || scene != currentScene){
            TransitionParticles(() =>
            {
                SwitchScene(scene);
            });
        }
    }

    public void SwitchScene(Scene scene)
    {
        if(scene == null || scene != currentScene){
            currentScene = scene;
            LoadSceneElements(scene.SceneElements);
            if (ih != null) ih.updateElementsNextFrame = true;


            if (scene.Type == Scene.MediaType.Video)
            {
                SwitchToVideo();
                videoMaterial.mainTextureOffset = new Vector2(scene.XOffset, scene.YOffset);
                vp.url = scene.Source;
            }
            else
            {
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(File.ReadAllBytes(scene.Source));
                photoMaterial.mainTexture = tex;
                photoMaterial.mainTextureOffset = new Vector2(scene.XOffset, scene.YOffset);
                SwitchToFoto();
            }
            if (settings != null) settings.CloseView(); ;
        }
    }

    public void LoadSceneElements(List<SceneElement> sceneElements)
    {
        Debug.Log("Loading " + sceneElements.Count + " elements");
        var children = new List<GameObject>();
        foreach (Transform child in sceneElementsContainer.transform) children.Add(child.gameObject);
        if (Application.isPlaying)
        {
            children.ForEach(child => Destroy(child));
        }
        else
        {
            children.ForEach(child => DestroyImmediate(child));
        }


        foreach (var sceneElement in sceneElements)
        {
            if (sceneElement.type == SceneElement.ElementType.Text)
            {
                LoadTextElement(sceneElement);
            }
            else if (sceneElement.type == SceneElement.ElementType.Textbox)
            {
                LoadTextboxElement(sceneElement);
            }
            else if (sceneElement.type == SceneElement.ElementType.DirectionArrow)
            {
                LoadDirectionArrow(sceneElement);
            }
        }
    }

    [SerializeField]
    TMP_Text textPrefab;
    public void LoadTextElement(SceneElement sceneElement)
    {
        var text = Instantiate(textPrefab, sceneElementsContainer.transform);
        text.name = sceneElement.text;
        text.text = sceneElement.text;
        DomePosition dp = text.GetComponent<DomePosition>();
        dp.position.x = sceneElement.x;
        dp.position.y = sceneElement.y;
        dp.distance = sceneElement.distance;
        Interactable interactable = text.GetComponent<Interactable>();
        interactable.OnInteract.AddListener(() =>
        {
            ActionParser(sceneElement.action);
        });
    }

    [SerializeField]
    GameObject textboxPrefab;
    public Sprite info, warning, question, play;

    public void LoadTextboxElement(SceneElement sceneElement)
    {
        var text = Instantiate(textboxPrefab, sceneElementsContainer.transform);
        var tmptext = text.GetComponentInChildren<TMP_Text>();
        var spriteRenderer = text.GetComponentInChildren<SpriteRenderer>();
        tmptext.text = sceneElement.text;
        DomePosition dp = text.GetComponent<DomePosition>();
        dp.position.x = sceneElement.x;
        dp.position.y = sceneElement.y;
        dp.distance = sceneElement.distance;

        Sprite sprite;
        switch (sceneElement.icon)
        {
            case "info":
                sprite = info;
                break;

            case "warning":
                sprite = warning;
                break;

            case "question":
                sprite = question;
                break;

            case "play":
                sprite = play;
                break;

            default:
                sprite = null;
                break;
        }
        if (sprite != null)
        {
            spriteRenderer.sprite = sprite;
        }
    }


    [SerializeField]
    GameObject arrowPrefab;
    public void LoadDirectionArrow(SceneElement sceneElement)
    {
        var arrow = Instantiate(arrowPrefab, sceneElementsContainer.transform);

        arrow.GetComponentInChildren<InteractableArrow>().SetRotation(sceneElement.rotation);

        DomePosition dp = arrow.GetComponent<DomePosition>();
        dp.position.x = sceneElement.x;
        dp.position.y = sceneElement.y;
        dp.distance = sceneElement.distance;

        Interactable interactable = arrow.GetComponent<Interactable>();
        interactable.OnInteract.AddListener(() =>
        {
            ActionParser(sceneElement.action);
        });
    }

    public static string[] actionTypes = { "toScene" };
    public void ActionParser(string action)
    {
        string pattern = @"toScene\((.*?)\)";
        Match match = Regex.Match(action, pattern);
        if (match.Success)
        {
            string sceneName = match.Groups[1].Value; // Extrahiere den Parameter aus der ersten Gruppe

            Scene scene = sm.sceneList[sceneName];

            if (scene != null)
            {
                SwitchSceneAnimation(scene);
            }
        }
    }

    public void TransitionParticles(UnityAction action)
    {
        StartCoroutine(_StartParticles(action));
    }

    private IEnumerator _StartParticles(UnityAction action)
    {
        if (!particles.activeSelf)
        {
            particles.SetActive(true);
            yield return new WaitForSeconds(1.5f);
            action.Invoke();
            yield return new WaitForSeconds(2f);
            particles.SetActive(false);
        }
    }
}
