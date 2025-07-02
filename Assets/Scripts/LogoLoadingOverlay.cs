using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoLoadingOverlay : MonoBehaviour
{

    [SerializeField]
    float fadeDuration = 0.5f;

    [SerializeField]
    Renderer logoRenderer;

    [SerializeField]
    Renderer backgroundRenderer;

    Material backgroundMaterial;
    Material logoMaterial;

    [SerializeField]
    Texture logo;

    Color backgroundColor;
    Color logoColor;
    void Start()
    {
        backgroundMaterial = backgroundRenderer.sharedMaterial;
        logoMaterial = logoRenderer.sharedMaterial;

        backgroundColor = backgroundMaterial.color;
        logoColor = logoMaterial.color;

        Vector2 logoSize = new Vector2(logo.width, logo.height);

        float maxDimension = Mathf.Max(logoSize.x, logoSize.y);
        float aspectWidth = logoSize.x / maxDimension;
        float aspectHeight = logoSize.y / maxDimension;


        logoMaterial.mainTexture = logo;
        logoMaterial.SetFloat("_TexWidth", aspectWidth);
        logoMaterial.SetFloat("_TexHeight", aspectHeight);

        backgroundMaterial.color = new Color(backgroundColor.r, backgroundColor.g, backgroundColor.b, 0);
        logoMaterial.color = new Color(logoColor.r, logoColor.g, logoColor.b, 0);

        FadeOut();
    }

    public void FadeIn()
    {
        StartCoroutine(FadeInCoroutine());
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    private IEnumerator FadeInCoroutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            backgroundMaterial.color = new Color(backgroundColor.r, backgroundColor.g, backgroundColor.b, alpha);
            logoMaterial.color = new Color(logoColor.r, logoColor.g, logoColor.b, alpha);
            yield return null;
        }
    }

    private IEnumerator FadeOutCoroutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(1 - (elapsedTime / fadeDuration));
            backgroundMaterial.color = new Color(backgroundColor.r, backgroundColor.g, backgroundColor.b, alpha);
            logoMaterial.color = new Color(logoColor.r, logoColor.g, logoColor.b, alpha);
            yield return null;
        }
    }
}
