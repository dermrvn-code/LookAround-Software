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

    void Start()
    {
        sm = FindObjectOfType<SceneManager>();
        ih = FindObjectOfType<InteractionHandler>();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ToMainScreen()
    {
        photoMaterial.mainTexture = mainScreenImage;
        SwitchToFoto();
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
        TransitionParticles(() =>
        {
            SwitchScene(scene);
        });
    }

    public void SwitchScene(Scene scene)
    {
        LoadSceneElements(scene.SceneElements);
        ih.updateElementsNextFrame = true;
        if (scene.Type == Scene.MediaType.Video)
        {
            SwitchToVideo();
            vp.url = scene.Source;
        }
        else
        {
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(File.ReadAllBytes(scene.Source));
            photoMaterial.mainTexture = tex;

            SwitchToFoto();
        }
    }

    public TMP_Text textPrefab;
    public void LoadSceneElements(List<SceneElement> sceneElements)
    {

        var children = new List<GameObject>();
        foreach (Transform child in sceneElementsContainer.transform) children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));


        foreach (var sceneElement in sceneElements)
        {
            if (sceneElement.type == SceneElement.ElementType.Text)
            {
                LoadTextElement(sceneElement);
            }
        }
    }

    public void LoadTextElement(SceneElement sceneElement)
    {
        var text = Instantiate(textPrefab, sceneElementsContainer.transform);
        text.name = sceneElement.text;
        text.text = sceneElement.text;
        DomePosition dp = text.GetComponent<DomePosition>();
        dp.position.x = sceneElement.x;
        dp.position.y = sceneElement.y;
        Interactable interactable = text.GetComponent<Interactable>();
        Debug.Log(sceneElement.action);
        interactable.OnInteract.AddListener(() =>
        {
            ActionParser(sceneElement.action);
        });
    }

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
