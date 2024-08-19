using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProgressLoader : MonoBehaviour
{
    [SerializeField]
    Transform progress;
    [SerializeField]
    TMP_Text progressText;
    [SerializeField]
    MeshRenderer meshRenderer;

    [SerializeField]
    string progressTextPrefix = "Loading: ";

    public bool show = false;
    float progressValue = 0;

    float currentProgressValue = 0;
    float progressSpeed = 2f;

    void Update()
    {
        progress.gameObject.SetActive(show);
        progressText.gameObject.SetActive(show);
        meshRenderer.enabled = show;

        if (show)
        {
            if (currentProgressValue != progressValue)
            {
                currentProgressValue = Mathf.Lerp(currentProgressValue, progressValue, progressSpeed * Time.deltaTime);
            }
            ScaleProgress(currentProgressValue);
        }
    }


    public void UpdateBar(float value, string message)
    {
        show = value < 1;
        progressValue = value;
        UpdateProgressText(message);
    }

    void UpdateProgressText(string message)
    {
        if (progressText != null)
        {
            progressText.text = progressTextPrefix + message;
        }
    }

    void ScaleProgress(float value)
    {
        Vector3 pos = progress.transform.position;
        Vector3 scale = progress.transform.localScale;

        scale.y = value;
        pos.x = -transform.localScale.y + scale.y;

        progress.transform.position = pos;
        progress.transform.localScale = scale;
    }
}
