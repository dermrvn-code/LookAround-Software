using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class SceneChanger : MonoBehaviour
{
    public LogoLoadingOverlay loadingOverlay;
    public GameObject particlesGameobject;
    ParticleSystem particles;

    public MeshRenderer domeRenderer;

    public Material videoMaterial;
    public Material photoMaterial;

    public Texture mainScreenImage;

    public GameObject sceneElementsContainer;

    public VideoPlayer vp;
    SceneManager sm;
    InteractionHandler ih;
    Settings settings;
    TextureManager textureManager;

    Scene currentScene;

    void Start()
    {
        sm = FindObjectOfType<SceneManager>();
        ih = FindObjectOfType<InteractionHandler>();
        settings = FindObjectOfType<Settings>();
        textureManager = FindObjectOfType<TextureManager>();

        // To prevent particles in the editor window
        particlesGameobject.SetActive(true);
        particles = particlesGameobject.GetComponent<ParticleSystem>();
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

    public void LoadWorld()
    {
        SceneManager.worldsList.TryGetValue(SceneManager.currentWorld, out string path);
        if (path == "")
        {
            Debug.LogWarning("The current world is not in the worlds list");
            return;
        }
        sm.LoadScenesOverview(path, () =>
        {
            Debug.Log("Loading World: " + path);
            ToStartScene();
        });
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

    public void SwitchSceneAnimation(Scene scene, int index = -1)
    {
        if (scene == null || scene != currentScene)
        {
            if (index == -1)
            {
                TransitionParticles((sceneLoaded) =>
                            {
                                SwitchScene(scene, () =>
                                {
                                    sceneLoaded?.Invoke();
                                });
                            });
            }
            else
            {
                TransitionLogo((sceneLoaded) =>
                            {
                                SwitchScene(scene, () =>
                                {
                                    sceneLoaded?.Invoke();
                                });
                            }, logoIndex: index);

            }
        }
    }

    public void SwitchScene(Scene scene, Action onLoaded = null)
    {
        if (scene == null || scene != currentScene)
        {
            currentScene = scene;
            LoadSceneElements(scene.SceneElements);
            if (ih != null) ih.updateElementsNextFrame = true;


            try
            {
                if (!File.Exists(scene.Source))
                {
                    Debug.LogWarning("Scene media " + scene.Source + " does not exist");
                    return;
                }


                if (scene.Type == Scene.MediaType.Video)
                {
                    SwitchToVideo();
                    videoMaterial.mainTextureOffset = new Vector2(scene.XOffset, scene.YOffset);
                    vp.url = scene.Source;
                    onLoaded?.Invoke();
                }
                else if (scene.Type == Scene.MediaType.Photo)
                {
                    StartCoroutine(textureManager.GetTexture(scene.Source, texture =>
                    {
                        photoMaterial.mainTexture = texture;
                        photoMaterial.mainTextureOffset = new Vector2(scene.XOffset, scene.YOffset);
                        SwitchToFoto();
                        onLoaded?.Invoke();
                    }));
                }
                else
                {
                    Debug.LogWarning("Scene type not supported");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error while switching to scene " + scene.Name);
                Debug.LogError(e.Message);
            }
        }
    }



    public void LoadSceneElements(List<SceneElement> sceneElements)
    {
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

        InteractableArrow interactableArrow = arrow.GetComponent<InteractableArrow>();
        interactableArrow.OnInteract.AddListener(() =>
        {
            ActionParser(sceneElement.action);
        });

        Color color = ColorUtility.TryParseHtmlString(sceneElement.color, out Color unityColor) ? unityColor : Color.white;
        interactableArrow.color = color;


    }

    public static string[] actionTypes = { "toScene" };
    public void ActionParser(string action)
    {
        string pattern = @"toScene\(([^,]*?)(?:,(\d))*\)";
        Match match = Regex.Match(action, pattern);
        if (match.Success)
        {
            string sceneName = match.Groups[1].Value;

            int animationIndex = -1; // -1 = particle, 0,1,2,... = logo index
            if (match.Groups.Count > 2 && match.Groups[2].Success)
            {
                int.TryParse(match.Groups[2].Value.Trim(), out animationIndex);
            }

            Scene scene = sm.sceneList[sceneName];

            if (scene != null)
            {
                SwitchSceneAnimation(scene, animationIndex);
            }
        }
    }

    public void TransitionLogo(Action<Action> sceneLoaded, int logoIndex)
    {
        StartCoroutine(_FadeIn(sceneLoaded, logoIndex));
    }

    public void TransitionParticles(Action<Action> sceneLoaded)
    {
        StartCoroutine(_StartParticles(sceneLoaded));
    }

    private IEnumerator _StartParticles(Action<Action> sceneLoaded)
    {
        particles.Play();
        yield return new WaitForSeconds(2f);
        sceneLoaded.Invoke(() =>
        {
            StartCoroutine(_StopParticles());
        });

    }

    private IEnumerator _StopParticles()
    {
        yield return new WaitForSeconds(0.5f);
        particles.Stop();
    }

    public IEnumerator _FadeIn(Action<Action> sceneLoaded, int logoIndex)
    {
        loadingOverlay.SetLogoFromIndex(logoIndex);
        loadingOverlay.FadeIn();
        yield return new WaitForSeconds(2f);
        sceneLoaded.Invoke(() =>
        {
            StartCoroutine(_FadeOut());
        });
    }

    private IEnumerator _FadeOut()
    {
        yield return new WaitForSeconds(0.5f);
        loadingOverlay.FadeOut();
    }
}
