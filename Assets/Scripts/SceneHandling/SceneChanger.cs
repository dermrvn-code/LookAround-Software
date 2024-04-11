using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

public class SceneChanger : MonoBehaviour
{
    public GameObject particles;

    public MeshRenderer domeRenderer;

    public Material videoMaterial;
    public Material fotoMaterial;

    public VideoPlayer vp;

    public void Quit()
    {
        Application.Quit();
    }


    /*  ============== TEMP ==============  */
    public VideoClip v1;
    public VideoClip v2;

    public bool switchScene = false;
    public bool switchFormat = false;


    void SwitchToVideo()
    {
        domeRenderer.material = videoMaterial;
    }
    void SwitchToFoto()
    {
        domeRenderer.material = fotoMaterial;
    }

    public void SwitchFormat(string sceneName)
    {
        TransitionParticles(() =>
        {

            if (switchFormat)
            {
                SwitchToVideo();
            }
            else
            {
                SwitchToFoto();
            }
            switchFormat = !switchFormat;
        });
    }

    public void SwitchScene(string sceneName)
    {
        if (switchFormat) return;

        TransitionParticles(() =>
        {

            if (switchScene)
            {
                vp.clip = v1;
            }
            else
            {
                vp.clip = v2;
            }
            switchScene = !switchScene;
        });
    }

    /*  =================================  */

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
